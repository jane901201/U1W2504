using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "NewItem", menuName = "MyGame/Item")]
public abstract class IItem : ScriptableObject
{
    [SerializeField] private Sprite icon;
    
    [Header("Tilemap")]
    private Tilemap _groundTilemap;
    private Tilemap _obstacleTilemap;
    
    public Sprite Icon => icon;


    public void Inintialize(Tilemap groundTilemap, Tilemap obstacleTilemap)
    {
        _groundTilemap = groundTilemap;
        _obstacleTilemap = obstacleTilemap;
    }
    
    public virtual void Use(ICharacter self, ICharacter[] targets)
    {
        if (self.CharacterState.Role == CharacterState.RoleType.Oni) UseAsOni(self, targets);
        else UseAsHuman(self, targets);
    }

    protected abstract void UseAsOni(ICharacter self, ICharacter[] targets);
    protected abstract void UseAsHuman(ICharacter self, ICharacter[] targets);
    
}
