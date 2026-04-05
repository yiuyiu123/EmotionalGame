using UnityEngine;

public class CameraPos : MonoBehaviour
{
    public Transform player;
    public float distance = 0f;
    public float yFollowFactor = 0.2f; // 玩家跳跃/下落时y移动的1/5
    public float normalFollowFactor = 1f; // 正常情况下y移动的1:1
    public float smoothSpeed = 5f; // 平滑过渡速度

    private PlayerController playerController;
    private PlayerColliderDetect playerColliderDetect;
    private float currentYFollowFactor; // 当前的y跟随比例
    private float jumpStartY; // 玩家起跳时的y位置
    private float cameraStartY; // 起跳时相机的y位置
    private bool isJumping = false; // 记录是否正在跳跃

    private void Start()
    {
        // 获取组件
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            playerColliderDetect = player.GetComponent<PlayerColliderDetect>();
            jumpStartY = player.position.y;
            cameraStartY = transform.position.y;
        }
        
        // 初始化跟随比例
        currentYFollowFactor = normalFollowFactor;
    }

    private void Update()
    {
        if (player != null)
        {
            // 计算目标相机位置
            Vector3 targetCameraPosition = new Vector3(
                player.position.x + distance,
                0,
                transform.position.z
            );
            
            // 检测玩家是否在跳跃
            bool currentJumping = playerController != null && playerController.isJumping;
            
            // 检测玩家是否在爬墙
            bool isClimbing = playerColliderDetect != null && playerColliderDetect.isClimbing;
            
            // 检测玩家是否在地面上
            bool isGrounded = playerController != null && playerController.isGrounded;
            
            // 检测是否在爬墙区域
            bool isInClimbArea = playerColliderDetect != null && playerColliderDetect.isInClimbArea;
            
            // 检测跳跃状态变化
            if (currentJumping && !isJumping)
            {
                // 开始跳跃，记录起跳位置
                jumpStartY = player.position.y;
                cameraStartY = transform.position.y;
                isJumping = true;
            }
            else if (!currentJumping && isJumping)
            {
                // 结束跳跃
                isJumping = false;
            }
            
            // 设置目标跟随比例
            float targetYFollowFactor;
            if (isClimbing || isInClimbArea)
            {
                // 爬墙或在爬墙区域内，使用1:1跟随
                targetYFollowFactor = normalFollowFactor;
            }
            else if (isGrounded)
            {
                // 在地面上，使用1:1跟随
                targetYFollowFactor = normalFollowFactor;
            }
            else
            {
                // 在空中（包括跳跃和离开爬墙区域后的下落），使用0.2倍跟随
                targetYFollowFactor = yFollowFactor;
            }
            
            // 平滑过渡到目标跟随比例
            currentYFollowFactor = Mathf.Lerp(currentYFollowFactor, targetYFollowFactor, smoothSpeed * Time.smoothDeltaTime);
            
            // 计算相机y位置
            float targetCameraY;
            if (isClimbing || isInClimbArea || isGrounded)
            {
                // 爬墙、在爬墙区域内或在地面上，1:1跟随
                targetCameraY = player.position.y * normalFollowFactor;
            }
            else
            {
                // 在空中，基于当前位置计算
                float yDistance = player.position.y - jumpStartY;
                targetCameraY = cameraStartY + yDistance * currentYFollowFactor;
            }
            
            // 设置目标相机的y位置
            targetCameraPosition.y = targetCameraY;
            
            // 平滑移动相机到目标位置
            // 使用Time.smoothDeltaTime可以获得更稳定的平滑效果
            transform.position = Vector3.Lerp(transform.position, targetCameraPosition, smoothSpeed * Time.smoothDeltaTime);
        }
    }
}
