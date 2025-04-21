using System.Collections;
using PolyNav;
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
            StartCoroutine(targetCharacter.WaitAndSetFalse());
            StartCoroutine(MoveSpeedUp());
            return damage;
        }

        public override IEnumerator MoveSpeedUp()
        {

            MoveSpeed *= escapeSpeedMultiplier;
            PolyNavAgent.maxSpeed = MoveSpeed;
            yield return new WaitForSeconds(3f); // 3秒待つ
            MoveSpeed /= escapeSpeedMultiplier;
            PolyNavAgent.maxSpeed = MoveSpeed;
        }
        
        public override IEnumerator WaitAndSetFalse()
        {
            PolyNavAgent.maxSpeed = 0f;
            yield return new WaitForSeconds(3f); // 3秒待つ
            IsFrozen = false;
            PolyNavAgent.maxSpeed = MoveSpeed;
            Debug.Log("3秒経過、isReady = true");

        }
    }
}