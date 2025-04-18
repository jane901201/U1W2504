using UnityEngine;

namespace DefaultNamespace
{
    public class ICharacter : MonoBehaviour
    {
        [SerializeField] private int hp = 3;
        [SerializeField] private CharacterState characterState;
        [SerializeField] private Animator animator;
        // ID could be any string, for item timer task
        [SerializeField] private string id;

        public int Hp { get => hp; set => hp = value; }
        public string Id { get => id; set => id = value; }
        public CharacterState CharacterState { get => characterState; set => characterState = value; }
    }
}