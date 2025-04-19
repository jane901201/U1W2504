using UnityEngine;
using DefaultNamespace;

public class CharacterTouchTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy" && other.gameObject.GetComponent<Enemy>().CharacterState.Emotion == CharacterState.EmotionType.Sad)
        {
            Debug.Log("Enemy be attacked");
        }
        else if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<Player>().CharacterState.Emotion == CharacterState.EmotionType.Sad)
        {
            Debug.Log("Player be attacked");
        }
    }
}
