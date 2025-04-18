using UnityEngine;

namespace DefaultNamespace
{
    public class ICharacter : MonoBehaviour
    {
        [SerializeField] private int hp = 3;
        [SerializeField] private Animator animator;
        private CharacterState characterState; 

        public CharacterState CharacterState { get => characterState; set => characterState = value; }

        
        public int Hp { get => hp; set => hp = value; }
        public Animator Animator{ get => animator; set => animator = value; }
        
        private void Awake()
        {
            characterState = new CharacterState();
        }

    }
}