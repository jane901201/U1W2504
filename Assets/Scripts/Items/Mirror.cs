using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Mirror", menuName = "MyGame/Items")]
    
    // あいの鏡
    public class Mirror : IItem
    {
        public override void Use(ICharacter self, ICharacter[] target)
        {
            あいの鏡(self);
        }

        private void あいの鏡(ICharacter self)
        {
            
        }
    }
}