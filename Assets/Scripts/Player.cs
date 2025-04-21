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
        if (PreventRoleChange)
        {
            return;
        }
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
            LastMoveDirection = input.normalized;

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

    public override int Attack(ICharacter targetCharacter)
    {
        int damage = base.Attack(targetCharacter);
        StateChangEvent?.Invoke();
        StartCoroutine(targetCharacter.WaitAndSetFalse());
        StartCoroutine(MoveSpeedUp());
        return damage;
    }
    

    public override bool AddItem(IItem item)
    {
        if (!base.AddItem(item))
        {
            return false;
        }   
        // 显示图标
        GameUI.Instance?.SetPlayerItemIcon(item.Icon, item.GetDescription(this));
        
        return true;
    }

    public override void UseItem()
    {
        base.UseItem();
        ItemEffectEvent?.Invoke(null, false);
        GameUI.Instance?.ClearPlayerItemIcon(); // 清除图标

    }

}