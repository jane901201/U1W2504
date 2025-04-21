using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "StateChangeItem", menuName = "MyGame/StateChangeItem")]
    public class StateChangeItem : IItem
    {
        public override void Use(ICharacter self, ICharacter[] targets)
        {
            self.SwitchState();
        }

        protected override void UseAsOni(ICharacter self, ICharacter[] targets)
        {
            
        }

        protected override void UseAsHuman(ICharacter self, ICharacter[] targets)
        {
            
        }
    }
}