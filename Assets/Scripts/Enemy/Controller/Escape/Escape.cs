using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Escape : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap groundTilemap;        // 地面或可通行的Tilemap
    public Tilemap obstacleTilemap;      // 障碍物Tilemap
    public int minRange = 3;          //最小逃跑搜索半径
    public int maxRange = 5;          //最大逃跑搜索半径
    public Vector3 MoveToFarthestTileInRange(Vector3 AIPosition,Vector3 targetPosition)
    {
        // 获取AI当前位置对应的Tile坐标
        Vector3Int aiCellPos = groundTilemap.WorldToCell(AIPosition);

        // 获取PL当前位置对应的Tile坐标
        Vector3Int targetCellPos = obstacleTilemap.WorldToCell(targetPosition);
        // 找到“最远的”且非障碍的Tile坐标
        Vector3Int bestCellPos = FindFarthestFromEnemyButWithinRangeOfAI(aiCellPos,targetCellPos, minRange, maxRange);
        
        if (bestCellPos != aiCellPos)
        {
            return groundTilemap.GetCellCenterWorld(bestCellPos);
        }

        Debug.Log("Error");
        return Vector3.zero;
    }

    /// <summary>
    /// 返回在[minRange, maxRange]内，且非障碍物、距离最远的Tile坐标
    /// </summary>
    private Vector3Int FindFarthestFromEnemyButWithinRangeOfAI(
        Vector3Int aiCellPos, 
        Vector3 playerPos, 
        int minRange, 
        int maxRange
    )
    {
        Vector3Int bestCellPos = aiCellPos;
        float bestDistToEnemy = float.MinValue;

        for (int x = -maxRange; x <= maxRange; x++)
        {
            for (int y = -maxRange; y <= maxRange; y++)
            {
                Vector3Int checkPos = aiCellPos + new Vector3Int(x, y, 0);
                
                float distAi = Vector2Int.Distance(
                    new Vector2Int(aiCellPos.x, aiCellPos.y),
                    new Vector2Int(checkPos.x, checkPos.y)
                );
                if (distAi < minRange || distAi > maxRange)
                    continue; // 超过范围就跳过

                // 2) 是否障碍物
                if (IsObstacle(checkPos))
                    continue;

                // 3) 计算到敌人的距离
                float distPlayer = Vector2.Distance(
                    new Vector2(playerPos.x, playerPos.y),
                    new Vector2Int(checkPos.x, checkPos.y)
                );
                // 4) 如果到敌人的距离更大，就更新
                if (distPlayer > bestDistToEnemy)
                {
                    bestDistToEnemy = distPlayer;
                    bestCellPos = checkPos;
                }
            }
        }

        return bestCellPos;
    }
    /// 根据Tilemap或Tile类型判定此坐标是否障碍
    private bool IsObstacle(Vector3Int cellPos)
    {
        // 如果obstacleTilemap上有Tile，就视为障碍
        TileBase obstacleTile = obstacleTilemap.GetTile(cellPos);
        return obstacleTile != null;
    }

}