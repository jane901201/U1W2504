using System;
using System.Collections;
using System.Collections.Generic;
using PolyNav;
using UnityEngine;
using U1w.FSM;
using Unity.VisualScripting;
using UnityEditor;

public class HateAIController : MonoBehaviour
{
    public EnemyStateMachine LoveStateMachine;
    public EnemyStateMachine HateStateMachine;
    public EnemyStateMachine CurrentStateMachine;
    
    public EnemyStateEnum CurrentState;

    private void Start()
    {
        CurrentStateMachine=LoveStateMachine;
        //Love
        LoveStateMachine = new(this);
        LoveStateMachine.AddState((int)EnemyStateEnum.Idle, new EnemyState_Idle(LoveStateMachine));
        LoveStateMachine.AddState((int)EnemyStateEnum.Follow, new EnemyState_Follow(LoveStateMachine));
        LoveStateMachine.AddState((int)EnemyStateEnum.Item, new EnemyState_Item(LoveStateMachine));

        LoveStateMachine.Begin((int)CurrentState);
        //Hate
        HateStateMachine.AddState((int)EnemyStateEnum.Idle, new EnemyState_Idle(LoveStateMachine));
        HateStateMachine.AddState((int)EnemyStateEnum.RunAway, new EnemyState_RunAway(LoveStateMachine));
    }
    void Update() {
        CurrentStateMachine.Update();
        //Hate
        //RunAway
        if (HateStateMachine.CurEid != (int)EnemyStateEnum.RunAway)
        {
            if (IsNeedToRunAway()) 
                HateStateMachine.SwitchState((int)EnemyStateEnum.RunAway);
        }
        //Love
        if (HateStateMachine.CurEid != (int)EnemyStateEnum.Item)
        {
            if (IsNeedToGoToItem()) 
                LoveStateMachine.SwitchState((int)EnemyStateEnum.Item);
        }
        

    }


    public void ChangeStateMachine()
    {
        if (true)
        {
            //To DO ChangeState
            // CurrentStateMachine =
        }
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
    public IItem Item;
    private bool IsNeedToGoToItem()
    {
        if (Vector2.Distance(this.transform.position, target.position) > 3f&&
            Vector2.Distance(this.transform.position, Item.GetComponent<Transform>().position)<3f)
        {
            return true;
        }

        return false;
    }
    //Run Away
    public Escape escape;

    private bool IsNeedToRunAway()
    {
        if (Vector2.Distance(this.transform.position, target.position) < 3f)
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