using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TimerFrame;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : ICharacter
{
    private Vector3Int playerPos;

    [Header("角色随机变鬼人时间配置")] 
    [SerializeField] private float minSwitchTime = 2f;
    [SerializeField] private float maxSwitchTime = 10f;
    private const string SwitchTaskId = "PlayerRandomSwitchRole";

    public Action PlayerStateChangEvent;
    
    private float moveCooldown = 0.2f; 
    private float lastMoveTime = 0f;
    private Rigidbody2D rb;
    private Vector2 movement;
    
    // CD
    private Vector2 lastInputDir = Vector2.zero;
    public Vector2 GetLastInputDirection() => lastInputDir;
    
    // IceCream
    // TODO: Complete logic of catching
    // void TryCatch(Player target)
    // {
    //     int damage = 1;
    //     if (this.HasIceAttackBuff)
    //         damage += 1;
    //     target.Hp -= damage;
    // }
    public bool HasIceAttackBuff { get; set; } = false;
    public bool HasIceEscapeMission { get; set; } = false;
    
    // Syringe
    // TODO: Catch logic
    // if (player.SyringeHealTaskId != null)
    // {
    //     TimerManager.Instance.CancelTask(player.SyringeHealTaskId);
    //     player.SyringeHealTaskId = null;
    // }
    public bool PreventRoleChange { get; set; } = false;
    public string SyringeHealTaskId { get; set; }
    
    private void ScheduleNextRoleSwitch()
    {
        if (TimerManager.Instance.HasTask(SwitchTaskId))
            TimerManager.Instance.RemoveTask(SwitchTaskId);

        float nextTime = Random.Range(minSwitchTime, maxSwitchTime);
        TimerManager.Instance.AddTask(SwitchTaskId, nextTime, () =>
        {
            SwitchEmotion();              // 切换身份
            ScheduleNextRoleSwitch();  // 再次安排下一轮
        });
    }
    
    public void SwitchEmotion()
    {
        CharacterState.Emotion = CharacterState.Emotion == CharacterState.EmotionType.Love ? CharacterState.EmotionType.Sad : CharacterState.EmotionType.Love;
        PlayerStateChangEvent?.Invoke();
    }


    
    private void Start()
    {
        Id = name;
        rb = GetComponent<Rigidbody2D>();
        playerPos = new Vector3Int(0, 0, 0);
        ScheduleNextRoleSwitch();
    }
    

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        Vector2 input = new Vector2(h, v);

        // CD
        if (input != Vector2.zero)
            lastInputDir = input.normalized;

        // Mirror
        if (IsControlReversed)
            input *= -1;

        movement = input.normalized;
    }

    private void FixedUpdate()
    {
        if (IsFrozen) return;
        rb.MovePosition(rb.position + movement * MoveSpeed * Time.fixedDeltaTime);
    }

    public override void TakeDamage(ICharacter attackedCharacter)
    {
        if (IsInvincible)
        {
            Debug.Log("无敌");
            return;
        }
        base.TakeDamage(attackedCharacter);
        StateChangEvent?.Invoke();
        StartCoroutine(attackedCharacter.WaitAndSetFalse());
        StartCoroutine(MoveSpeedUp());
    }
    

    public override bool AddItem(IItem item)
    {
        if (!base.AddItem(item))
        {
            return false;
        }   
        // 显示图标
        GameUI.Instance?.SetPlayerItemIcon(item.Icon);
        
        return true;
    }

    public override void UseItem()
    {
        base.UseItem();
        GameUI.Instance?.ClearPlayerItemIcon(); // 清除图标

    }

}