using UnityEngine;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "CD", menuName = "MyGame/Items/CD")]
    public class CD : IItem, IMapTileItem
    {
        [Header("提示文本")]
        [SerializeField] private string hintText = "障害物を一つ乗り越えた！";

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
            Player player = (Player)self;
            Vector3 direction = player.GetLastInputDirection();
            Vector3Int currentCell = groundTilemap.WorldToCell(player.transform.position);

            Vector3Int dir = GetCardinalDirection(direction);
            bool jumped = TryLeap(currentCell, dir, player);

            if (!jumped)
            {
                // 如果跳跃失败，尝试四方向
                Vector3Int[] directions = {
                    new Vector3Int(1, 0, 0),  // 右
                    new Vector3Int(-1, 0, 0), // 左
                    new Vector3Int(0, -1, 0),  // 上
                    new Vector3Int(0, 1, 0)  // 下
                };

                foreach (var fallbackDir in directions)
                {
                    if (TryLeap(currentCell, fallbackDir, player))
                    {
                        break;
                    }
                }
            }
        }

        // 方向单位向量（上下左右）
        private Vector3Int GetCardinalDirection(Vector3 input)
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                return new Vector3Int((int)Mathf.Sign(input.x), 0, 0);
            }
            else
            {
                return new Vector3Int(0, -(int)Mathf.Sign(input.y), 0);
            }
        }


        private bool TryLeap(Vector3Int current, Vector3Int dir, Player player)
        {
            Vector3Int first = current + dir;
            Vector3Int landing = current + dir * 2;

            if (IsObstacle(first) && !IsObstacle(landing) && HasGround(landing))
            {
                player.transform.position = groundTilemap.GetCellCenterWorld(landing);
                return true;
            }

            return false;
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
    }
}
