using System;
using System.Collections.Generic;
using DefaultNamespace;
using TimerFrame;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : ICharacter
{
    private Vector3Int playerPos;
    [SerializeField] private List<IItem> items = new List<IItem>();

    [Header("角色随机变鬼人时间配置")] 
    [SerializeField] private float minSwitchTime = 2f;
    [SerializeField] private float maxSwitchTime = 10f;
    private const string SwitchTaskId = "PlayerRandomSwitchRole";

    public Action PlayerStateChangEvent;
    
    private float moveCooldown = 0.2f; 
    private float lastMoveTime = 0f;
    private Rigidbody2D rb;
    private Vector2 movement;
    
    // Strawberry
    public bool IsInvincible { get; set; } = false;
    
    // Mirror
    public bool IsControlReversed { get; set; } = false;
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



    public void SetFrozen(bool frozen)
    {
        isFrozen = frozen;
    }
    
    private void ScheduleNextRoleSwitch()
    {
        if (TimerManager.Instance.HasTask(SwitchTaskId))
            TimerManager.Instance.RemoveTask(SwitchTaskId);

        float nextTime = Random.Range(minSwitchTime, maxSwitchTime);
        TimerManager.Instance.AddTask(SwitchTaskId, nextTime, () =>
        {
            SwitchRole();              // 切换身份
            ScheduleNextRoleSwitch();  // 再次安排下一轮
        });
    }
    
    private void SwitchRole()
    {
        if (CharacterState.Role == CharacterState.RoleType.Human)
            CharacterState.Role = CharacterState.RoleType.Oni;
        else
            CharacterState.Role = CharacterState.RoleType.Human;
        PlayerStateChangEvent?.Invoke();
    }


    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerPos = new Vector3Int(0, 0, 0);
        // TimerManager.Instance.AddTask("PlayerRandomSwitchRoleRandomTimer", );
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
        if (isFrozen) return;
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public override void TakeDamage(ICharacter attackedCharacter)
    {
        base.TakeDamage(attackedCharacter);
        StateChangEvent?.Invoke();
        //TODO:aiController.ChangeAIState(); 需要在這裡有一段讓 Enemy 等待玩家逃跑的時間
        StartCoroutine(MoveSpeedUp());
    }
    

    public void AddItem(IItem item)
    {
        if (items.Count < 2)
        {
            items.Add(item);
        }
        
        if (item is IMapTileItem mapItem)
        {
            mapItem.SetTilemaps(GameSystem.Instance.GetTilemap(), GameSystem.Instance.GetObstacleTilemap());
        }
        // 显示图标
        GameUI.Instance?.SetPlayerItemIcon(item.Icon);
    }

    public void UseItem()
    {
        if(items.Count == 0) return;
        var item = items[0];
        
        if (item is IMapTileItem mapItem)
        {
            mapItem.SetTilemaps(GameSystem.Instance.GetTilemap(), GameSystem.Instance.GetObstacleTilemap());
        }
        
        Debug.Log(item.name);
        
        item.Use(this, FindTargets());
        items.RemoveAt(0);

        GameUI.Instance?.ClearPlayerItemIcon(); // 清除图标

    } 
    
    // TODO: 锁敌功能
    private ICharacter[] FindTargets()
    {
        return new ICharacter[] {};
    }

}