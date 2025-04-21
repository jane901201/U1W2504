using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class AduioController : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource mainSource;
    public AudioSource voiceSource;
    public AudioManager voiceAudioManager;
    public GameSystem gameSystem;
    public static AduioController _Instance;
    public AudioClip LoveBGM;
    public AudioClip HateBGM;
    public GameObject BG;

    [Header("Volume")]
    [Range(0f, 1f)]
    public float mainVolume=0.5f;
    [Range(0f, 1f)]
    public float voiceVolume;
    [Range(0f, 1f)]
    public float voiceVolumeWhenMainPlay=0.05f;
    [Range(60, 600)]
    public int GaiLv = 300;
    
    void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
            
        }
       
    }

    void Start()
    {
        PlayBGM();
    }


    public void PlayBGM()
    {
        if (mainSource != null)
        {
            if (gameSystem.GameState != GameState.EnemyChasePlayer)
            {
                mainSource.clip = LoveBGM;
                mainSource.loop = true;
                mainSource.volume = mainVolume;
                BG.GetComponent<Animator>().SetBool("GameState", true);
                mainSource.Play();
            }
            else
            {
                mainSource.clip = HateBGM;
                mainSource.volume = mainVolume;
                mainSource.loop = true;
                BG.GetComponent<Animator>().SetBool("GameState", false);
                mainSource.Play();
            }
        }
    }

    private void FixedUpdate()
    {
        if (voiceSource.isPlaying)
        {
            mainSource.volume = voiceVolumeWhenMainPlay;
        }
        else
        {
            mainSource.volume = mainVolume;
        }

        if (!voiceSource.isPlaying)
        {
            //Random
            int index = Random.Range(0, GaiLv);
            if (index < 1)
            {
                voiceSource.clip=voiceAudioManager.PlaySound();
                voiceSource.Play();
            }
        }
    }
}
