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
            //����״̬����ҿ�ʼ���Ŵ�������
           
        }
        public override void OnPhysicsUpdate()
        {
            //�����ϣ������״̬����Ҳ������ٶ�Ϊ0
           
        }
        public override void OnUpdate()
        {
            //��⵽����ƶ��ˣ���ô�л����ƶ�״̬
           
        }
        //û���뿪��������Ϊ���ƶ�״̬�У������ֱ���л�����
        //һ����˵���뿪�ͽ��뷽��ֻҪ��һ�����ھ����ˣ������һЩר���Ĳ���֮�����Ҫ����������뿪����������
    }
}