using UnityEngine;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "CD", menuName = "MyGame/Items/CD")]
    public class CD : IItem, IMapTileItem
    {
        [Header("提示文本")]
        [SerializeField] private string hintText = "障害物を一つ乗り越えた！";
        
        private bool isDurationItem => false;

        public Tilemap groundTilemap;
        public Tilemap obstacleTilemap;

        protected override void UseAsOni(ICharacter self, ICharacter[] targets)
        {
            TryLeapOverObstacle(self);
        }

        protected override void UseAsHuman(ICharacter self, ICharacter[] targets)
        {
            TryLeapOverObstacle(self);
        }

        private void TryLeapOverObstacle(ICharacter self)
        {
            Vector3 rawDir = self.LastMoveDirection;
            if (rawDir == Vector3.zero) return;
            
            Vector3 dir = rawDir.normalized;
            Vector3Int cellDir = GetDominantCellDirection(dir);
            Vector3 rawPos = self.GetComponent<Rigidbody2D>().position;
            
            Vector3 offsetPos = rawPos + (- (Vector3)cellDir) * 0.3f;

            // Vector3Int currentCell = groundTilemap.WorldToCell(self.transform.position);
            Vector3Int currentCell = groundTilemap.WorldToCell(offsetPos);
            
            for (int i = 1; i < 5; i++)
            {
                Vector3Int landCell = currentCell + cellDir * (i + 1);
                
                if (!IsObstacle(landCell))
                {
                    Vector3 worldPos = groundTilemap.GetCellCenterWorld(landCell);
                    self.transform.position = worldPos;
                    return;
                }
            }
        }


        // 方向单位向量（上下左右）
        private Vector3Int GetDominantCellDirection(Vector3 input)
        {
            Vector3Int[] directions = {
                new Vector3Int(1, 0, 0),   // →
                new Vector3Int(1, 1, 0),   // ↗
                new Vector3Int(0, 1, 0),   // ↑
                new Vector3Int(-1, 1, 0),  // ↖
                new Vector3Int(-1, 0, 0),  // ←
                new Vector3Int(-1, -1, 0), // ↙
                new Vector3Int(0, -1, 0),  // ↓
                new Vector3Int(1, -1, 0),  // ↘
            };

            float maxDot = -1f;
            Vector3Int best = Vector3Int.zero;

            foreach (var d in directions)
            {
                float dot = Vector3.Dot(input, ((Vector3)d).normalized);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    best = d;
                }
            }

            return best;
        }


        private bool IsObstacle(Vector3Int pos)
        {
            return obstacleTilemap.HasTile(pos);
        }

        private bool HasGround(Vector3Int pos)
        {
            return groundTilemap.HasTile(pos);
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

        public override string GetDescription(ICharacter self)
        {
            return hintText;
        }
    }
}
