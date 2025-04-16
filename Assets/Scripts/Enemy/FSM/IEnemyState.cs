using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace U1w.FSM
{
    public interface IState
    {
        public void OnEnter(); //进入
        public void OnExit(); //退出
        public void OnUpdate(); 
        public void OnPhysicsUpdate(); 
    }
}