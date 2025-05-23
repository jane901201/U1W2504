using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using NUnit.Framework.Constraints;
using PolyNav;
using UnityEngine;
using U1w.FSM;
using Unity.VisualScripting;
using UnityEditor;

public class AIController : MonoBehaviour
{
    
    [HideInInspector] public EnemyStateMachine LoveStateMachine;
    [HideInInspector] public EnemyStateMachine HateStateMachine;
    [HideInInspector] public EnemyStateMachine CurrentStateMachine;
    [HideInInspector] public GameSystem GameSystem;
    private GameState oldGameState;
    public EnemyStateEnum CurrentState;

    private void Start()
    {
        GameSystem=GameSystem.Instance;

        //Love
        LoveStateMachine = new(this);
        LoveStateMachine.AddState((int)EnemyStateEnum.Idle, new EnemyState_Idle(LoveStateMachine));
        LoveStateMachine.AddState((int)EnemyStateEnum.Follow, new EnemyState_Follow(LoveStateMachine));
        LoveStateMachine.AddState((int)EnemyStateEnum.Item, new EnemyState_Item(LoveStateMachine));

        LoveStateMachine.Begin((int)CurrentState);
        //Hate
        HateStateMachine = new(this);
        HateStateMachine.AddState((int)EnemyStateEnum.Idle, new EnemyState_Idle(HateStateMachine));
        HateStateMachine.AddState((int)EnemyStateEnum.RunAway, new EnemyState_RunAway(HateStateMachine));
        HateStateMachine.Begin((int)CurrentState);
        
        CurrentStateMachine=LoveStateMachine;
    }
    void Update() {

        if (GameSystem.GameState != null)
        {
            if (GameSystem.GameState != oldGameState)
            {
                oldGameState = GameSystem.GameState;
                ChangeAIState(GameSystem.GameState);
            }
        }
        CurrentStateMachine.Update();
        //Hate
        //RunAway
        if (HateStateMachine.CurEid != (int)EnemyStateEnum.RunAway)
        {
            if (IsNeedToRunAway()) 
                HateStateMachine.SwitchState((int)EnemyStateEnum.RunAway);
        }
        if (HateStateMachine.CurEid != (int)EnemyStateEnum.Item)
        {
            if (IsNeedToGoToItem()) 
                LoveStateMachine.SwitchState((int)EnemyStateEnum.Item);
        }
        //Love
        if (HateStateMachine.CurEid != (int)EnemyStateEnum.Item)
        {
            if (IsNeedToGoToItemWhenRunAway()) 
                HateStateMachine.SwitchState((int)EnemyStateEnum.Item);
        }
        

    }
    public void ChangeAIState(GameState newGameState)
    {

        if (newGameState==GameState.EnemyChasePlayer)
        {
            CurrentStateMachine = LoveStateMachine;
        }
        else
        {
            CurrentStateMachine = HateStateMachine;
        }
        CurrentStateMachine.SwitchState((int)EnemyStateEnum.Idle);
        AduioController._Instance.PlayBGM();
    }
    
    public void FixedUpdate()
    {
        CurrentStateMachine.FixedUpdate();
    }
    
    
    //Map
    private PolyNavAgent _agent;
    public PolyNavAgent agent {
        get { return _agent != null ? _agent : _agent = GetComponent<PolyNavAgent>(); }
    }
    //Follow
    public Transform target;
    //Item
    public ItemTrigger Item;
    [Header("Get Item Parameter When Follow")] public float DisttoPlayer=3f;
    public float DistToItem = 3f;
    private bool IsNeedToGoToItem()
    {
        try
        {
            if (Vector2.Distance(this.transform.position, target.position) > DisttoPlayer &&
                Vector2.Distance(this.transform.position, Item.GetComponent<Transform>().position) < DistToItem)
            {
                return true;
            }
        }
        catch
        {
            return false;
        }

        return false;
    }
    //Run Away
    public Escape escape;
    [Header("Run Away Parameter")]
    public float runAwayFromPlayer = 3f;
    private bool IsNeedToRunAway()
    {
        if (Vector2.Distance(this.transform.position, target.position) < runAwayFromPlayer)
        {
            return true;
        }

        return false;
    }
    [Header("Get Item Parameter When Follow")] public float AwayFormPlayer=4f;
    private bool IsNeedToGoToItemWhenRunAway()
    {
        if (Vector2.Distance(this.transform.position, target.position) > AwayFormPlayer)
        {
            return true;
        }

        return false;
    }

}
public enum EnemyStateEnum
{
    Idle,
    Follow,
    RunAway,
    Item
}