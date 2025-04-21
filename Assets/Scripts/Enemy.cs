using System.Collections;
using PolyNav;
using TimerFrame;
using UnityEngine;

namespace DefaultNamespace
{
    public class Enemy : ICharacter
    {
        AIController aiController;

        public AIController AIController;

        public PolyNavAgent PolyNavAgent;

        
        // TODO: 每次移动修改移动方向LastMoveDirection
        
        protected override void Awake()
        {
            base.Awake();
            Id = name;
            aiController = GetComponent<AIController>();
            PolyNavAgent = GetComponent<PolyNavAgent>();
        }
        

        public override int Attack(ICharacter targetCharacter)
        {
            
            int damage = base.Attack(targetCharacter);
            StateChangEvent?.Invoke();
            targetCharacter.IsFrozen = true;
            targetCharacter.WaitAndSetFalse();
            MoveSpeedUp();
            return damage;
        }

        public override void MoveSpeedUp()
        {

            MoveSpeed *= escapeSpeedMultiplier;
            PolyNavAgent.maxSpeed = MoveSpeed;
            TimerManager.Instance.AddTask($"{this.name}_MoveSpeedUp_3s", 3f, () =>
            {
                MoveSpeed /= escapeSpeedMultiplier;
                PolyNavAgent.maxSpeed = MoveSpeed;
            });
        }
        
        public override void WaitAndSetFalse()
        {
            effectIcon.gameObject.SetActive(true);
            effectIcon.sprite = moveStopSprite;
            PolyNavAgent.maxSpeed = 0f;
            TimerManager.Instance.AddTask($"{this.name}_WaitAndSetFalse_3s", 3f, () =>
            {
                effectIcon.gameObject.SetActive(false);
                IsFrozen = false;
                PolyNavAgent.maxSpeed = MoveSpeed;
            });
        }
    }
}