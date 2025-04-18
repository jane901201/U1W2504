using DefaultNamespace;
using UnityEngine;

public class PlayTest : MonoBehaviour
{
    [SerializeField] private GameSystem gameSystem;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) //TODO:Test
        {
            if (gameSystem.GameState == GameState.PlayerChaseEnemy)
            {
                gameSystem.GameState = GameState.EnemyChasePlayer;
            }
            else
            {
                gameSystem.GameState = GameState.PlayerChaseEnemy;
            }
        }
    }
}
