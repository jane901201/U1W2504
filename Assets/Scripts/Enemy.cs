using UnityEngine;

namespace DefaultNamespace
{
    public class Enemy : ICharacter
    {
        AIController aiController;

        protected override void Awake()
        {
            base.Awake();
            aiController = GetComponent<AIController>();
        }

        public override void TakeDamage(ICharacter character)
        {
            base.TakeDamage(character);
            StateChangEvent?.Invoke();
            character.IsForzen = true;
            StartCoroutine(character.WaitAndSetFalse());

        }
    }
}