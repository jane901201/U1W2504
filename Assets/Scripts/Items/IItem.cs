using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;


[CreateAssetMenu(fileName = "NewItem", menuName = "MyGame/Item")]
public abstract class IItem : ScriptableObject
{
    public virtual void Use(ICharacter self, ICharacter[] targets)
    {
        if (self.CharacterState.Role == CharacterState.RoleType.Oni) UseAsOni(self, targets);
        else UseAsHuman(self, targets);
    }

    protected abstract void UseAsOni(ICharacter self, ICharacter[] targets);
    protected abstract void UseAsHuman(ICharacter self, ICharacter[] targets);
    
}
