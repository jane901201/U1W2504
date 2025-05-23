using System;
using System.Collections;
using System.Collections.Generic;
using TimerFrame;
using Unity.VisualScripting;
using UnityEngine;

namespace DefaultNamespace
{
    public class ICharacter : MonoBehaviour
    {
        [SerializeField] private List<IItem> items = new List<IItem>();
        [SerializeField] private int hp = 3;

        [SerializeField] private Animator animator;
        [SerializeField] private Animator effectAnimator;
        [SerializeField] protected Sprite moveStopSprite;
        [SerializeField] public float MoveSpeed = 5f;
        public float OriginalMoveSpeed;
        
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
        public Animator EffectAnimator{ get => effectAnimator; set => effectAnimator = value; }
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
            
            OriginalMoveSpeed = MoveSpeed;
            
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
            targetCharacter.IsFrozen = true;
            targetCharacter.WaitAndSetFalse();
            SwitchState();
            return damage;
        }
        
        public void SwitchState(string source="default")
        {
            if (PreventRoleChange)
            {
                return;
            }

            GameSystem.Instance.SwitchGameState(source);
        }
        
        public virtual void WaitAndSetFalse()
        {
            effectIcon.gameObject.SetActive(true);
            effectIcon.sprite = moveStopSprite;
            TimerManager.Instance.AddTask($"{this.name}_WaitAndSetFalse_3s", 3f, () =>
            {
                effectIcon.gameObject.SetActive(false); // 3秒待つ
                IsFrozen = false;
            });
        }

        public virtual void MoveSpeedUp()
        {
            MoveSpeed *= escapeSpeedMultiplier;
            TimerManager.Instance.AddTask($"{this.name}_MoveSpeedUp_3s", 3f, () =>
            {
                MoveSpeed = OriginalMoveSpeed;
            }); 
        }
        
        public void ShowEffect(Sprite sprite, float duration)
        {
            effectIcon.GameObject().SetActive(true);
            effectIcon.sprite = sprite;
            TimerManager.Instance.AddTask($"ShowEffect_{this.name}_{sprite.name}", duration, () =>
            {
                effectIcon.GameObject().SetActive(false);
            });
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
                ShowEffect(item.GetEffectIcon(this), item.GetDuration(this));
            }
        
            items.RemoveAt(0);
        }
        
        public ICharacter[] FindTargets()
        {
            return new ICharacter[] { GameSystem.Instance.GetEnemy(this) };
        }


    }
}