using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace U1w.FSM
{
    public class EnemyStateMachine : FSM
    {
        public HateAIController hateAIController;
        public EnemyStateMachine(HateAIController hateAIController)
        {
            this.hateAIController = hateAIController;
        }
    }
}