using UnityEngine;

namespace DefaultNamespace
{
    public class ICharacter : MonoBehaviour
    {
        [SerializeField] private int hp = 3;
        [SerializeField] private Animator animator;

        public int Hp { get => hp; set => hp = value; }
    }
}