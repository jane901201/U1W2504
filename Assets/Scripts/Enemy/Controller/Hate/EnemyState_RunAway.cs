using System.Collections;
using System.Collections.Generic;
using U1w.FSM;
using UnityEngine;

public class EnemyState_RunAway : EnemyState
{
    public EnemyState_RunAway(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }
    public override void OnEnter()
    {
        Vector3 tragetPos= stateMachine.aIController.escape.MoveToFarthestTileInRange(stateMachine.aIController.transform.position,
            stateMachine.aIController.target.position);
      
        stateMachine.aIController.agent.SetDestination(tragetPos, delegate{stateMachine.SwitchState((int)EnemyStateEnum.Idle); });
    }
    public override void OnPhysicsUpdate()
    {

    }
    public override void OnUpdate()
    {
        //Debug.Log("Run Away");
        if (!stateMachine.aIController.agent.hasPath)
        {
            stateMachine.SwitchState((int)EnemyStateEnum.Idle);
        }

    }
    
    public override void OnExit()
    {
            
    }

}
