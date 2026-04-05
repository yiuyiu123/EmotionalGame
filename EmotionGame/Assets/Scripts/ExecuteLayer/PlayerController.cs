using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float brakeDuration = 0.5f;

    public Rigidbody2D rb;

    public bool isMoving { get; private set; }
    public bool isJumping { get; set; }
    public bool isTakingPhotos { get; set; }

    private bool isGrounded;
    private float brakeTimer;

    public event Action OnMakePhoneCall;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void OnEnable()
    {
        PlayerInputManager pim = FindObjectOfType<PlayerInputManager>();
        if (pim != null)
        {
            pim.OnMoveLeft += HandleMoveLeft;
            pim.OnMoveRight += HandleMoveRight;
            pim.OnJumpOrClimb += HandleJumpOrClimb;
            pim.OnTryMakePhoneCall += HandleTryMakePhoneCall;
        }
    }

    private void OnDisable()
    {
        PlayerInputManager pim = FindObjectOfType<PlayerInputManager>();
        if (pim != null)
        {
            pim.OnMoveLeft -= HandleMoveLeft;
            pim.OnMoveRight -= HandleMoveRight;
            pim.OnJumpOrClimb -= HandleJumpOrClimb;
            pim.OnTryMakePhoneCall -= HandleTryMakePhoneCall;
        }
    }

    private void Update()
    {
        HandleBrake();
    }

    private void HandleMoveLeft()
    {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        isMoving = true;
        brakeTimer = brakeDuration;
    }

    private void HandleMoveRight()
    {
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        isMoving = true;
        brakeTimer = brakeDuration;
    }

    private void HandleJumpOrClimb()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
        }
    }

    private void HandleTryMakePhoneCall()
    {
        if (!isMoving && !isJumping && !isTakingPhotos)
        {
            OnMakePhoneCall?.Invoke();
            Debug.Log("玩家打电话");
        }
    }

    private void HandleBrake()
    {
        if (isMoving)
        {
            brakeTimer -= Time.deltaTime;
            if (brakeTimer <= 0)
            {
                isMoving = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
