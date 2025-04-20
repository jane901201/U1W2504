using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AduioMag", menuName = "MyGame/Aduio")]
public class AudioManager : ScriptableObject
{
    public List<AudioClip> Voice=new List<AudioClip>();

    public AudioClip PlaySound(EAudioClips Enumclip)
    {
        return Voice[(int)Enumclip];
    }
}

public enum EAudioClips
{
    Victory,
    Defeat,
    Item,
    
    
}