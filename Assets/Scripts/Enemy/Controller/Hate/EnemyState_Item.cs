using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
namespace U1w.FSM 
{
    public class EnemyState_Item : EnemyState
    {
        public EnemyState_Item(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }
        public override void OnEnter()
        {
            if (stateMachine.hateAIController.Item != null)
            {
                stateMachine.hateAIController.agent.SetDestination(stateMachine.hateAIController.Item.GetComponent<Transform>().position, 
                    delegate{stateMachine.SwitchState((int)EnemyStateEnum.Idle); });
            }
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
}