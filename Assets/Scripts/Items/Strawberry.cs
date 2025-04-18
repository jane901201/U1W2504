using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Strawberry", menuName = "MyGame/Items")]
    
    // イチゴ
    public class Strawberry : IItem
    {
        public float stopDuration = 2f;      // 鬼役用
        public float invincibleDuration = 5f; // 人役用

        public override void Use(Player player)
        {
            if (player.CharacterState.Role == CharacterState.RoleType.Oni)
            {
                イチゴジャム();
            }
            else if (player.CharacterState.Role == CharacterState.RoleType.Human)
            {
                イチゴ邪夢();
            }
        }

        private void イチゴジャム()
        {
            
        }

        private void イチゴ邪夢()
        {
            
        }
    }
}