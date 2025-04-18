using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "StateChangeItem", menuName = "MyGame/StateChangeItem")]
    public class StateChangeItem : IItem
    {
        public override void Use(Player player)
        {
            player.PlayerStateChangEvent?.Invoke();
            //player.CharacterState = player.CharacterState == CharacterState.Love ? CharacterState.Sad : CharacterState.Love;
        }

        public override void Use(ICharacter character)
        {
            
        }
    }
}