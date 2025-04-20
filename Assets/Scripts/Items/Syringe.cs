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
            var player = (Player)self;

            string taskId = "Syringe_Heal_" + self.Id;

            if (TimerManager.Instance.HasTask(taskId))
                TimerManager.Instance.ResetTask(taskId);
            else
            {
                // 30秒后 +1 HP
                TimerManager.Instance.AddTask(taskId, duration, () =>
                {
                    player.Hp += 1;
                });

                // 标记用于被抓时取消
                player.SyringeHealTaskId = taskId;
            }
        }
        
        public override float GetDuration(ICharacter self)
        {
            if(self.CharacterState.Role == CharacterState.RoleType.Oni)
                return duration;
            else
                return duration;
        }
    }
}