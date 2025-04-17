using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveAITwat : MonoBehaviour
{
    public float moveSpeed = 5f; // 可在Inspector中设置移动速度

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 moveVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 获取输入（水平、垂直轴：WASD或方向键）
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // 归一化，防止对角线移动比单方向快
        moveInput = new Vector2(moveX, moveY).normalized;
        moveVelocity = moveInput * moveSpeed;
    }

    void FixedUpdate()
    {
        // 实际移动
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }
}
