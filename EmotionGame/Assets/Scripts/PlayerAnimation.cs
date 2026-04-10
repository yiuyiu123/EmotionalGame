using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerController playerController;
    private PlayerColliderDetect playerColliderDetect;

    private void Start()
    {
        // 获取组件
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        playerColliderDetect = GetComponent<PlayerColliderDetect>();
        
        // 初始化动画参数，确保初始状态正确
        ResetAnimationParameters();
    }

    private void Update()
    {
        // 更新速度参数（控制走路/跑步）
        if (playerController != null)
        {
            // 检测左右移动输入
            bool isMovingLeft = Input.GetKey(KeyCode.A);
            bool isMovingRight = Input.GetKey(KeyCode.D);
            
            // 根据输入设置速度
            float speed = 0f;
            if (isMovingLeft || isMovingRight)
            {
                speed = playerController.moveSpeed; // 使用玩家移动速度
            }
            
            anim.SetFloat("Speed", speed);
        }
        else if (rb != null)
        {
            // 备用方案：使用rigidbody速度
            float speed = Mathf.Abs(rb.velocity.x);
            anim.SetFloat("Speed", speed);
        }

        // 更新跳跃状态
        if (playerController != null)
        {
            anim.SetBool("IsJumping", playerController.isJumping);
        }
        else
        {
            // 确保playerController为null时，跳跃状态为false
            anim.SetBool("IsJumping", false);
        }

        // 更新攀爬状态
        if (playerColliderDetect != null)
        {
            anim.SetBool("IsClimbing", playerColliderDetect.isClimbing);
        }
        else
        {
            // 确保playerColliderDetect为null时，攀爬状态为false
            anim.SetBool("IsClimbing", false);
        }
        
        // 更新投降状态
        // 注意：这里不需要实时更新，因为投降是通过TriggerRaise和EndRaise方法控制的
    }

    // 初始化动画参数
    private void ResetAnimationParameters()
    {
        if (anim != null)
        {
            // 确保所有状态参数初始为false
            anim.SetFloat("Speed", 0f);
            anim.SetBool("IsJumping", false);
            anim.SetBool("IsClimbing", false);
            anim.SetBool("IsRaising", false);
        }
    }

    // 触发投降动画
    public void TriggerRaise()
    {
        anim.SetBool("IsRaising", true);
    }

    // 结束投降动画
    public void EndRaise()
    {
        anim.SetBool("IsRaising", false);
    }

    // 跳跃动画结束事件（在动画中添加事件调用）
    public void OnJumpAnimationEnd()
    {
        if (playerController != null)
        {
            playerController.isJumping = false;
        }
    }

    // 停止动画结束事件（在动画中添加事件调用）
    public void OnStopAnimationEnd()
    {
        // 可以在这里添加停止动画结束后的逻辑
    }

    // 攀爬动画结束事件（在动画中添加事件调用）
    public void OnClimbAnimationEnd()
    {
        if (playerColliderDetect != null)
        {
            // 确保攀爬状态正确重置
        }
    }
}