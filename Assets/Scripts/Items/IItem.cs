using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;


[CreateAssetMenu(fileName = "NewItem", menuName = "MyGame/Item")]
public abstract class IItem : ScriptableObject
{
    public abstract void Use(ICharacter self, ICharacter[] targets);
    
}
