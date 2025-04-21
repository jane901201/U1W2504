using TimerFrame;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Strawberry", menuName = "MyGame/Items/Strawberry")]
    
    // イチゴ
    public class Strawberry : IItem
    {
        [Header("持续时间")]
        [SerializeField] private float _stopDuration = 2f;      // 鬼役用
        [SerializeField] private float _invincibleDuration = 5f; // 人役用
        
        [Header("提示文字")]
        [SerializeField] private string oniHintMessage = "2秒間足止め成功！";
        [SerializeField] private string humanHintMessage = "5秒間無敵になった！";
        
        private bool isDurationItem => true;

        
        protected override void UseAsOni(ICharacter self, ICharacter[] targets)
        {
            イチゴジャム(targets);
        }

        protected override void UseAsHuman(ICharacter self, ICharacter[] targets)
        {
            イチゴ邪夢(self);
        }

        private void イチゴジャム(ICharacter[] targets)
        {
            if (targets == null || targets.Length == 0)
            {
                return;
            }
            
            foreach (var target in targets)
            {
                string taskId = $"イチゴジャム_{target.Id}";
                if (TimerManager.Instance.HasTask(taskId))
                {
                    // Reset or do nothing.
                    TimerManager.Instance.ResetTask(taskId);
                    continue;
                }
                else
                {
                    // Enemy freeze function
                    // target.Stop = true;
                    target.IsFrozen = true;
                    TimerManager.Instance.AddTask(taskId, _stopDuration, () =>
                    {
                        // Unfreeze
                        // target.Stop = false;
                        target.IsFrozen = false;
                    });
                }
            }
            
        }

        private void イチゴ邪夢(ICharacter self)
        {
            string taskId = $"イチゴ邪夢_{self.Id}";
            if (TimerManager.Instance.HasTask(taskId))
            {
                TimerManager.Instance.ResetTask(taskId);
            }
            else
            {
                // Player attack damage 0 function
                // int tempDamage = self.Damage;
                // self.Damage = 0;
                self.IsInvincible = true;
                TimerManager.Instance.AddTask(taskId, _invincibleDuration, () =>
                {
                    // Restore
                    // self.Damage = tempDamage;
                    self.IsInvincible = false;
                });
            }
        }

        public override float GetDuration(ICharacter self)
        {
            if(self.CharacterState.Emotion == CharacterState.EmotionType.Sad)
                return _stopDuration;
            else
                return _invincibleDuration;
        }
        
        public override string GetDescription(ICharacter self)
        {
            string hintText = self.CharacterState.Emotion == CharacterState.EmotionType.Love ? oniHintMessage : humanHintMessage;
            return hintText;
        }
    }
}