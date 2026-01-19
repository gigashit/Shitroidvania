using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpPower = 12f;

    [Header("Jump Tweaks (The Bonus)")]
    [Tooltip("How much to cut velocity if button released early (0 to 1). Lower = shorter hop.")]
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("References")]
    [SerializeField] private Animator roachAnimator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private float horizontalInput;

    // --- Input System Events ---

    // Linked to the "Move" action
    public void OnMove(InputAction.CallbackContext context)
    {
        // Read the X value (-1 to 1)
        horizontalInput = context.ReadValue<Vector2>().x;
    }

    // Linked to the "Jump" action
    public void OnJump(InputAction.CallbackContext context)
    {
        // 1. Initial Jump (When button is PRESSED)
        if (context.performed && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        }

        // 2. Variable Jump Height (When button is RELEASED)
        // If the player lets go while moving up, we cut the speed
        if (context.canceled && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }
    }

    // --- Physics Loop ---

    private void FixedUpdate()
    {
        // Apply velocity for walking
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    // --- Logic Loop ---

    private void Update()
    {
        FlipSprite();
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        bool isMoving = false;

        if (IsGrounded())
        {
            roachAnimator.SetBool("isJumping", false);
            if (horizontalInput != 0f) { isMoving = true; } else { isMoving = false; }
            roachAnimator.SetBool("isWalking", isMoving);
        }
        else
        {
            roachAnimator.SetBool("isJumping", true);
        }
    }

    private void FlipSprite()
    {
        // If moving right, face right. If moving left, face left.
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    public bool IsGrounded()
    {
        // Creates a tiny invisible circle at the feet to check for ground
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}