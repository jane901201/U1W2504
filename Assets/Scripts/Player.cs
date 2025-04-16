using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    private Vector3Int playerPos;
    [SerializeField] private CharacterState characterState;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private int hp = 3;
    [SerializeField] private List<IItem> items = new List<IItem>();

    private float moveCooldown = 0.2f; 
    private float lastMoveTime = 0f;

    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerPos = new Vector3Int(0, 0, 0);
        Vector3 worldPosition = tilemap.CellToWorld(playerPos);
        gameObject.transform.position = worldPosition;
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal"); 
        float v = Input.GetAxisRaw("Vertical");   
        movement = new Vector2(h, v).normalized;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void MoveToCell(Vector3Int cellPos)
    {
        Vector3 worldPos = tilemap.CellToWorld(cellPos) + tilemap.cellSize / 2;
        transform.position = worldPos;
    }
    
    

    private void TileMapMove()
    {
        if (Time.time - lastMoveTime < moveCooldown) return;


        if (Input.GetKey(KeyCode.W))
        {
            playerPos += new Vector3Int(0, 1, 0);
            MoveToCell(playerPos);
            lastMoveTime = Time.time;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            playerPos += new Vector3Int(0, -1, 0);
            MoveToCell(playerPos);
            lastMoveTime = Time.time;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            playerPos += new Vector3Int(-1, 0, 0);
            MoveToCell(playerPos);
            lastMoveTime = Time.time;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            playerPos += new Vector3Int(1, 0, 0);
            MoveToCell(playerPos);
            lastMoveTime = Time.time;
        }
    }
}

public enum CharacterState
{
    Love,
    Hate
}