using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{   
    private Rigidbody2D rb;
    private Ground ground;

    [SerializeField, Range(0f, 100f)] private float maxSpeed = 4f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 35f;
    [SerializeField, Range(0f, 100f)] private float maxAirAcceleration = 20f;
    [SerializeField, Range(0f, 100f)] private float deAcceleration = 20f;
    
    [SerializeField, Range(0f, 10f)] private float jumpHeight = 3f;
    [SerializeField, Range(0f, 5f)] private int maxAirJumps = 0;
    [SerializeField, Range(0f, 5f)] private float downwardMovementMultiplier = 3f;
    [SerializeField, Range(0f, 5f)] private float upwardMovementMultiplier = 3f;
    [SerializeField] private float jumpBuffer = 0.1f;

    private bool hasBufferedJump_ => onGround && lastJumpPressed_ + jumpBuffer > Time.time;

    private float defaultGravityScale = 1f;

    private float maxSpeedChange;
    private float acceleration;

    private float horizontal_;
    private Vector2 desiredVelocity_;
    private Vector2 velocity;
    private bool onGround;
    private float speed_;

    private float jumpPhase = 0;
    private bool jumpPressed;
    private bool jumpReleased;
    private float lastJumpPressed_;
    

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ground = GetComponent<Ground>();
    }

    public void Update()
    {
        float horizontalVelocity = Mathf.Clamp(horizontal_ * Mathf.Max(maxSpeed - ground.GetFriction(), 0f), -maxSpeed, maxSpeed);
        desiredVelocity_ = new Vector2(horizontalVelocity, 0f);
    }

    public void FixedUpdate()
    {   
        onGround = ground.GetOnGround();
        velocity = rb.velocity;


        if (onGround)
        {
            jumpPhase = 0;
        }

        if (jumpPressed || hasBufferedJump_)
        {
            jumpPressed = false;
            CalculateJump();
        }

        if (velocity.y > 0)
        {
            rb.gravityScale = upwardMovementMultiplier;
        }
        else if (velocity.y < 0)
        {
            rb.gravityScale = downwardMovementMultiplier;
        }
        else if (velocity.y == 0)
        {
            rb.gravityScale = defaultGravityScale;
        }

        rb.velocity = velocity;

        if (horizontal_ != 0)
        {
            acceleration = onGround ? maxAcceleration : maxAirAcceleration;
            maxSpeedChange = acceleration * Time.deltaTime;
            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity_.x, maxSpeedChange);

            rb.velocity = velocity;
        }
        else
        {
            // no input so we slow the character down to avoid 'sliding'
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deAcceleration * Time.deltaTime);
            // keep the vertical speed the same
            rb.velocity = new Vector2(velocity.x, rb.velocity.y);
        }
    }

    public void CalculateJump()
    {
        if (onGround || jumpPhase < maxAirJumps)
        {
            jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * jumpHeight);
            if (velocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
            }
            velocity.y += jumpSpeed;
        }

        if (!onGround && jumpReleased)
        {}
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal_ = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // calculate time for jump buffer
            lastJumpPressed_ = Time.time;
            jumpPressed = true;
        }
        if (context.canceled)
        {
            jumpReleased = true;
        }
    }
}