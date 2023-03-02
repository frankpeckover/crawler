using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private LayerMask jumpableGround;

    [SerializeField] private int maximumJumps = 2;
    private int jumpsRemaining;
    private float heightOfCollider;

    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;

    private enum MovementState { idle, running, jumping, falling }

    private void Start()
    {
        this.jumpsRemaining = this.maximumJumps;
        heightOfCollider = boxCollider2D.bounds.extents.y;
    }

    // Update is called once per frame
    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        rigidBody2D.velocity = new Vector2(dirX * moveSpeed, rigidBody2D.velocity.y);

        if ((Input.GetButtonDown("Jump")))
        {
            if (IsGrounded()) 
            {
                this.jumpsRemaining = this.maximumJumps;
            }

            if (this.jumpsRemaining > 0) 
            {
                this.jumpsRemaining--;
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpForce);
            }
        }

        if (rigidBody2D.velocity.y > -0.1 && rigidBody2D.velocity.y < 0.1) 
        {
            if (IsGrounded()) 
            {
                this.jumpsRemaining = this.maximumJumps;
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position + (-Vector3.up * heightOfCollider), -Vector3.up, heightOfCollider);
    }
}