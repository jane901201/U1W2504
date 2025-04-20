using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "NewItem", menuName = "MyGame/Item")]
public abstract class IItem : ScriptableObject
{
    [SerializeField] private Sprite icon;
    
    public Sprite Icon => icon;


    public void Inintialize()
    {
    }
    
    public virtual void Use(ICharacter self, ICharacter[] targets)
    {
        if (self.CharacterState.Role == CharacterState.RoleType.Oni) UseAsOni(self, targets);
        else UseAsHuman(self, targets);
    }

    protected abstract void UseAsOni(ICharacter self, ICharacter[] targets);
    protected abstract void UseAsHuman(ICharacter self, ICharacter[] targets);
    
}
