using System.Collections;
using System.Collections.Generic;
using U1w.FSM;
using UnityEngine;

public class EnemyState_Follow : EnemyState
{
    public EnemyState_Follow(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }
    public override void OnEnter()
    {

    }
    public override void OnPhysicsUpdate()
    {

    }
    public override void OnUpdate()
    {
        if ( stateMachine.hateAIController
                .target != null ) {
            stateMachine.hateAIController.agent.SetDestination(stateMachine.hateAIController.target.position);
        }
    }
    
    public override void OnExit()
    {
            
    }

}
