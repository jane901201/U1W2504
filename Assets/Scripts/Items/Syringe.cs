using UnityEngine;
using TimerFrame;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Syringe", menuName = "MyGame/Items/Syringe")]
    public class Syringe : IItem
    {
        [SerializeField] private float duration = 30f;
        [SerializeField] private string oniHint = "30秒間、鬼役を維持！";
        [SerializeField] private string humanHint = "30秒間、無事ならHP+1！";
        
        private bool isDurationItem => true;

        protected override void UseAsOni(ICharacter self, ICharacter[] targets)
        {
            var player = (Player)self;

            string taskId = "Syringe_PreventRole_" + self.Id;
            player.PreventRoleChange = true;

            TimerManager.Instance.AddTask(taskId, duration, () =>
            {
                player.PreventRoleChange = false;
            });
        }

        protected override void UseAsHuman(ICharacter self, ICharacter[] targets)
        {

            string taskId = "Syringe_Heal_" + self.Id;

            if (TimerManager.Instance.HasTask(taskId))
                TimerManager.Instance.ResetTask(taskId);
            else
            {
                self.HasSyringeHealTask = true;
                // 30秒后 +1 HP
                TimerManager.Instance.AddTask(taskId, duration, () =>
                {
                    if (self.HasSyringeHealTask)
                    {
                        self.Hp += 1;
                    }
                    self.HasSyringeHealTask = false;
                });
            }
        }
        
        public override float GetDuration(ICharacter self)
        {
            return duration;
        }
    }
}