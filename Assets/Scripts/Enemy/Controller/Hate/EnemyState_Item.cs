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
            if (stateMachine.aIController.Item != null)
            {
                stateMachine.aIController.agent.SetDestination(stateMachine.aIController.Item.GetComponent<Transform>().position, 
                    delegate{stateMachine.SwitchState((int)EnemyStateEnum.Idle); });
            }
        }
        public override void OnPhysicsUpdate()
        {
           
        }
        public override void OnUpdate()
        {
            Debug.Log("Item");
            if (stateMachine.aIController.Item == null)
            {
                stateMachine.SwitchState((int)EnemyStateEnum.Idle);
            }
               
        }

        public override void OnExit()
        {
            
        }
    }
}