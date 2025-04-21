using UnityEngine;
using DefaultNamespace;

public class CharacterTouchTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy" && other.gameObject.GetComponent<Enemy>().CharacterState.Emotion == CharacterState.EmotionType.Sad)
        {
			Player player = GetComponentInParent<Player>();
	    	Debug.Log(player);
            
            player.Attack(other.gameObject.GetComponent<Enemy>());

            Debug.Log("Enemy be attacked");
        }
        else if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<Player>().CharacterState.Emotion == CharacterState.EmotionType.Sad)
        {
			Enemy enemy = GetComponentInParent<Enemy>();
		    Debug.Log(enemy);
            
            enemy.Attack(other.gameObject.GetComponent<Player>());

            Debug.Log("Player be attacked");
        }
    }
}
