using TimerFrame;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Mirror", menuName = "MyGame/Items")]
    
    // あいの鏡
    public class Mirror : IItem
    {
        
        [SerializeField] private float reverseDuration = 5f;
        [SerializeField] private string hintMessage = "操作が5秒間反転する！";

        protected override void UseAsOni(ICharacter self, ICharacter[] targets)
        {
            あいの鏡(self);
        }

        protected override void UseAsHuman(ICharacter self, ICharacter[] targets)
        {
            あいの鏡(self);
        }

        private void あいの鏡(ICharacter self)
        {
            Player player = (Player) self;

            string taskId = "MirrorReverse_" + self.Id;

            if (TimerManager.Instance.HasTask(taskId))
            {
                TimerManager.Instance.ResetTask(taskId);
            }
            else
            {
                player.IsControlReversed = true;
                TimerManager.Instance.AddTask(taskId, reverseDuration, () =>
                {
                    player.IsControlReversed = false;
                });
            }
        }
    }
}