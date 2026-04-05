using System;
using System.Collections;
using UnityEngine;

public class PlayerColliderDetect : MonoBehaviour
{
    public static PlayerColliderDetect Instance { get; private set; }

    public float climbHeight = 2f;
    public float climbSpeed = 2f;
    public float photoSuccessDuration = 2f;

    public event Action OnPhotoSuccess;
    public event Action OnEnterMineArea;
    public event Action OnMineHit;

    private bool isInPhotoArea;
    private bool isInClimbArea;
    private bool isClimbing;
    private Vector3 climbStartPosition;
    private float climbProgress;
    private int meetMineCount = -1;

    public int MeetMineCount => meetMineCount;

    private PlayerController playerController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        PlayerInputManager pim = FindObjectOfType<PlayerInputManager>();
        if (pim != null)
        {
            pim.OnJumpOrClimb += HandleJumpOrClimb;
            pim.OnTryTakePhoto += HandleTryTakePhoto;
        }
    }

    private void OnDisable()
    {
        PlayerInputManager pim = FindObjectOfType<PlayerInputManager>();
        if (pim != null)
        {
            pim.OnJumpOrClimb -= HandleJumpOrClimb;
            pim.OnTryTakePhoto -= HandleTryTakePhoto;
        }
    }

    private void Update()
    {
        HandleClimbing();
        HandleMovementInClimbArea();
    }

    private void HandleJumpOrClimb()
    {
        if (isInClimbArea && !isClimbing)
        {
            StartClimbing();
        }
    }

    private void HandleTryTakePhoto()
    {
        if (isInPhotoArea)
        {
            OnPhotoSuccess?.Invoke();
            Debug.Log("拍照成功");
            StartCoroutine(PhotoSuccessCoroutine());
        }
    }

    private void StartClimbing()
    {
        isClimbing = true;
        climbStartPosition = transform.position;
        climbProgress = 0f;
        if (playerController != null)
        {
            playerController.isJumping = true;
        }
    }

    private void HandleClimbing()
    {
        if (isClimbing)
        {
            climbProgress += climbSpeed * Time.deltaTime;
            float currentHeight = Mathf.Min(climbProgress, climbHeight);
            transform.position = new Vector3(climbStartPosition.x, climbStartPosition.y + currentHeight, climbStartPosition.z);

            if (climbProgress >= climbHeight)
            {
                isClimbing = false;
                if (playerController != null)
                {
                    playerController.isJumping = false;
                }
            }
        }
    }

    private void HandleMovementInClimbArea()
    {
        if (isInClimbArea && !isClimbing)
        {
            // 禁用重力
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0;
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }

            // 检测AD输入，给玩家一个力让它掉落
            if (Input.GetKey(KeyCode.A))
            {
                if (rb != null)
                {
                    rb.gravityScale = 1;
                    rb.AddForce(Vector2.left * 5f, ForceMode2D.Impulse);
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {
                if (rb != null)
                {
                    rb.gravityScale = 1;
                    rb.AddForce(Vector2.right * 5f, ForceMode2D.Impulse);
                }
            }
        }
        else
        {
            // 恢复重力
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null && rb.gravityScale == 0)
            {
                rb.gravityScale = 1;
            }
        }
    }

    private IEnumerator PhotoSuccessCoroutine()
    {
        if (playerController != null)
        {
            playerController.isTakingPhotos = true;
        }
        yield return new WaitForSeconds(photoSuccessDuration);
        if (playerController != null)
        {
            playerController.isTakingPhotos = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        Debug.Log($"Player entered trigger: {tag}");

        if (tag == "PhotoArea")
        {
            isInPhotoArea = true;
        }
        else if (tag == "Climb")
        {
            isInClimbArea = true;
        }
        else if (tag == "PreMine")
        {
            meetMineCount++;
            Debug.Log($"即将进入地雷区，当前地雷计数: {meetMineCount}");
            // 先触发事件，再销毁物体
            OnEnterMineArea?.Invoke();
            // 延迟销毁，确保事件处理完成
            Destroy(collision.gameObject, 0.1f);
        }
        else if (tag == "Mine")
        {
            Debug.Log($"玩家碰到地雷！当前地雷计数: {meetMineCount}");
            // 触发地雷命中事件
            OnMineHit?.Invoke();
            // 广播玩家死亡到GM
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerDeath();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        string tag = collision.tag;
        Debug.Log($"Player exited trigger: {tag}");

        if (tag == "PhotoArea")
        {
            isInPhotoArea = false;
        }
        else if (tag == "Climb")
        {
            isInClimbArea = false;
            isClimbing = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        string tag = collision.tag;
        Debug.Log($"Player staying in trigger: {tag}");
    }
}
