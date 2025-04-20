using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "StateChangeItem", menuName = "MyGame/StateChangeItem")]
    public class StateChangeItem : IItem
    {
        public override void Use(ICharacter self, ICharacter[] targets)
        {
            Player player = (Player) self;
            player.PlayerStateChangEvent?.Invoke();
            player.CharacterState.Emotion = player.CharacterState.Emotion == CharacterState.EmotionType.Love ? CharacterState.EmotionType.Sad : CharacterState.EmotionType.Love;
        }

        protected override void UseAsOni(ICharacter self, ICharacter[] targets)
        {
            
        }

        protected override void UseAsHuman(ICharacter self, ICharacter[] targets)
        {
            
        }
    }
}