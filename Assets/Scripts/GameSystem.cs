using System;
using System.Collections;
using System.Collections.Generic;
using TimerFrame;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class GameSystem : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private Enemy enemy;
        [SerializeField] private GameUI gameUI;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Tilemap obstacleTilemap;
        [SerializeField] private float spawnInterval = 5f;
        [SerializeField] private GameObject[] objectsToSpawn;
        [SerializeField] private GameState gameState;
        [SerializeField] private SceneManager sceneManager;
        
        private List<Vector3Int> tilePositions = new List<Vector3Int>();
        private List<Vector3Int> obstaclePositions = new List<Vector3Int>();
        
        public static GameSystem Instance;
        public Tilemap GetTilemap() => tilemap;
        public Tilemap GetObstacleTilemap() => obstacleTilemap;
        
        public GameState GameState { get => gameState; set => gameState = value; }

        private void Awake()
        {
            Instance = this;
            gameUI = GameObject.Find("Canvas").GetComponent<GameUI>();
            obstacleTilemap = GameObject.Find("ObstaclesTilemap").GetComponent<Tilemap>();
            tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        }

        private void Start()
        {
            gameState = GameState.PlayerChaseEnemy;
            gameUI.SetPlayerChaseEnemyIcon();
            player.PlayerStateChangEvent += StateChangEvent;
            player.StateChangEvent += StateChangEvent;
            enemy.StateChangEvent += StateChangEvent;
            player.ItemEffectEvent += PlayerItemEffectEvent;
            // タイルがある座標をリストアップ
            BoundsInt bounds = tilemap.cellBounds;
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    if (tilemap.HasTile(pos) && !obstacleTilemap.HasTile(pos))
                    {
                        tilePositions.Add(pos);
                    }
                }
            }

            // 一定時間ごとに生成を開始
            // StartCoroutine(SpawnLoop());
            TimerManager.Instance.AddTask("SpawnObjectAtRandomPosition", spawnInterval, SpawnObjectAtRandomPosition, true);
        }

        void SpawnObjectAtRandomPosition()
        {
            if (tilePositions.Count == 0) return;

            int index = Random.Range(0, tilePositions.Count);
            Vector3Int cellPos = tilePositions[index];
            Vector3 worldPos = tilemap.CellToWorld(cellPos) + tilemap.cellSize / 2; // 中心に合わせるため
            
            int itemNum = Random.Range(0, objectsToSpawn.Length);

            Instantiate(objectsToSpawn[itemNum], worldPos, Quaternion.identity);
        }

        
        private void Update()
        {
            if (player.Hp == 0)
            {
                GameOver();
            }
            else if (enemy.Hp == 0)
            {
                Victory();
            }
            
            gameUI.SetPlayerHPIcon(player.Hp);
            gameUI.SetEnemyHPIcon(enemy.Hp);
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                player.UseItem();
            }

            if (gameState == GameState.PlayerChaseEnemy)
            {
                player.CharacterState.Emotion = CharacterState.EmotionType.Love;
                enemy.CharacterState.Emotion = CharacterState.EmotionType.Sad;
                player.Animator.SetInteger("CharacterState", (int)player.CharacterState.Emotion);
                enemy.Animator.SetInteger("CharacterState", (int)enemy.CharacterState.Emotion);
                gameUI.SetPlayerIcon(player);
                gameUI.SetEnemyIcon(enemy);
            }

            if (gameState == GameState.EnemyChasePlayer)
            {
                player.CharacterState.Emotion = CharacterState.EmotionType.Sad;
                enemy.CharacterState.Emotion = CharacterState.EmotionType.Love;
                player.Animator.SetInteger("CharacterState", (int)player.CharacterState.Emotion);
                enemy.Animator.SetInteger("CharacterState", (int)enemy.CharacterState.Emotion);
                gameUI.SetPlayerIcon(player);
                gameUI.SetEnemyIcon(enemy);
                
            }
        }

        private void StateChangEvent()
        {
            if (gameState == GameState.PlayerChaseEnemy)
            {
                gameState = GameState.EnemyChasePlayer;
                gameUI.SetEnemyChasePlayerIcon();
            }
            else if(gameState == GameState.EnemyChasePlayer)
            {
                gameState = GameState.PlayerChaseEnemy;
                gameUI.SetPlayerChaseEnemyIcon();
            }
        }

        private void PlayerItemEffectEvent(Sprite sprite, float duration)
        {
            gameUI.SetPlayerEffectImage(sprite, duration);
        }

        private void Victory()
        {
            sceneManager.LoadScene("VictoryScene");
        }

        private void GameOver()
        {
            sceneManager.LoadScene("GameOverScene");
        }
        
    }

    public enum GameState
    {
        EnemyChasePlayer = 0,
        PlayerChaseEnemy = 1
    } 
}