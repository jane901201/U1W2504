using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewItem", menuName = "MyGame/Item")]
public abstract class IItem : ScriptableObject
{
    public abstract void Use(Player player);
}
