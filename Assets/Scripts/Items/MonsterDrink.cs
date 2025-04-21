using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "MonsterDrink", menuName = "MyGame/Items/MonsterDrink")]
    public class MonsterDrink : IItem
    {
        [SerializeField] private string hintText = "役割を入れ替えた！";

        protected override void UseAsOni(ICharacter self, ICharacter[] targets)
        {
            SwitchRoles(self, targets);
        }

        protected override void UseAsHuman(ICharacter self, ICharacter[] targets)
        {
            SwitchRoles(self, targets);
        }

        private void SwitchRoles(ICharacter self, ICharacter[] targets)
        {
            ICharacter target = targets[0];

            // 自己变为相反角色
            ((Player)self).SwitchEmotion();
            ((Player)self).PlayerStateChangEvent?.Invoke();

            // 对方也变为相反角色
            target.CharacterState.Emotion = target.CharacterState.Emotion == CharacterState.EmotionType.Love
                ? CharacterState.EmotionType.Sad
                : CharacterState.EmotionType.Love;
            target.StateChangEvent?.Invoke();
        }
    }
}