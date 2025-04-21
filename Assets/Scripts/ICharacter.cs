using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DefaultNamespace
{
    public class ICharacter : MonoBehaviour
    {
        [SerializeField] private List<IItem> items = new List<IItem>();
        [SerializeField] private int hp = 3;

        [SerializeField] private Animator animator;
        [SerializeField] protected Sprite moveStopSprite;
        public float MoveSpeed { get; set; } = 5f;
                
        
        protected float effectShowTime = 0;
        
        public Action StateChangEvent;
        public Action<Sprite, bool> ItemEffectEvent;

        public float escapeSpeedMultiplier = 2f;
        [SerializeField] protected SpriteRenderer effectIcon; 
        
        // CD
        public Vector2 LastMoveDirection { get; set; } = Vector2.zero;

        // Mirror
        public bool IsControlReversed { get; set; } = false;
        
        // Strawberry
        public bool IsFrozen { get; set; }
        public bool IsInvincible { get; set; } = false;
        
        // IceCream
        public bool HasIceAttackBuff { get; set; } = false;
        public bool HasIceEscapeMission { get; set; } = false;
    
        // Syringe
        public bool PreventRoleChange { get; set; } = false;
        public bool HasSyringeHealTask { get; set; }




        public int Hp { get => hp; set => hp = value; }
        public string Id { get; set; }
        public CharacterState CharacterState { get; set; }
        public Animator Animator{ get => animator; set => animator = value; }
        public SpriteRenderer EffectIcon { get => effectIcon; set => effectIcon = value; } 
        
        protected virtual void Awake()
        {
            if (CharacterState == null)
            {
                CharacterState = new CharacterState();
            }
            else
            {
                return;
            }
            
        }

        public virtual int Attack(ICharacter targetCharacter)
        {
            if (targetCharacter.IsInvincible)
            {
                return 0;
            }
            targetCharacter.HasIceEscapeMission = false;
            targetCharacter.HasSyringeHealTask = false;
            int damage = 1;
            if (HasIceAttackBuff)
            {
                damage += 1;
            }
            targetCharacter.Hp -= damage;
            return damage;
        }
        
        public virtual IEnumerator WaitAndSetFalse()
        {
            effectIcon.gameObject.SetActive(true);
            effectIcon.sprite = moveStopSprite;
            yield return new WaitForSeconds(3f);
            effectIcon.gameObject.SetActive(false);// 3秒待つ
            IsFrozen = false;
            Debug.Log("3秒経過、isReady = true");
        }

        public virtual IEnumerator MoveSpeedUp()
        {
            MoveSpeed *= escapeSpeedMultiplier;
            yield return new WaitForSeconds(3f); // 3秒待つ
            MoveSpeed /= escapeSpeedMultiplier;
        }
        
        public IEnumerator ShowEffect(Sprite sprite)
        {
            effectIcon.GameObject().SetActive(true);
            effectIcon.sprite = sprite;
            yield return new WaitForSeconds(effectShowTime); 
            effectIcon.GameObject().SetActive(false);
        }

        public virtual bool AddItem(IItem item)
        {
            if (items.Count == 0)
            {
                items.Add(item);
                ItemEffectEvent?.Invoke(item.GetEffectIcon(this), true);
            }
            else
            {
                return false;
            }
        
            if (item is IMapTileItem mapItem)
            {
                mapItem.SetTilemaps(GameSystem.Instance.GetTilemap(), GameSystem.Instance.GetObstacleTilemap());
            }

            return true;
        }

        public virtual void UseItem()
        {
            if(items.Count == 0) return;
            var item = items[0];
        
            if (item is IMapTileItem mapItem)
            {
                mapItem.SetTilemaps(GameSystem.Instance.GetTilemap(), GameSystem.Instance.GetObstacleTilemap());
            }
        
            Debug.Log(item.name);
        
            item.Use(this, FindTargets());
            if (item.IsDurationItem)
            {
                effectShowTime = item.GetDuration(this);
                StartCoroutine(ShowEffect(item.GetEffectIcon(this)));
            }
        
            items.RemoveAt(0);
        }
        
        public ICharacter[] FindTargets()
        {
            return new ICharacter[] { GameSystem.Instance.GetEnemy(this) };
        }


    }
}