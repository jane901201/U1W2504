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
        Vector3 tragetPos= stateMachine.hateAIController.escape.GetEscapeTargetLocation(stateMachine.hateAIController.transform.position,
            stateMachine.hateAIController.target.position);
      
        stateMachine.hateAIController.agent.SetDestination(tragetPos, delegate{stateMachine.SwitchState((int)EnemyStateEnum.Idle); });
    }
    public override void OnPhysicsUpdate()
    {

    }
    public override void OnUpdate()
    {

    }
    
    public override void OnExit()
    {
            
    }

}
