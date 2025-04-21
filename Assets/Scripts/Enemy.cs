using System;
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

        private float _lastMaxSpeed;
        
        protected override void Awake()
        {
            base.Awake();
            Id = name;
            aiController = GetComponent<AIController>();
            PolyNavAgent = GetComponent<PolyNavAgent>();
            _lastMaxSpeed = PolyNavAgent.maxSpeed;
        }

        private void Update()
        {
            if (IsFrozen)
            {
                if (PolyNavAgent.maxSpeed != 0)
                {
                    _lastMaxSpeed = PolyNavAgent.maxSpeed;
                }
                PolyNavAgent.maxSpeed = 0;
            }
            else
            {
                PolyNavAgent.maxSpeed = _lastMaxSpeed;
            }

            if (IsControlReversed && !TimerManager.Instance.HasTask($"{name}_AIReversed"))
            {
                PolyNavAgent.reverse = true;
                TimerManager.Instance.AddTask($"{name}_AIReversed", 1f, () =>
                {
                    IsControlReversed = false;
                    PolyNavAgent.reverse = false;
                });
            }

            // CD
            LastMoveDirection = PolyNavAgent.movingDirection.normalized;
        }


        public override int Attack(ICharacter targetCharacter)
        {
            
            int damage = base.Attack(targetCharacter);
            MoveSpeedUp();
            return damage;
        }

        public override void MoveSpeedUp()
        {
            MoveSpeed *= escapeSpeedMultiplier;
            PolyNavAgent.maxSpeed = MoveSpeed;
            TimerManager.Instance.AddTask($"{this.name}_MoveSpeedUp_3s", 3f, () =>
            {
                MoveSpeed = OriginalMoveSpeed;
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