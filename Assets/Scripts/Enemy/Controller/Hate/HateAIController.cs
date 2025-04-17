using System;
using System.Collections;
using System.Collections.Generic;
using PolyNav;
using UnityEngine;
using U1w.FSM;

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

        LoveStateMachine.Begin((int)CurrentState);
        //Hate
        HateStateMachine.AddState((int)EnemyStateEnum.Idle, new EnemyState_Idle(LoveStateMachine));
        HateStateMachine.AddState((int)EnemyStateEnum.RunAway, new EnemyState_RunAway(LoveStateMachine));
    }
    void Update() {
        CurrentStateMachine.Update();
        if (LoveStateMachine.CurEid != (int)EnemyStateEnum.RunAway)
        {
            if (IsNeedToRunAway()) LoveStateMachine.SwitchState((int)EnemyStateEnum.RunAway);
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
    //Run Away
    public Escape escape;
    private bool IsNeedToRunAway()
    {
        if (Vector2.Distance(this.transform.position, target.position) < 5f)
        {
            return true;
        }
        return false;
    }
    //
    


    
    
    
}
public enum EnemyStateEnum
{
    Idle,
    Follow,
    RunAway
}