using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace DefaultNamespace
{
    public class ICharacter : MonoBehaviour
    {
        [SerializeField] private int hp = 3;
        
        [SerializeField] private CharacterState characterState;
        [SerializeField] private Animator animator;
        public float moveSpeed = 5f;
        public float escapeSpeedMultiplier = 2f;
        // ID could be any string, for item timer task
        [SerializeField] private string id;
        [SerializeField] protected SpriteRenderer effectIcon; 
        
        
        protected float effectShowTime = 0;
        
        public Action StateChangEvent;
        public Action<Sprite, float> ItemEffectEvent;
        
        protected bool isFrozen = false;
        public bool IsForzen { get => isFrozen; set => isFrozen = value; }
        
        

        public int Hp { get => hp; set => hp = value; }
        public string Id { get => id; set => id = value; }
        public CharacterState CharacterState { get => characterState; set => characterState = value; }
        public Animator Animator{ get => animator; set => animator = value; }
        public SpriteRenderer EffectIcon { get => effectIcon; set => effectIcon = value; } 
        
        protected virtual void Awake()
        {
            characterState = new CharacterState();
        }

        public virtual void TakeDamage(ICharacter attackedCharacter)
        {
            attackedCharacter.Hp -= 1;
        }
        
        public virtual IEnumerator WaitAndSetFalse()
        {
            yield return new WaitForSeconds(3f); // 3秒待つ
            isFrozen = false;
            Debug.Log("3秒経過、isReady = true");
        }

        public virtual IEnumerator MoveSpeedUp()
        {
            moveSpeed *= escapeSpeedMultiplier;
            yield return new WaitForSeconds(3f); // 3秒待つ
            moveSpeed /= escapeSpeedMultiplier;
        }
        
        public IEnumerator ShowEffect(Sprite sprite)
        {
            effectIcon.GameObject().SetActive(true);
            effectIcon.sprite = sprite;
            yield return new WaitForSeconds(effectShowTime); 
            effectIcon.GameObject().SetActive(false);
        }


    }
}