namespace DefaultNamespace.Map
{
    public enum MapRegionType
    {
        None = 0,
        OpenGround = 1,     // 普通通行区域
        ObstacleCluster = 2, // 障碍群（草堆、杂物）
        Forest = 3,         // 树林（可穿越但复杂）
        Landmark = 4        // 特定结构区域
    }

}