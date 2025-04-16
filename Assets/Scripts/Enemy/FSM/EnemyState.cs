using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace U1w.FSM
{
    public class EnemyState : IState
    {


        protected EnemyStateMachine stateMachine;

        public EnemyState(EnemyStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }
        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void OnUpdate() { }
        public virtual void OnPhysicsUpdate() { }
    }
}
