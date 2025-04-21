namespace DefaultNamespace.Map
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public class SmartMapGenerator : MonoBehaviour
    {
        [Header("地图尺寸")]
        public int width = 30;
        public int height = 30;

        [Header("Tilemap组件")]
        public Tilemap groundTilemap;
        public Tilemap obstacleTilemap;

        [Header("Tile资源库")]
        public TileLibrary tileLibrary;

        [Header("种子设置（设为0表示随机）")]
        public int seed = 0;

        [Header("区域地图")]
        private MapRegionType[,] regionMap;

        [Header("路径连通检测")]
        public bool checkConnectivity = true;

        [Header("外围防跳格数")]
        public int boundaryWidth = 6;

        private Vector3Int offset;

        public void Generate()
        {
            if (seed != 0)
                Random.InitState(seed);
            else
                Random.InitState(System.DateTime.Now.Millisecond);

            offset = -new Vector3Int(width / 2, height / 2, 0);
            regionMap = new MapRegionType[width, height];

            groundTilemap.ClearAllTiles();
            obstacleTilemap.ClearAllTiles();

            GenerateRegions();
            ApplyTiles();

            if (checkConnectivity)
            {
                if (!IsMapConnected(new Vector2Int(width / 2, height / 2)))
                {
                    Debug.LogWarning("生成地图不连通，重新生成...");
                    Generate(); // 重试生成
                }
            }

            Debug.Log("地图生成完成");
        }

        void GenerateRegions()
        {
            // 初始全部为 OpenGround
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    regionMap[x, y] = MapRegionType.OpenGround;

            // 随机生成障碍簇群
            int clusters = width * height / 20;
            for (int i = 0; i < clusters; i++)
            {
                Vector2Int center = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
                int radius = Random.Range(2, 4);
                MapRegionType type = Random.value < 0.5f ? MapRegionType.ObstacleCluster : MapRegionType.Forest;
                PaintCluster(center, radius, type);
            }
        }

        void PaintCluster(Vector2Int center, int radius, MapRegionType type)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    int x = center.x + dx;
                    int y = center.y + dy;
                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        if (Random.value < 0.8f)
                            regionMap[x, y] = type;
                    }
                }
            }
        }

        void ApplyTiles()
        {
            for (int x = -boundaryWidth; x < width + boundaryWidth; x++)
            {
                for (int y = -boundaryWidth; y < height + boundaryWidth; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0) + offset;

                    bool inMainMap = x >= 0 && x < width && y >= 0 && y < height;
                    bool inFence = 
                        (x == 0 || x == width - 1 || y == 0 || y == height - 1);


                    if (!inMainMap)
                    {
                        obstacleTilemap.SetTile(pos, tileLibrary.invisibleBarrier);
                        continue;
                    }

                    MapRegionType region = regionMap[x, y];
                    groundTilemap.SetTile(pos,
                        region == MapRegionType.ObstacleCluster ? tileLibrary.obstacleGround : tileLibrary.normalGround);

                    if (region == MapRegionType.ObstacleCluster && Random.value < 0.3f)
                    {
                        Vector3 worldPos = groundTilemap.GetCellCenterWorld(pos);
                        PlaceObstacle(worldPos);
                    }

                    // 奇数宽度地图 → 四边围栏交替放置（横2格，竖1格）
                    if (width % 2 == 1)
                    {
                        int left = 0, right = width - 1, bottom = 0, top = height - 1;
                        bool useHorizontal = true;

                        // 从左下角开始顺时针围一圈
                        for (int i = 0; i < (width + height) * 2; )
                        {
                            if (useHorizontal)
                            {
                                // 横放2格
                                if (left + i / 2 < right)
                                {
                                    Vector3Int p1 = new Vector3Int(left + i / 2, bottom, 0) + offset;
                                    Vector3Int p2 = new Vector3Int(left + i / 2, top, 0) + offset;
                                    obstacleTilemap.SetTile(p1, tileLibrary.fenceHorizontal);
                                    obstacleTilemap.SetTile(p2, tileLibrary.fenceHorizontal);
                                }
                                i += 2;
                            }
                            else
                            {
                                // 竖放1格
                                if (bottom + i / 2 < top)
                                {
                                    Vector3Int p1 = new Vector3Int(left, bottom + i / 2, 0) + offset;
                                    Vector3Int p2 = new Vector3Int(right, bottom + i / 2, 0) + offset;
                                    obstacleTilemap.SetTile(p1, tileLibrary.fenceVertical);
                                    obstacleTilemap.SetTile(p2, tileLibrary.fenceVertical);
                                }
                                i += 1;
                            }
                            useHorizontal = !useHorizontal;
                        }
                    }

                }
            }
        }

        void PlaceObstacle(Vector3 worldPos)
        {
            if (tileLibrary.singleObstaclePrefabs.Count > 0)
            {
                int index = Random.Range(0, tileLibrary.singleObstaclePrefabs.Count);
                Instantiate(tileLibrary.singleObstaclePrefabs[index], worldPos, Quaternion.identity);
            }
        }

        bool IsMapConnected(Vector2Int start)
        {
            bool[,] visited = new bool[width, height];
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(start);
            visited[start.x, start.y] = true;
            int reachable = 1;

            int[] dx = {1, -1, 0, 0};
            int[] dy = {0, 0, 1, -1};

            while (queue.Count > 0)
            {
                var p = queue.Dequeue();
                for (int d = 0; d < 4; d++)
                {
                    int nx = p.x + dx[d];
                    int ny = p.y + dy[d];
                    if (nx >= 0 && nx < width && ny >= 0 && ny < height && !visited[nx, ny])
                    {
                        if (regionMap[nx, ny] != MapRegionType.ObstacleCluster)
                        {
                            queue.Enqueue(new Vector2Int(nx, ny));
                            visited[nx, ny] = true;
                            reachable++;
                        }
                    }
                }
            }

            return reachable > (width * height / 2);
        }
    }
}