using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "StateChangeItem", menuName = "MyGame/StateChangeItem")]
    public class StateChangeItem : IItem
    {
        public override void Use(ICharacter self, ICharacter[] targets)
        {
            //self.PlayerStateChangEvent?.Invoke();
            //player.CharacterState = player.CharacterState == CharacterState.Love ? CharacterState.Sad : CharacterState.Love;
        }

        protected override void UseAsOni(ICharacter self, ICharacter[] targets)
        {
            
        }

        protected override void UseAsHuman(ICharacter self, ICharacter[] targets)
        {
            
        }
    }
}