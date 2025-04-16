using System;
using System.Collections;
using System.Collections.Generic;
using PolyNav;
using UnityEngine;
using U1w.FSM;

public class HateAIController : MonoBehaviour
{
    public EnemyStateMachine StateMachine;
    
    public EnemyStateEnum CurrentState;

    private void Start()
    {
        StateMachine = new(this);
        StateMachine.AddState((int)EnemyStateEnum.Idle, new EnemyState_Idle(StateMachine));
        StateMachine.AddState((int)EnemyStateEnum.Follow, new EnemyState_Follow(StateMachine));
        StateMachine.AddState((int)EnemyStateEnum.RunAway, new EnemyState_RunAway(StateMachine));
        StateMachine.Begin((int)CurrentState);
    }
    
    public void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }
    //Follow
    public Transform target;

    
    //Map
    private PolyNavAgent _agent;
    public PolyNavAgent agent {
        get { return _agent != null ? _agent : _agent = GetComponent<PolyNavAgent>(); }
    }
    //Run Away
    public Escape escape;
    
    public bool IsNeedToRunAway()
    {
        if (Vector2.Distance(this.transform.position, target.position) < 5f)
        {
            return true;
        }
        return false;
    }
    

    void Update() {
        StateMachine.Update();
        if(IsNeedToRunAway()) StateMachine.SwitchState((int)EnemyStateEnum.RunAway);

    }



    /** test begin */
    private bool _bNeedToFollow;
    
    public bool needToFollow{get{return _bNeedToFollow;}set
    {
        if (value == true) StateMachine.SwitchState((int)EnemyStateEnum.Follow);else StateMachine.SwitchState((int)EnemyStateEnum.RunAway);}
    
    }
    
    
    
}
public enum EnemyStateEnum
{
    None,
    Idle,
    Follow,
    RunAway
}