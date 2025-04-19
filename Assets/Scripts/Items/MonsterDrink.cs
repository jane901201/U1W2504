using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "MonsterDrink", menuName = "MyGame/Items/MonsterDrink")]
    public class MonsterDrink : IItem
    {
        [SerializeField] private string hintText = "役割を入れ替えた！";

        protected override void UseAsOni(ICharacter self, ICharacter[] targets)
        {
            SwitchRoles(self, targets);
        }

        protected override void UseAsHuman(ICharacter self, ICharacter[] targets)
        {
            SwitchRoles(self, targets);
        }

        private void SwitchRoles(ICharacter self, ICharacter[] targets)
        {
            if (targets == null || targets.Length == 0) return;

            ICharacter target = targets[0]; // 只作用于第一个目标
            var roleSelf = self.CharacterState.Role;
            var roleTarget = target.CharacterState.Role;

            // 自己变为相反角色
            self.CharacterState.Role = roleSelf == CharacterState.RoleType.Oni
                ? CharacterState.RoleType.Human
                : CharacterState.RoleType.Oni;

            // 对方也变为相反角色
            target.CharacterState.Role = roleTarget == CharacterState.RoleType.Oni
                ? CharacterState.RoleType.Human
                : CharacterState.RoleType.Oni;
        }
    }
}