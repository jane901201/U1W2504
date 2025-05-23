﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PolyNav
{

    ///<summary>Main class for map generation and navigation</summary>
    public class PolyNavMap : MonoBehaviour
    {

        [Tooltip("The Shape used. Changing this will also change the Collider2D component type.")]
        public PolyNavObstacle.ShapeType shapeType = PolyNavObstacle.ShapeType.Polygon;
        [Tooltip("All PolyNavObstacles in the selected layers will be monitored and added/removed automatically.")]
        public LayerMask obstaclesMask = -1;
        [Tooltip("The frame interval to auto update obstacle chages (enabled/disable or transform change). Set 0 to disable auto-regenerate.")]
        public int autoRegenerateInterval = 60;
        [Tooltip("The radius from the edges to offset the agents.")]
        public float radiusOffset = 0.1f;
        [Tooltip("Inverts the master polygon.")]
        public bool invertMasterPolygon = false;

        ///<summary>Raised when a PolyNav map is generated (or re-generated), with argument being the PolyNav component.</summary>
        public static event Action<PolyNavMap> onMapGenerated;

        private List<PolyNavObstacle> navObstacles;
        private bool useCustomMap;
        private PolyMap map;
        private List<PathNode> nodes = new List<PathNode>();
        private PathNode[] tempNodes;

        private Queue<PathRequest> pathRequests = new Queue<PathRequest>();
        private PathRequest currentRequest;
        private bool isProcessingPath;
        private PathNode startNode;
        private PathNode endNode;
        private bool regenerateFlag;


        private Collider2D _masterCollider;
        private Collider2D masterCollider {
            get
            {
                if ( _masterCollider != null ) {
                    return _masterCollider;
                }
                var colliders = GetComponents<Collider2D>();
                var containsComposite = colliders.Any(c => c is CompositeCollider2D);
                return _masterCollider = ( containsComposite ? colliders.FirstOrDefault(c => c is CompositeCollider2D) : colliders.FirstOrDefault() );
            }
        }

        ///<summary>The current / first instance found of a PolyNavMap</summary>
        private static PolyNavMap _current;
        public static PolyNavMap current {
            get
            {
                if ( _current == null || !Application.isPlaying ) {
                    _current = FindFirstObjectByType<PolyNavMap>();
                }

                return _current;
            }
        }

        ///<summary>The total nodes count of the map</summary>
        public int nodesCount {
            get { return nodes.Count; }
        }

        ///<summary>Is the pathfinder currently processing a pathfinding request?</summary>
        public bool pendingRequest {
            get { return isProcessingPath; }
        }

        ///----------------------------------------------------------------------------------------------

        void Awake() {
            if ( _current == null ) {
                _current = this;
            }
            regenerateFlag = false;
            isProcessingPath = false;
            navObstacles = FindObjectsByType<PolyNavObstacle>(FindObjectsSortMode.None).Where(o => obstaclesMask == ( obstaclesMask | 1 << o.gameObject.layer )).ToList();
            PolyNavObstacle.OnObstacleStateChange += MonitorObstacle;
            if ( masterCollider != null ) {
                masterCollider.enabled = false;
                GenerateMap(true);
            }
        }

        void LateUpdate() {

            if ( useCustomMap || autoRegenerateInterval <= 0 ) {
                return;
            }

            if ( Time.frameCount % autoRegenerateInterval != 0 ) {
                return;
            }

            for ( var i = 0; i < navObstacles.Count; i++ ) {
                var obstacle = navObstacles[i];
                if ( obstacle.transform.hasChanged ) {
                    obstacle.transform.hasChanged = false;
                    regenerateFlag = true;
                }
            }

            if ( regenerateFlag == true ) {
                regenerateFlag = false;
                GenerateMap(false);
            }
        }

        void MonitorObstacle(PolyNavObstacle obstacle, bool active) {
            if ( obstaclesMask == ( obstaclesMask | 1 << obstacle.gameObject.layer ) ) {
                if ( active ) { AddObstacle(obstacle); } else { RemoveObstacle(obstacle); }
            }
        }

        ///<summary>Adds a PolyNavObstacle to the map.</summary>
        void AddObstacle(PolyNavObstacle navObstacle) {
            if ( !navObstacles.Contains(navObstacle) ) {
                navObstacles.Add(navObstacle);
                regenerateFlag = true;
            }
        }

        ///<summary>Removes a PolyNavObstacle from the map.</summary>
        void RemoveObstacle(PolyNavObstacle navObstacle) {
            if ( navObstacles.Contains(navObstacle) ) {
                navObstacles.Remove(navObstacle);
                regenerateFlag = true;
            }
        }

        ///<summary>Generate the map</summary>
        public void GenerateMap() { GenerateMap(true); }
        public void GenerateMap(bool generateMaster) {
            CreatePolyMap(generateMaster);
            CreateNodes();
            LinkNodes(nodes);
            if ( onMapGenerated != null ) {
                onMapGenerated(this);
            }
        }


        ///<summary>Use this to provide a custom map for generation	</summary>
        public void SetCustomMap(PolyMap map) {
            useCustomMap = true;
            this.map = map;
            CreateNodes();
            LinkNodes(nodes);
        }

        ///----------------------------------------------------------------------------------------------

        ///<summary>Find a path 'from' and 'to', providing a callback for when path is ready containing the path.</summary>
        public void FindPath(Vector2 start, Vector2 end, System.Action<Vector2[]> callback) {

            if ( CheckLOS(start, end) ) {
                callback(new Vector2[] { start, end });
                return;
            }

            pathRequests.Enqueue(new PathRequest(start, end, callback));
            TryNextFindPath();
        }

        //Pathfind next request
        void TryNextFindPath() {

            if ( isProcessingPath || pathRequests.Count <= 0 ) {
                return;
            }

            isProcessingPath = true;
            currentRequest = pathRequests.Dequeue();

            if ( !PointIsValid(currentRequest.start) ) {
                currentRequest.start = GetCloserEdgePoint(currentRequest.start);
            }

            //create start & end as temp nodes
            startNode = new PathNode(currentRequest.start);
            endNode = new PathNode(currentRequest.end);

            nodes.Add(startNode);
            LinkStart(startNode, nodes);

            nodes.Add(endNode);
            LinkEnd(endNode, nodes);

            AStar.CalculatePath(startNode, endNode, nodes, RequestDone);
        }


        //Pathfind request finished (path found or not)
        void RequestDone(Vector2[] path) {

            //Remove temp start and end nodes
            for ( int i = 0; i < endNode.links.Count; i++ ) {
                nodes[endNode.links[i]].links.Remove(nodes.IndexOf(endNode));
            }
            nodes.Remove(endNode);
            nodes.Remove(startNode);
            //			

            isProcessingPath = false;
            currentRequest.callback(path);
            TryNextFindPath();
        }

        //takes all colliders points and convert them to usable stuff
        void CreatePolyMap(bool generateMaster) {

            var masterPolys = new List<Polygon>();
            var obstaclePolys = new List<Polygon>();

            //create a polygon object for each obstacle
            for ( int i = 0; i < navObstacles.Count; i++ ) {
                var obstacle = navObstacles[i];
                if ( obstacle == null ) {
                    continue;
                }

                var rad = radiusOffset + obstacle.extraOffset;
                for ( var p = 0; p < obstacle.GetPathCount(); p++ ) {
                    var points = obstacle.GetPathPoints(p);
                    var transformed = TransformPoints(ref points, obstacle.transform);
                    var inflated = InflatePolygon(ref transformed, rad);
                    obstaclePolys.Add(new Polygon(inflated));
                }
            }

            if ( generateMaster ) {

                if ( masterCollider is PolygonCollider2D ) {

                    var polyCollider = (PolygonCollider2D)masterCollider;

                    for ( int i = 0; i < polyCollider.pathCount; ++i ) {
                        var points = polyCollider.GetPath(i);

                        //invert the main polygon points so that we save checking for inward/outward later (for Inflate)
                        if ( invertMasterPolygon ) {
                            System.Array.Reverse(points);
                        }

                        var transformed = TransformPoints(ref points, polyCollider.transform);
                        var inflated = InflatePolygon(ref transformed, Mathf.Max(0.01f, radiusOffset));
                        masterPolys.Add(new Polygon(inflated));
                    }

                } else if ( masterCollider is BoxCollider2D ) {
                    var box = (BoxCollider2D)masterCollider;
                    var tl = box.offset + new Vector2(-box.size.x, box.size.y) / 2;
                    var tr = box.offset + new Vector2(box.size.x, box.size.y) / 2;
                    var br = box.offset + new Vector2(box.size.x, -box.size.y) / 2;
                    var bl = box.offset + new Vector2(-box.size.x, -box.size.y) / 2;
                    var points = new Vector2[] { tl, bl, br, tr };
                    var transformed = TransformPoints(ref points, masterCollider.transform);
                    var inflated = InflatePolygon(ref transformed, Mathf.Max(0.01f, radiusOffset));
                    masterPolys.Add(new Polygon(inflated));

                } else if ( masterCollider is CompositeCollider2D ) {

                    var polyCollider = (CompositeCollider2D)masterCollider;

                    for ( int i = 0; i < polyCollider.pathCount; ++i ) {

                        var points = new Vector2[polyCollider.GetPathPointCount(i)];
                        polyCollider.GetPath(i, points);

                        //invert the main polygon points so that we save checking for inward/outward later (for Inflate)
                        if ( invertMasterPolygon ) {
                            System.Array.Reverse(points);
                        }

                        var transformed = TransformPoints(ref points, polyCollider.transform);
                        var inflated = InflatePolygon(ref transformed, Mathf.Max(0.01f, radiusOffset));
                        masterPolys.Add(new Polygon(inflated));
                    }
                }

            } else {

                if ( map != null ) {
                    masterPolys = map.masterPolygons.ToList();
                }
            }

            //create the main polygon map (based on inverted) also containing the obstacle polygons
            map = new PolyMap(masterPolys.ToArray(), obstaclePolys.ToArray());

            //
            //The colliders are never used again after this point. They are simply a drawing method.
            //
        }


        //Create Nodes at convex points (since master poly is inverted, it will be concave for it) if they are valid
        void CreateNodes() {

            nodes.Clear();

            for ( int p = 0; p < map.allPolygons.Length; p++ ) {
                var poly = map.allPolygons[p];
                //Inflate even more for nodes, by a marginal value to allow CheckLOS between them
                var points = poly.points.ToArray();
                var inflatedPoints = InflatePolygon(ref points, 0.05f);
                for ( int i = 0; i < inflatedPoints.Length; i++ ) {

                    //if point is concave dont create a node
                    if ( PointIsConcave(inflatedPoints, i) ) {
                        continue;
                    }

                    //if point is not in valid area dont create a node
                    if ( !PointIsValid(inflatedPoints[i]) ) {
                        continue;
                    }

                    nodes.Add(new PathNode(inflatedPoints[i]));
                }
            }
        }

        //link the nodes provided
        void LinkNodes(List<PathNode> nodeList) {

            for ( int a = 0; a < nodeList.Count; a++ ) {

                nodeList[a].links.Clear();

                for ( int b = 0; b < nodeList.Count; b++ ) {

                    if ( b > a ) {
                        continue;
                    }

                    if ( nodeList[a] == nodeList[b] ) {
                        continue;
                    }

                    if ( CheckLOS(nodeList[a].pos, nodeList[b].pos) ) {
                        nodeList[a].links.Add(b);
                        nodeList[b].links.Add(a);
                    }
                }
            }
        }


        //Link the start node in
        void LinkStart(PathNode start, List<PathNode> toNodes) {
            for ( int i = 0; i < toNodes.Count; i++ ) {
                if ( CheckLOS(start.pos, toNodes[i].pos) ) {
                    start.links.Add(i);
                }
            }
        }

        //Link the end node in
        void LinkEnd(PathNode end, List<PathNode> toNodes) {
            for ( int i = 0; i < toNodes.Count; i++ ) {
                if ( CheckLOS(end.pos, toNodes[i].pos) ) {
                    end.links.Add(i);
                    toNodes[i].links.Add(toNodes.IndexOf(end));
                }
            }
        }


        ///<summary>Determine if 2 points see each other.</summary>
        public bool CheckLOS(Vector2 posA, Vector2 posB) {
            // return !Physics2D.CircleCast(posA, radiusOffset / 2, ( posB - posA ).normalized, ( posA - posB ).magnitude, obstaclesMask.value);
            if ( ( posA - posB ).magnitude < Mathf.Epsilon ) {
                return true;
            }

            for ( int i = 0; i < map.allPolygons.Length; i++ ) {
                var poly = map.allPolygons[i];
                for ( int j = 0; j < poly.points.Length; j++ ) {
                    if ( SegmentsCross(posA, posB, poly.points[j], poly.points[( j + 1 ) % poly.points.Length]) ) {
                        return false;
                    }
                }
            }
            return true;
        }

        ///<summary>determine if a point is within a valid (walkable) area.</summary>
        public bool PointIsValid(Vector2 point) {
            // return Physics2D.OverlapCircle(point, radiusOffset / 2, obstaclesMask.value) == null;
            for ( int i = 0; i < map.allPolygons.Length; i++ ) {
                if ( i == 0 ? !PointInsidePolygon(map.allPolygons[i].points, point) : PointInsidePolygon(map.allPolygons[i].points, point) ) {
                    return false;
                }
            }
            return true;
        }

        //helper function
        Vector2[] TransformPoints(ref Vector2[] points, Transform t) {
            for ( int i = 0; i < points.Length; i++ ) {
                points[i] = t.TransformPoint(points[i]);
            }
            return points;
        }


        ///<summary>Return points representing an enlarged polygon.</summary>
        public static Vector2[] InflatePolygon(ref Vector2[] points, float dist) {
            var enlarged_points = new Vector2[points.Length];
            for ( var j = 0; j < points.Length; j++ ) {
                // Find the new location for point j.
                // Find the points before and after j.
                var i = ( j - 1 );
                if ( i < 0 ) { i += points.Length; }
                var k = ( j + 1 ) % points.Length;

                // Move the points by the offset.
                var v1 = new Vector2(points[j].x - points[i].x, points[j].y - points[i].y).normalized;
                v1 *= dist;
                var n1 = new Vector2(-v1.y, v1.x);

                var pij1 = new Vector2((float)( points[i].x + n1.x ), (float)( points[i].y + n1.y ));
                var pij2 = new Vector2((float)( points[j].x + n1.x ), (float)( points[j].y + n1.y ));

                var v2 = new Vector2(points[k].x - points[j].x, points[k].y - points[j].y).normalized;
                v2 *= dist;
                var n2 = new Vector2(-v2.y, v2.x);

                var pjk1 = new Vector2((float)( points[j].x + n2.x ), (float)( points[j].y + n2.y ));
                var pjk2 = new Vector2((float)( points[k].x + n2.x ), (float)( points[k].y + n2.y ));

                // See where the shifted lines ij and jk intersect.
                bool lines_intersect, segments_intersect;
                Vector2 poi, close1, close2;
                FindIntersection(pij1, pij2, pjk1, pjk2, out lines_intersect, out segments_intersect, out poi, out close1, out close2);
                Debug.Assert(lines_intersect, "Edges " + i + "-->" + j + " and " + j + "-->" + k + " are parallel");
                enlarged_points[j] = poi;
            }

            return enlarged_points.ToArray();
        }

        ///<summary>Find the point of intersection between the lines p1 --> p2 and p3 --> p4.</summary>
        public static void FindIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out bool lines_intersect, out bool segments_intersect, out Vector2 intersection, out Vector2 close_p1, out Vector2 close_p2) {
            // Get the segments' parameters.
            var dx12 = p2.x - p1.x;
            var dy12 = p2.y - p1.y;
            var dx34 = p4.x - p3.x;
            var dy34 = p4.y - p3.y;

            // Solve for t1 and t2
            var denominator = ( dy12 * dx34 - dx12 * dy34 );

            var t1 = ( ( p1.x - p3.x ) * dy34 + ( p3.y - p1.y ) * dx34 ) / denominator;
            if ( float.IsInfinity(t1) ) {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new Vector2(float.NaN, float.NaN);
                close_p1 = new Vector2(float.NaN, float.NaN);
                close_p2 = new Vector2(float.NaN, float.NaN);
                return;
            }
            lines_intersect = true;

            var t2 = ( ( p3.x - p1.x ) * dy12 + ( p1.y - p3.y ) * dx12 ) / -denominator;

            // Find the point of intersection.
            intersection = new Vector2(p1.x + dx12 * t1, p1.y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect = ( ( t1 >= 0 ) && ( t1 <= 1 ) && ( t2 >= 0 ) && ( t2 <= 1 ) );

            // Find the closest points on the segments.
            if ( t1 < 0 ) { t1 = 0; } else if ( t1 > 1 ) { t1 = 1; }
            if ( t2 < 0 ) { t2 = 0; } else if ( t2 > 1 ) { t2 = 1; }

            close_p1 = new Vector2(p1.x + dx12 * t1, p1.y + dy12 * t1);
            close_p2 = new Vector2(p3.x + dx34 * t2, p3.y + dy34 * t2);
        }

        ///<summary>Check intersection of two segments, each defined by two vectors.</summary>
        public static bool SegmentsCross(Vector2 a, Vector2 b, Vector2 c, Vector2 d) {
            bool c1 = ( d.y - a.y ) * ( c.x - a.x ) > ( c.y - a.y ) * ( d.x - a.x );
            bool c2 = ( d.y - b.y ) * ( c.x - b.x ) > ( c.y - b.y ) * ( d.x - b.x );
            bool c3 = ( c.y - a.y ) * ( b.x - a.x ) > ( b.y - a.y ) * ( c.x - a.x );
            bool c4 = ( d.y - a.y ) * ( b.x - a.x ) > ( b.y - a.y ) * ( d.x - a.x );
            return c1 != c2 && c3 != c4;
        }

        ///<summary>Check if or not a point is concave to the polygon points provided</summary>
        public static bool PointIsConcave(Vector2[] points, int pointIndex) {
            Vector2 current = points[pointIndex];
            Vector2 next = points[( pointIndex + 1 ) % points.Length];
            Vector2 previous = points[pointIndex == 0 ? points.Length - 1 : pointIndex - 1];
            Vector2 left = new Vector2(current.x - previous.x, current.y - previous.y);
            Vector2 right = new Vector2(next.x - current.x, next.y - current.y);
            return ( left.x * right.y ) - ( left.y * right.x ) > 0;
        }

        ///<summary>Is a point inside a polygon?</summary>
        public static bool PointInsidePolygon(Vector2[] polyPoints, Vector2 point) {

            float xMin = float.PositiveInfinity;
            for ( int i = 0; i < polyPoints.Length; i++ ) {
                xMin = Mathf.Min(xMin, polyPoints[i].x);
            }

            Vector2 origin = new Vector2(xMin - 0.1f, point.y);
            int intersections = 0;

            for ( int i = 0; i < polyPoints.Length; i++ ) {

                Vector2 pA = polyPoints[i];
                Vector2 pB = polyPoints[( i + 1 ) % polyPoints.Length];

                if ( SegmentsCross(origin, point, pA, pB) ) {
                    intersections++;
                }
            }

            return ( intersections & 1 ) == 1;
        }

        ///<summary>Finds the closer edge point to the navigation valid area</summary>
        public Vector2 GetCloserEdgePoint(Vector2 point) {

            var possiblePoints = new List<Vector2>();
            var closerVertex = Vector2.zero;
            var closerVertexDist = Mathf.Infinity;

            for ( int p = 0; p < map.allPolygons.Length; p++ ) {

                var poly = map.allPolygons[p];
                var points = poly.points.ToArray();
                var inflatedPoints = InflatePolygon(ref points, 0.01f);

                for ( int i = 0; i < inflatedPoints.Length; i++ ) {

                    Vector2 a = inflatedPoints[i];
                    Vector2 b = inflatedPoints[( i + 1 ) % inflatedPoints.Length];

                    Vector2 originalA = poly.points[i];
                    Vector2 originalB = poly.points[( i + 1 ) % poly.points.Length];

                    Vector2 proj = (Vector2)Vector3.Project(( point - a ), ( b - a )) + a;

                    if ( SegmentsCross(point, proj, originalA, originalB) && PointIsValid(proj) ) {
                        possiblePoints.Add(proj);
                    }

                    float dist = ( point - inflatedPoints[i] ).sqrMagnitude;
                    if ( dist < closerVertexDist && PointIsValid(inflatedPoints[i]) ) {
                        closerVertexDist = dist;
                        closerVertex = inflatedPoints[i];
                    }
                }
            }

            possiblePoints.Add(closerVertex);

            var closerDist = Mathf.Infinity;
            var index = 0;
            for ( int i = 0; i < possiblePoints.Count; i++ ) {
                var dist = ( point - possiblePoints[i] ).sqrMagnitude;
                if ( dist < closerDist ) {
                    closerDist = dist;
                    index = i;
                }
            }
            Debug.DrawLine(point, possiblePoints[index]);
            return possiblePoints[index];
        }



        ///----------------------------------------------------------------------------------------------
        ///----------------------------------------------------------------------------------------------
        ///----------------------------------------------------------------------------------------------


        ///<summary>Defines the main navigation polygon and its sub obstacle polygons</summary>
        public class PolyMap
        {
            public Polygon[] masterPolygons;
            public Polygon[] obstaclePolygons;
            public Polygon[] allPolygons { get; private set; }

            public PolyMap(Polygon[] masterPolys, params Polygon[] obstaclePolys) {
                masterPolygons = masterPolys;
                obstaclePolygons = obstaclePolys;
                var temp = new List<Polygon>();
                temp.AddRange(masterPolys);
                temp.AddRange(obstaclePolys);
                allPolygons = temp.ToArray();
            }

            public PolyMap(Polygon masterPoly, params Polygon[] obstaclePolys) {
                masterPolygons = new Polygon[] { masterPoly };
                obstaclePolygons = obstaclePolys;
                var temp = new List<Polygon>();
                temp.Add(masterPoly);
                temp.AddRange(obstaclePolys);
                allPolygons = temp.ToArray();
            }
        }

        ///<summary>Defines a polygon.</summary>
        public struct Polygon
        {
            public Vector2[] points;
            public Polygon(Vector2[] points) {
                this.points = points;
            }
        }

        ///<summary>defines a node for A*</summary>
        public class PathNode : IHeapItem<PathNode>
        {
            public Vector2 pos;
            public List<int> links;
            public float gCost;
            public float hCost;
            public PathNode parent;

            public PathNode(Vector2 pos) {
                this.pos = pos;
                this.links = new List<int>();
                this.gCost = 1f;
                this.hCost = 0f;
                this.parent = null;
            }

            public float fCost {
                get { return gCost + hCost; }
            }

            int IHeapItem<PathNode>.heapIndex { get; set; }

            int IComparable<PathNode>.CompareTo(PathNode other) {
                int compare = fCost.CompareTo(other.fCost);
                if ( compare == 0 ) {
                    compare = hCost.CompareTo(other.hCost);
                }
                return -compare;
            }
        }

        ///<summary>used for internal path requests</summary>
        struct PathRequest
        {
            public Vector2 start;
            public Vector2 end;
            public Action<Vector2[]> callback;

            public PathRequest(Vector2 start, Vector2 end, Action<Vector2[]> callback) {
                this.start = start;
                this.end = end;
                this.callback = callback;
            }
        }



        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        void OnDrawGizmos() {

            //the original drawn polygons
            if ( !useCustomMap ) {

                if ( !Application.isPlaying ) {
                    navObstacles = FindObjectsByType<PolyNavObstacle>(FindObjectsSortMode.None).Where(o => obstaclesMask == ( obstaclesMask | 1 << o.gameObject.layer )).ToList();
                    CreatePolyMap(true);
                }

                if ( masterCollider is PolygonCollider2D ) {
                    var polyCollider = (PolygonCollider2D)masterCollider;
                    for ( int i = 0; i < polyCollider.pathCount; ++i ) {
                        var points = polyCollider.GetPath(i).ToArray();
                        points = TransformPoints(ref points, polyCollider.transform);
                        for ( int p = 0; p < points.Length; ++p ) {
                            DebugDrawPolygon(points, Color.green);
                        }
                    }

                } else if ( masterCollider is BoxCollider2D ) {
                    var box = masterCollider as BoxCollider2D;
                    var tl = box.offset + new Vector2(-box.size.x, box.size.y) / 2;
                    var tr = box.offset + new Vector2(box.size.x, box.size.y) / 2;
                    var br = box.offset + new Vector2(box.size.x, -box.size.y) / 2;
                    var bl = box.offset + new Vector2(-box.size.x, -box.size.y) / 2;
                    var points = new Vector2[] { tl, tr, br, bl };
                    points = TransformPoints(ref points, masterCollider.transform);
                    DebugDrawPolygon(points, Color.green);

                } else if ( masterCollider is CompositeCollider2D ) {
                    var polyCollider = (CompositeCollider2D)masterCollider;
                    for ( int i = 0; i < polyCollider.pathCount; ++i ) {
                        var points = new Vector2[polyCollider.GetPathPointCount(i)];
                        polyCollider.GetPath(i, points);
                        points = TransformPoints(ref points, polyCollider.transform);
                        DebugDrawPolygon(points, Color.green);
                    }
                }

                foreach ( var obstacle in navObstacles ) {
                    if ( obstacle != null ) {
                        for ( var i = 0; i < obstacle.GetPathCount(); i++ ) {
                            var points = obstacle.GetPathPoints(i);
                            points = TransformPoints(ref points, obstacle.transform);
                            DebugDrawPolygon(points, Color.green);
                        }
                    }
                }

            }
            //

            //the inflated actualy used polygons
            if ( map != null ) {
                foreach ( Polygon pathPoly in map.masterPolygons ) {
                    DebugDrawPolygon(pathPoly.points, Color.grey);
                }

                foreach ( Polygon poly in map.obstaclePolygons ) {
                    DebugDrawPolygon(poly.points, Color.grey);
                }
            }
            //
        }

        //helper debug function
        void DebugDrawPolygon(Vector2[] points, Color color) {
            for ( int i = 0; i < points.Length; i++ ) {
                Gizmos.color = color;
                Gizmos.DrawLine(points[i], points[( i + 1 ) % points.Length]);
                Gizmos.color = Color.white;
            }
        }

        ///----------------------------------------------------------------------------------------------

        void Reset() {
            if ( masterCollider == null ) {
                _masterCollider = gameObject.AddComponent<PolygonCollider2D>();
            }
            _masterCollider.enabled = false;
        }

        [UnityEditor.MenuItem("Tools/ParadoxNotion/PolyNav/Create PolyNav Map", false, 600)]
        public static void Create() {
            var newNav = new GameObject("@PolyNavMap").AddComponent<PolyNavMap>();
            UnityEditor.Selection.activeObject = newNav;
        }

        [UnityEditor.MenuItem("Tools/ParadoxNotion/PolyNav/Create PolyNav Obstacle", false, 600)]
        public static void CreatePolyNavObstacle() {
            var obs = new GameObject("PolyNavObstacle").AddComponent<PolyNavObstacle>();
            UnityEditor.Selection.activeObject = obs;
        }

        [UnityEditor.MenuItem("Tools/ParadoxNotion/PolyNav/Website...", false, 600)]
        public static void VisitWebsite() {
            UnityEditor.Help.BrowseURL("https://paradoxnotion.com/polynav/");
        }

#endif

    }
}