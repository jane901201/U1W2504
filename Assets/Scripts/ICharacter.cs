using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class ICharacter : MonoBehaviour
    {
        [SerializeField] private int hp = 3;
        
        [SerializeField] private CharacterState characterState;
        [SerializeField] private Animator animator;
        public float moveSpeed = 5f;
        // ID could be any string, for item timer task
        [SerializeField] private string id;
        
        public Action StateChangEvent;
        
        protected bool isFrozen = false;
        public bool IsForzen { get => isFrozen; set => isFrozen = value; }
        

        public int Hp { get => hp; set => hp = value; }
        public string Id { get => id; set => id = value; }
        public CharacterState CharacterState { get => characterState; set => characterState = value; }
        public Animator Animator{ get => animator; set => animator = value; }
        
        protected virtual void Awake()
        {
            characterState = new CharacterState();
        }

        public virtual void TakeDamage(ICharacter attackedCharacter)
        {
            attackedCharacter.Hp -= 1;
        }
        
        public IEnumerator WaitAndSetFalse()
        {
            yield return new WaitForSeconds(3f); // 3秒待つ
            isFrozen = false;
            Debug.Log("3秒経過、isReady = true");
        }

    }
}