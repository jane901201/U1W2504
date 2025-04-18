using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    private Vector3Int playerPos;
    [SerializeField] private CharacterState characterState;
    [SerializeField] private int hp = 3;
    [SerializeField] private List<IItem> items = new List<IItem>();
    [SerializeField] private Animator animator;

    private float moveCooldown = 0.2f; 
    private float lastMoveTime = 0f;

    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    public CharacterState CharacterState { get => characterState; set => characterState = value; }
    public Animator Animator{ get => animator; set => animator = value; }

    public int Hp { get => hp; set => hp = value; }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerPos = new Vector3Int(0, 0, 0);
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
    
    public void AddItem(IItem item)
    {
        if (items.Count < 2)
        {
            items.Add(item);
        }
    }

    public void UseItem()
    {
        items[0].Use(this);
        items.RemoveAt(0);
    } 
}