using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{   

    public ParticleSystem footsteps;
    private ParticleSystem.EmissionModule footEmission;
    [SerializeField] private float footstepsEmissionRate = 35f;

    public ParticleSystem impactEffect;
    private bool wasOnGround;

    private Rigidbody2D rb;
    private Ground ground;

    [SerializeField, Range(0f, 100f)] private float maxSpeed = 4f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 35f;
    [SerializeField, Range(0f, 100f)] private float maxAirAcceleration = 20f;
    [SerializeField, Range(0f, 100f)] private float deAcceleration = 20f;
    [SerializeField, Range(20f, 100f)] private float maxFallSpeed = 40f;

    [SerializeField, Range(0f, 10f)] private float jumpHeight = 3f;
    [SerializeField, Range(0f, 5f)] private int maxAirJumps = 0;
    [SerializeField, Range(0f, 5f)] private float downwardMovementMultiplier = 3f;
    [SerializeField, Range(0f, 5f)] private float upwardMovementMultiplier = 3f;
    [SerializeField, Range(0f, 5f)] private float jumpEarlyMovementModifier = 3f;
    [SerializeField] private float jumpBuffer = 0.1f;
    

    private bool hasBufferedJump_ => onGround && lastJumpPressed_ + jumpBuffer > Time.time;

    private float timeLeftGrounded;
    private bool firstGroundCheck = false;
    private bool coyoteUsable;
    [SerializeField] private float coyoteTimeThreshold = 0.1f;
    private bool CanUseCoyote => coyoteUsable && !onGround && timeLeftGrounded + coyoteTimeThreshold > Time.time;

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
    private bool endedJumpEarly_ = false;
    
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip jumpLanding; 
    private AudioSource audioSource;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ground = GetComponent<Ground>();
        audioSource = GetComponent<AudioSource>();
        footEmission = footsteps.emission;
    }

    public void Update()
    {
        float horizontalVelocity = Mathf.Clamp(horizontal_ * Mathf.Max(maxSpeed - ground.GetFriction(), 0f), -maxSpeed, maxSpeed);
        desiredVelocity_ = new Vector2(horizontalVelocity, 0f);
    }

    public void FixedUpdate()
    {   
        onGround = ground.GetOnGround();
        // Need to detect time when first left the ground
        if (!onGround)
        {
            if (!firstGroundCheck)
            {
                timeLeftGrounded = Time.time;
                coyoteUsable = true;
                firstGroundCheck = !firstGroundCheck;
            }
        }
        else 
        {
            firstGroundCheck = !firstGroundCheck;
            jumpPhase = 0;
        }
        velocity = rb.velocity;
        CalculateJump();
        CalculateGravity();

        rb.velocity = velocity;

        if (horizontal_ != 0)
        {
            acceleration = onGround ? maxAcceleration : maxAirAcceleration;
            maxSpeedChange = acceleration * Time.deltaTime;
            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity_.x, maxSpeedChange);
            footEmission.rateOverTime = footstepsEmissionRate;
            rb.velocity = velocity;
        }
        else
        {
            footEmission.rateOverTime = 0f;
            // no input so we slow the character down to avoid 'sliding'
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deAcceleration * Time.deltaTime);
            // keep the vertical speed the same
            rb.velocity = new Vector2(velocity.x, rb.velocity.y);
        }

        if (!wasOnGround && onGround)
        {
            audioSource.PlayOneShot(jumpLanding, 0.6f);
            impactEffect.gameObject.SetActive(true);
            impactEffect.Stop();
            impactEffect.transform.position = footsteps.transform.position;
            impactEffect.Play();
        }
        wasOnGround = onGround;
    }

    public void CalculateJump()
    {   
        if (jumpPressed && jumpPhase < maxAirJumps && CanUseCoyote || hasBufferedJump_)
        {
            audioSource.PlayOneShot(jumpSound, 0.3f);
            coyoteUsable = false;
            jumpPressed = false;
            endedJumpEarly_ = false;
            jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * jumpHeight);
            if (velocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
            }
            velocity.y += jumpSpeed;
        }

        if (!onGround && jumpReleased && !endedJumpEarly_ && velocity.y > 0)
        {
            endedJumpEarly_ = true;
            jumpReleased = false;
        }
    }

    public void CalculateGravity()
    {
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

        // Variable jump height
        if (endedJumpEarly_ && velocity.y > 0)
        {
            rb.gravityScale = upwardMovementMultiplier * jumpEarlyMovementModifier;
        }

        // Clamp fall speed
        if (velocity.y < -maxFallSpeed) { velocity.y = -maxFallSpeed; }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal_ = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // calculate time for jump buffer
            lastJumpPressed_ = Time.time;
            jumpPressed = true;
            jumpReleased = false;
        }
        if (context.canceled)
        {
            jumpReleased = true;
        }
    }
}
