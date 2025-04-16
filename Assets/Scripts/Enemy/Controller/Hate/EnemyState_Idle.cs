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
            //进入状态，玩家开始播放待机动画
           
        }
        public override void OnPhysicsUpdate()
        {
            //物理上，在这个状态，玩家不动，速度为0
           
        }
        public override void OnUpdate()
        {
            //检测到玩家移动了，那么切换到移动状态
           
        }
        //没有离开方法，因为在移动状态中，进入会直接切换动画
        //一般来说，离开和进入方法只要有一个存在就行了，如果有一些专属的参数之类的需要清理则会用离开方法来清理
    }
}