using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTouchTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("ONTRIGGER");
        if (other.gameObject.tag == "Enemy" )
        {
            Debug.Log("Enemy be attacked");
        }
        else if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player be attacked");
        }
    }
}
