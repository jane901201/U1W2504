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

        protected override void Awake()
        {
            base.Awake();
            aiController = GetComponent<AIController>();
            PolyNavAgent = GetComponent<PolyNavAgent>();
        }
        

        public override void TakeDamage(ICharacter character)
        {
            if (IsInvincible)
            {
                return;
            }
            base.TakeDamage(character);
            StateChangEvent?.Invoke();
            character.IsFrozen = true;
            StartCoroutine(character.WaitAndSetFalse());
            StartCoroutine(MoveSpeedUp());
        }

        public override IEnumerator MoveSpeedUp()
        {
            moveSpeed *= escapeSpeedMultiplier;
            PolyNavAgent.maxSpeed = moveSpeed;
            yield return new WaitForSeconds(3f); // 3秒待つ
            moveSpeed /= escapeSpeedMultiplier;
            PolyNavAgent.maxSpeed = moveSpeed;
        }
        
        public override IEnumerator WaitAndSetFalse()
        {
            PolyNavAgent.maxSpeed = 0f;
            yield return new WaitForSeconds(3f); // 3秒待つ
            IsFrozen = false;
            PolyNavAgent.maxSpeed = moveSpeed;
            Debug.Log("3秒経過、isReady = true");

        }
    }
}