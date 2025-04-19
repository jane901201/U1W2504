using TimerFrame;
using UnityEditor.UI;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Strawberry", menuName = "MyGame/Items")]
    
    // イチゴ
    public class Strawberry : IItem
    {
        [Header("持续时间")]
        private static float _stopDuration = 2f;      // 鬼役用
        private static float _invincibleDuration = 5f; // 人役用
        
        [Header("提示文字（UI表示用）")]
        [SerializeField] private string oniHintMessage = "2秒間足止め成功！";
        [SerializeField] private string humanHintMessage = "5秒間無敵になった！";

        
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
                    ((Player) target).SetFrozen(true);
                    TimerManager.Instance.AddTask(taskId, _stopDuration, () =>
                    {
                        // Unfreeze
                        // target.Stop = false;
                        ((Player) target).SetFrozen(false);
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
                ((Player) self).IsInvincible = true;
                TimerManager.Instance.AddTask(taskId, _invincibleDuration, () =>
                {
                    // Restore
                    // self.Damage = tempDamage;
                    ((Player) self).IsInvincible = false;
                });
            }
        }
    }
}