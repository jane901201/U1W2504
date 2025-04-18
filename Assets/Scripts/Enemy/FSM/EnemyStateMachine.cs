using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace U1w.FSM
{
    public class EnemyStateMachine : FSM
    {
        public AIController aIController;
        public EnemyStateMachine(AIController aIController)
        {
            this.aIController = aIController;
        }
    }
}