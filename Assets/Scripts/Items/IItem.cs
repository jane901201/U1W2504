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
    [SerializeField] private bool isDurationItem = false;
    [SerializeField] private Sprite oniEffectIcon;
    [SerializeField] private Sprite humanEffectIcon;
    
    public Sprite Icon => icon;

    public bool IsDurationItem => isDurationItem;

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

    /// <summary>
    /// humanかoniのdurationを返すために使います
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public virtual float GetDuration(ICharacter self)
    {
        
        return 1f;
    }

    public Sprite GetEffectIcon(ICharacter self)
    {
        return self.CharacterState.Role == CharacterState.RoleType.Oni ? oniEffectIcon : humanEffectIcon; 
    } 
    
}
