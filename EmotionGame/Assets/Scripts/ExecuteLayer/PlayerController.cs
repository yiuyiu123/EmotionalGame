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
    public bool isGrounded { get; private set; }

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
        // 检查是否在爬墙区域
        PlayerColliderDetect pcd = FindObjectOfType<PlayerColliderDetect>();
        if (pcd != null && pcd.isInClimbArea)
        {
            // 在爬墙区域，由PlayerColliderDetect处理爬墙
            return;
        }
        
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

    private Vector3 initialPosition;

    private void Start()
    {
        // 保存初始位置
        initialPosition = transform.position;
        
        // 显式设置初始状态
        isGrounded = true; // 假设初始位置在地面上
        isJumping = false;
        isMoving = false;
        isTakingPhotos = false;
    }

    public void Respawn()
    {
        // 重置位置到初始位置
        transform.position = initialPosition;
        
        // 重置速度为零
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 3f; // 确保重力设置正确
        }
        
        // 重置状态变量
        isMoving = false;
        isJumping = false;
        isTakingPhotos = false;
        isGrounded = true; // 假设重生点在地面上
        brakeTimer = 0f;
        
        Debug.Log("PlayerController: 角色已重生到初始位置并重置所有状态");
    }
}
