namespace DefaultNamespace.Map
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    [CreateAssetMenu(fileName = "TileLibrary", menuName = "MyGame/TileLibrary")]
    public class TileLibrary : ScriptableObject
    {
        [Header("普通路径地面")]
        public TileBase normalGround;

        [Header("障碍物区域地面")]
        public TileBase obstacleGround;

        [Header("转角Tile（按方向排列：下右，下左，上右，上左）")]
        public TileBase turn1; // 下 + 右
        public TileBase turn2; // 下 + 左
        public TileBase turn3; // 上 + 右
        public TileBase turn4; // 上 + 左

        [Header("围栏Tile")]
        public TileBase fenceHorizontal;
        public TileBase fenceVertical;

        [Header("隐形边界Tile（用于限制传送范围）")]
        public TileBase invisibleBarrier;

        [Header("障碍物Prefab（1格）")]
        public List<GameObject> singleObstaclePrefabs;

        [Header("障碍物Prefab（2格，横向）")]
        public List<GameObject> doubleHorizontalPrefabs;

        [Header("障碍物Prefab（3格，横向）")]
        public List<GameObject> tripleHorizontalPrefabs;
    }

}