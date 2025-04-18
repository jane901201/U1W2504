using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
namespace U1w.FSM {
    public class EnemyState_Idle : EnemyState
    {
        public EnemyState_Idle(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }
        public override void OnEnter()
        {
            stateMachine.SwitchState((int)EnemyStateEnum.Follow);
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