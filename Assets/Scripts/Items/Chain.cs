using UnityEngine;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Chain", menuName = "MyGame/Items/Chain")]
    public class Chain : IItem, IMapTileItem
    {

        public Tilemap groundTilemap;
        public Tilemap obstacleTilemap;
        
        [Header("各种参数")]
        [SerializeField] private int oniRange = 3;
        [SerializeField] private int humanRange = 6;
        [SerializeField] private string oniHint = "相手を近くに引き寄せた！";
        [SerializeField] private string humanHint = "相手を遠くに押し出した！";
        
        private bool isDurationItem => false;

        protected override void UseAsOni(ICharacter self, ICharacter[] targets)
        {
            
            foreach (var target in targets)
            {
                Vector3Int selfCell = groundTilemap.WorldToCell(self.transform.position);
                Vector3Int targetCell = groundTilemap.WorldToCell(target.transform.position);
                Vector3Int farPos = FindFarthestFromTargetButFree(selfCell, targetCell, humanRange);

                target.transform.position = groundTilemap.GetCellCenterWorld(farPos);
            }
        }

        protected override void UseAsHuman(ICharacter self, ICharacter[] targets)
        {
            foreach (var target in targets)
            {
                Vector3Int selfCell = groundTilemap.WorldToCell(self.transform.position);
                Vector3Int newCell = FindFreePositionNear(selfCell, oniRange);

                target.transform.position = groundTilemap.GetCellCenterWorld(newCell);
            }
        }

        // 用于鬼役：在周围找到某个合法位置
        private Vector3Int FindFreePositionNear(Vector3Int center, int radius)
        {
            for (int r = 1; r <= radius; r++)
            {
                for (int dx = -r; dx <= r; dx++)
                {
                    for (int dy = -r; dy <= r; dy++)
                    {
                        Vector3Int pos = center + new Vector3Int(dx, dy, 0);
                        if (!IsObstacle(pos)) return pos;
                    }
                }
            }

            return center;
        }

        // 用于人役：将对方传送到远处
        private Vector3Int FindFarthestFromTargetButFree(Vector3Int selfPos, Vector3Int targetPos, int range)
        {
            Vector3Int bestCell = targetPos;
            float bestDist = float.MinValue;

            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    Vector3Int check = selfPos + new Vector3Int(x, y, 0);
                    if (IsObstacle(check)) continue;

                    float dist = Vector2Int.Distance(new Vector2Int(check.x, check.y), new Vector2Int(selfPos.x, selfPos.y));
                    if (dist > bestDist)
                    {
                        bestDist = dist;
                        bestCell = check;
                    }
                }
            }

            return bestCell;
        }

        private bool IsObstacle(Vector3Int pos)
        {
            TileBase tile = obstacleTilemap.GetTile(pos);
            return tile != null;
        }

        public void SetTilemaps(Tilemap ground, Tilemap obstacle)
        {
            this.groundTilemap = ground;
            this.obstacleTilemap = obstacle;
        }

        public override float GetDuration(ICharacter self)
        {
            return 0f;
        }
        
        
    }
}
