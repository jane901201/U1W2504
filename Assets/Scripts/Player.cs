using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : ICharacter
{
    private Vector3Int playerPos;
    [SerializeField] private List<IItem> items = new List<IItem>();

    public Action PlayerStateChangEvent;
    
    private float moveCooldown = 0.2f; 
    private float lastMoveTime = 0f;

    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    
    
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
        if(items.Count == 0) return;
        items[0].Use(this, null); //TODO:後調整
        items.RemoveAt(0);
    } 
}