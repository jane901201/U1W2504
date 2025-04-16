using System;
using UnityEngine;

public class Escape : MonoBehaviour
{
    // 先暂时把地图参数作为类变量缓存，如果可以的话希望后期缓存到别的地方
    private class MapInfo
    {

        private float XSize;
        private float YSize;
        private int Length;

        // 默认分成9个区域
        public MapInfo(Vector2 mapSize, int partitions = 9)
        {
            double dLen = Math.Sqrt(partitions);
            int len = (int) dLen;

            if (partitions <= 0 || len != dLen)
            {
                throw new Exception("Partitions must be a perfect square.");
            }

            XSize = mapSize.x;
            YSize = mapSize.y;
            Length = len;
        }

        // 工具函数：查找坐标所在分区
        public int GetPartition(Vector3 position)
        {
            int xPartition = Mathf.FloorToInt(position.x / (XSize / Length));
            int yPartition = Mathf.FloorToInt(position.y / (YSize / Length));

            return yPartition * Length + xPartition;
        }

        // 工具函数：返回所在分区的相邻分区
        public int[] GetNearPartitions(int partition)
        {
            int x = partition % Length;
            int y = partition / Length;
            int[] neighbors = new int[4];  // 左、右、上、下相邻分区

            // 上
            if (y > 0) neighbors[0] = (y - 1) * Length + x;
            // 下
            if (y < Length - 1) neighbors[1] = (y + 1) * Length + x;
            // 左
            if (x > 0) neighbors[2] = y * Length + (x - 1);
            // 右
            if (x < Length - 1) neighbors[3] = y * Length + (x + 1);

            return neighbors;
        }
        
        // 工具函数：获取分区内随机点
        public Vector2 GetRandomPointInPartition(int partition)
        {
            int xPartition = partition % Length;
            int yPartition = partition / Length;

            float xPosition = (xPartition + UnityEngine.Random.Range(0f, 1f)) * (XSize / Length);
            float yPosition = (yPartition + UnityEngine.Random.Range(0f, 1f)) * (YSize / Length);
            
            return new Vector2(xPosition, yPosition);
        }
    }

    private static MapInfo _mapInfo;

    public BoxCollider2D tilemapCollider;  // 用于获取Tilemap大小

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(Vector2 mapSize, int partitions = 9)
    {
        _mapInfo = new MapInfo(mapSize);
    }

    public void Initialize(int partitions = 9)
    {
        tilemapCollider = GameObject.Find("Tilemap").GetComponent<BoxCollider2D>();
        if (tilemapCollider != null)
        {
            Vector2 mapSize = tilemapCollider.size;
            Initialize(mapSize, partitions);
        }
    }

    // 返回逃离目标坐标
    public Vector3 GetEscapeTargetLocation(Vector3 playerPosition, Vector3 myPosition)
    {
        int playerPartition = _mapInfo.GetPartition(playerPosition);
        // 获取自己的分区
        int myPartition = _mapInfo.GetPartition(myPosition);
        int[] nearbyPartitions = _mapInfo.GetNearPartitions(playerPartition);

        // 选择一个最远的分区
        int farthestPartition = -1;
        int maxDistance = -1;

        foreach (var partition in nearbyPartitions)
        {
            if (partition == -1 || partition == myPartition) {
                continue;  // 跳过无效的分区
            }
            // if (partition == -1 || partition == myPartition) {continue;}

            int dist = Math.Abs(playerPartition - partition);
            if (dist > maxDistance)
            {
                farthestPartition = partition;
                maxDistance = dist;
            }
        }

        Vector2 result = _mapInfo.GetRandomPointInPartition(farthestPartition);
        
        Debug.Log(result);
        return new Vector3(result.x, result.y, 0f);
    }
    
    // Test method
    public int GetPartition(Vector3 position)
    {
        return _mapInfo.GetPartition(position);
    }
}
