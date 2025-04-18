using TimerFrame;
using UnityEditor.UI;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Strawberry", menuName = "MyGame/Items")]
    
    // イチゴ
    public class Strawberry : IItem
    {
        private static float _stopDuration = 2f;      // 鬼役用
        private static float _invincibleDuration = 5f; // 人役用

        public override void Use(ICharacter self, ICharacter[] targets)
        {
            if (self.CharacterState.Role == CharacterState.RoleType.Oni)
            {
                イチゴジャム(targets);
            }
            else if (self.CharacterState.Role == CharacterState.RoleType.Human)
            {
                イチゴ邪夢(self);
            }
        }

        private void イチゴジャム(ICharacter[] targets)
        {
            foreach (var target in targets)
            {
                if (TimerManager.Instance.HasTask("イチゴジャム" + target.Id))
                {
                    // Reset or do nothing.
                    TimerManager.Instance.ResetTask("イチゴジャム" + target.Id);
                    continue;
                }
                else
                {
                    // Enemy freeze function
                    // target.Stop = true;
                    TimerManager.Instance.AddTask("イチゴジャム" + target.Id, _stopDuration, () =>
                    {
                        // Unfreeze
                        // target.Stop = false;
                    });
                }
            }
            
        }

        private void イチゴ邪夢(ICharacter self)
        {
            if (TimerManager.Instance.HasTask("イチゴ邪夢" + self.Id))
            {
                TimerManager.Instance.ResetTask("イチゴ邪夢" + self.Id);
            }
            else
            {
                // Player attack damage 0 function
                // int tempDamage = self.Damage;
                // self.Damage = 0;
                TimerManager.Instance.AddTask("イチゴ邪夢" + self.Id, _invincibleDuration, () =>
                {
                    // Restore
                    // self.Damage = tempDamage;
                });
            }
        }
    }
}