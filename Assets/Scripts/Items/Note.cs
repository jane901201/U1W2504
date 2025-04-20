using UnityEngine;
using TimerFrame;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Note", menuName = "MyGame/Items/Note")]
    public class Note : IItem
    {
        [Header("各种参数")]
        [SerializeField] private float duration = 5f;
        [SerializeField] private float speedUpFactor = 1.5f;
        [SerializeField] private float slowDownFactor = 0.75f;

        [Header("提示文本")]
        [SerializeField] private string oniHint = "5秒間加速した！";
        [SerializeField] private string humanHint = "相手を5秒間減速させた！";

        protected override void UseAsOni(ICharacter self, ICharacter[] targets)
        {
            var player = (Player)self;

            string taskId = "Note_SpeedUp_" + self.Id;

            if (TimerManager.Instance.HasTask(taskId))
            {
                TimerManager.Instance.ResetTask(taskId);
            }
            else
            {
                float originalSpeed = player.moveSpeed;
                player.moveSpeed *= speedUpFactor;

                TimerManager.Instance.AddTask(taskId, duration, () =>
                {
                    player.moveSpeed = originalSpeed;
                });
            }
        }

        protected override void UseAsHuman(ICharacter self, ICharacter[] targets)
        {
            if (targets == null) return;

            foreach (var target in targets)
            {
                // TODO: Replace to Enemy
                // var enemy = (Enemy) target;
                var enemy = (Player) target;

                string taskId = "Note_SlowDown_" + enemy.Id;

                if (TimerManager.Instance.HasTask(taskId))
                {
                    TimerManager.Instance.ResetTask(taskId);
                }
                else
                {
                    float originalSpeed = enemy.moveSpeed;
                    enemy.moveSpeed *= slowDownFactor;

                    TimerManager.Instance.AddTask(taskId, duration, () =>
                    {
                        enemy.moveSpeed = originalSpeed;
                    });
                }
            }
        }
    }
}
