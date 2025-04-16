using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using U1w.FSM;

public class HateAIController : MonoBehaviour
{
    public EnemyStateMachine StateMachine;

    private void Start()
    {
        StateMachine = new(this);
        StateMachine.AddState((int)PlayerStateEnum.Idle, new EnemyState_Idle(StateMachine));
        //记得设置初始状态机
        StateMachine.Begin((int)PlayerStateEnum.Idle);
    }
    //言简意赅，将状态机的更新提上来
    public void Update()
    {
        StateMachine.Update();
    }
    public void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }
}
public enum PlayerStateEnum
{
    None,
    Idle,
    Move,
    Attack
}