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
        //�ǵ����ó�ʼ״̬��
        StateMachine.Begin((int)PlayerStateEnum.Idle);
    }
    //�Լ����࣬��״̬���ĸ���������
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