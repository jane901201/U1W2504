using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Mirror", menuName = "MyGame/Items")]
    
    // あいの鏡
    public class Mirror : IItem
    {
        public float stopDuration = 2f;      // 鬼役用
        public float invincibleDuration = 5f; // 人役用

        public override void Use(Player player)
        {
            あいの鏡(player);
        }

        private void あいの鏡(Player player)
        {
            
        }
    }
}