using System;
using System.Collections;
using System.Collections.Generic;
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
            gameUI =GameObject.Find("Canvas").GetComponent<GameUI>();
            obstacleTilemap = GameObject.Find("ObstaclesTilemap").GetComponent<Tilemap>();
        }

        private void Start()
        {
            gameState = GameState.PlayerChaseEnemy;
            player.PlayerStateChangEvent += StateChangEvent;
            player.StateChangEvent += StateChangEvent;
            enemy.StateChangEvent += StateChangEvent;
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
            StartCoroutine(SpawnLoop());
        }
        
        IEnumerator SpawnLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnInterval);
                SpawnObjectAtRandomPosition();
            }
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

            if (enemy.Hp == 0)
            {
                Victory();
            }
            
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
            }

            if (gameState == GameState.EnemyChasePlayer)
            {
                player.CharacterState.Emotion = CharacterState.EmotionType.Sad;
                enemy.CharacterState.Emotion = CharacterState.EmotionType.Love;
                player.Animator.SetInteger("CharacterState", (int)player.CharacterState.Emotion);
                enemy.Animator.SetInteger("CharacterState", (int)enemy.CharacterState.Emotion);
            }
        }

        private void StateChangEvent()
        {
            if (gameState == GameState.PlayerChaseEnemy)
            {
                gameState = GameState.EnemyChasePlayer;
            }
            else if(gameState == GameState.EnemyChasePlayer)
            {
                gameState = GameState.PlayerChaseEnemy;
            }
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