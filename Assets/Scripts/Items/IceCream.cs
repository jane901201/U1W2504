using UnityEngine;
using TimerFrame;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "IceCream", menuName = "MyGame/Items/IceCream")]
    public class IceCream : IItem
    {
        [SerializeField] private float duration = 30f;
        [SerializeField] private string oniHint = "30秒間、ダメージ+1！";
        [SerializeField] private string humanHint = "30秒間、逃げ切れば鬼にダメージ！";
        
        private bool isDurationItem => true;

        protected override void UseAsOni(ICharacter self, ICharacter[] targets)
        {
            self.HasIceAttackBuff = true;
            string taskId = "IceBuff_Oni_" + self.Id;

            TimerManager.Instance.AddTask(taskId, duration, () =>
            {
                self.HasIceAttackBuff = false;
            });
        }

        protected override void UseAsHuman(ICharacter self, ICharacter[] targets)
        {

            string taskId = "IceBuff_Human_" + self.Id;
            self.HasIceEscapeMission = true;

            TimerManager.Instance.AddTask(taskId, duration, () =>
            {
                if (self.HasIceEscapeMission)
                {
                    self.HasIceEscapeMission = false;

                    foreach (var target in targets)
                    {
                        target.Hp -= 1;
                    }
                }
                
            });
        }
        
        public override string GetDescription(ICharacter self)
        {
            string hintText = self.CharacterState.Emotion == CharacterState.EmotionType.Love ? oniHint : humanHint;
            return hintText;
        }
        
        public override float GetDuration(ICharacter self)
        {
            return duration;
        }
    }
}