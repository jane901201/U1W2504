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
        
        private bool isDurationItem => true;

        protected override void UseAsOni(ICharacter self, ICharacter[] targets)
        {

            string taskId = "Note_SpeedUp_" + self.Id;

            if (TimerManager.Instance.HasTask(taskId))
            {
                TimerManager.Instance.ResetTask(taskId);
            }
            else
            {
                float originalSpeed = self.MoveSpeed;
                self.MoveSpeed *= speedUpFactor;
                self.StateChangEvent?.Invoke();

                TimerManager.Instance.AddTask(taskId, duration, () =>
                {
                    self.MoveSpeed = originalSpeed;
                    self.StateChangEvent?.Invoke();
                });
            }
        }

        protected override void UseAsHuman(ICharacter self, ICharacter[] targets)
        {
            if (targets == null) return;

            foreach (var target in targets)
            {

                string taskId = "Note_SlowDown_" + target.Id;
                
                if (TimerManager.Instance.HasTask(taskId))
                {
                    TimerManager.Instance.ResetTask(taskId);
                }
                else
                {
                    float originalSpeed = target.MoveSpeed;
                    target.MoveSpeed *= slowDownFactor;
                    self.StateChangEvent?.Invoke();

                    TimerManager.Instance.AddTask(taskId, duration, () =>
                    {
                        target.MoveSpeed = originalSpeed;
                        self.StateChangEvent?.Invoke();
                    });
                }
            }
        }
        
        public override float GetDuration(ICharacter self)
        {
            return duration;
        }

    }
}
