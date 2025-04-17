using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    public class GameSystem : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private GameUI gameUI;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private float spawnInterval = 5f;
        private List<Vector3Int> tilePositions = new List<Vector3Int>();
        [SerializeField] private GameObject[] objectsToSpawn; 

        
        
        
        private void Start()
        {
            // タイルがある座標をリストアップ
            BoundsInt bounds = tilemap.cellBounds;
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    if (tilemap.HasTile(pos))
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

            Instantiate(objectsToSpawn[0], worldPos, Quaternion.identity);
        }

        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                player.UseItem();
            }
            if (player.CharacterState == CharacterState.Love)
            {
                player.Animator.SetInteger("CharacterState", (int)player.CharacterState);
            }

            if (player.CharacterState == CharacterState.Sad)
            {
                player.Animator.SetInteger("CharacterState", (int)player.CharacterState);
                //Enemy AI　change
            }
        }
        
    }
}