using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerController playerController;
    private PlayerColliderDetect playerColliderDetect;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        playerColliderDetect = GetComponent<PlayerColliderDetect>();
    }

    private void Update()
    {
        if (anim == null) return;

        // 走路/跑步速度控制
        float speed = 0f;
        if (playerController != null)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                speed = playerController.moveSpeed;
            }
        }
        anim.SetFloat("Speed", speed);

        // 跳跃状态控制
        bool isJumping = false;
        if (playerController != null)
        {
            isJumping = playerController.isJumping;
        }
        anim.SetBool("IsJumping", isJumping);

        // 攀爬状态控制
        bool isClimbing = false;
        if (playerColliderDetect != null)
        {
            isClimbing = playerColliderDetect.isClimbing;
        }
        anim.SetBool("IsClimbing", isClimbing);
    }

    public void TriggerRaise()
    {
        if (anim != null)
        {
            anim.SetBool("IsRaising", true);
        }
    }

    public void EndRaise()
    {
        if (anim != null)
        {
            anim.SetBool("IsRaising", false);
        }
    }

    public void OnJumpAnimationEnd()
    {
        if (playerController != null)
        {
            playerController.isJumping = false;
        }
    }

    public void OnStopAnimationEnd()
    {
    }

    public void OnClimbAnimationEnd()
    {
    }
}