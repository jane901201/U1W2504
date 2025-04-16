using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace U1w.FSM
{
    public class FSM 
    {

        public Dictionary<int, IState> StateDict;

        public IState currentState;
        public int CurEid;
        public FSM()
        {
            StateDict = new();
        }
        public virtual void AddState(int eid, IState state)
        {
            if (!StateDict.ContainsKey(eid)) StateDict.Add(eid, state);
        }

        public virtual void Update()
        {
            currentState?.OnUpdate();
        }

        public virtual void FixedUpdate()
        {
            currentState?.OnPhysicsUpdate();
        }
        
        public virtual void Begin(int eid)
        {
            if (!StateDict.ContainsKey(eid)) return;
            CurEid = eid;
            currentState = StateDict[eid];
            currentState?.OnEnter();
        }

        public void SwitchState(int eid)
        {
            if (!StateDict.ContainsKey(eid)) return;
            currentState?.OnExit();
            currentState = StateDict[eid];
            CurEid = eid;
            currentState?.OnEnter();

        }
    }

}