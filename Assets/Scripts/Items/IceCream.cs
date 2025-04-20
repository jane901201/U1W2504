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

        protected override void UseAsOni(ICharacter self, ICharacter[] targets)
        {
            var player = (Player) self;

            player.HasIceAttackBuff = true;
            string taskId = "IceBuff_Oni_" + self.Id;

            TimerManager.Instance.AddTask(taskId, duration, () =>
            {
                player.HasIceAttackBuff = false;
            });
        }

        protected override void UseAsHuman(ICharacter self, ICharacter[] targets)
        {
            var player = (Player)self;

            string taskId = "IceBuff_Human_" + self.Id;
            player.HasIceEscapeMission = true;

            TimerManager.Instance.AddTask(taskId, duration, () =>
            {
                player.HasIceEscapeMission = false;

                player.Hp -= 1;
            });
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