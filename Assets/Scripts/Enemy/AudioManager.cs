using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
[CreateAssetMenu(fileName = "AduioMag", menuName = "MyGame/Aduio")]
public class AudioManager : ScriptableObject
{
    public List<AudioClip> LoveVoice=new List<AudioClip>();
    public List<AudioClip> HateVoice=new List<AudioClip>();

    public AudioClip PlaySound()
    {
       
        if (GameSystem.Instance.GameState != GameState.EnemyChasePlayer)
        {
            
            return LoveVoice[(int)Random.Range(0, LoveVoice.Count)];
        }
        else
        {
            return HateVoice[(int)Random.Range(0, HateVoice.Count)];
        }
        
    }
}
