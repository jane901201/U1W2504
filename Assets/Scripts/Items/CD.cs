using UnityEngine;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "CD", menuName = "MyGame/Items/CD")]
    public class CD : IItem
    {

        [Header("提示文本")]
        [SerializeField] private string hintText = "障害物を一つ乗り越えた！";

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
            Player player = (Player) self;
            Vector3 direction = player.GetLastInputDirection();
            if (direction == Vector3.zero) return;

            Vector3Int currentCell = groundTilemap.WorldToCell(player.transform.position);
            Vector3Int firstCheck = currentCell + Vector3Int.RoundToInt(direction);
            Vector3Int landingCell = currentCell + Vector3Int.RoundToInt(direction) * 2;
            
            if (IsObstacle(firstCheck) && !IsObstacle(landingCell) && HasGround(landingCell))
            {
                player.transform.position = groundTilemap.GetCellCenterWorld(landingCell);
            }
        }

        private bool IsObstacle(Vector3Int pos)
        {
            return obstacleTilemap.GetTile(pos) != null;
        }

        private bool HasGround(Vector3Int pos)
        {
            return groundTilemap.GetTile(pos) != null;
        }
    }
}