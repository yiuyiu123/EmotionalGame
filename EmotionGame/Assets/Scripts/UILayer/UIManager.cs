using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class UIManager : MonoBehaviour
{
    public GameObject deathPanel;
    public GameObject photoPanel;
    public GameObject phonePanel; // 电话面板
    
    // 电话面板移动参数
    public float phonePanelMoveDistance = 100f; // 移动距离
    public float phonePanelMoveSpeed = 50f; // 移动速度
    
    private bool isPhonePanelMoving = false; // 电话面板是否正在移动
    private Vector3 phonePanelStartPos; // 电话面板起始位置
    private Vector3 phonePanelTargetPos; // 电话面板目标位置
    private bool isPhonePanelActive = false; // 电话面板是否激活

    private void Start()
    {
        // 初始设置为不显示
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }
        if (photoPanel != null)
        {
            photoPanel.SetActive(false);
        }
        if (phonePanel != null)
        {
            phonePanel.SetActive(false);
        }

        // 延迟订阅事件，确保所有管理器都已初始化
        StartCoroutine(SetupEventListenersWithRetry());
    }

    private IEnumerator SetupEventListenersWithRetry()
    {
        int retryCount = 0;
        while (retryCount < 5)
        {
            Debug.Log($"UIManager: 尝试订阅事件 (尝试 {retryCount + 1})");
            
            // 监听打电话事件
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.OnMakePhoneCall += HandleMakePhoneCall;
                Debug.Log("UIManager: 成功监听OnMakePhoneCall事件");
            }
            else
            {
                Debug.Log("UIManager: 警告 - PlayerController为null");
            }

            // 监听死亡事件
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerDeathEvent += HandlePlayerDeath;
                Debug.Log("UIManager: 成功监听OnPlayerDeathEvent事件");
            }
            else
            {
                Debug.Log("UIManager: 警告 - GameManager.Instance为null");
            }

            // 监听拍照成功事件
            if (PlayerColliderDetect.Instance != null)
            {
                PlayerColliderDetect.Instance.OnPhotoSuccess += HandlePhotoSuccess;
                Debug.Log("UIManager: 成功监听OnPhotoSuccess事件");
            }
            else
            {
                Debug.Log("UIManager: 警告 - PlayerColliderDetect.Instance为null");
            }

            // 检查是否所有必要的实例都已找到
            if (GameManager.Instance != null && PlayerColliderDetect.Instance != null)
            {
                Debug.Log("UIManager: 所有事件订阅完成");
                break;
            }

            retryCount++;
            yield return new WaitForSeconds(0.5f);
        }

        if (retryCount >= 5)
        {
            Debug.Log("UIManager: 警告 - 多次尝试后仍无法订阅所有事件");
        }
    }

    private void OnDisable()
    {
        // 取消监听打电话事件
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.OnMakePhoneCall -= HandleMakePhoneCall;
        }

        // 取消监听死亡事件
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDeathEvent -= HandlePlayerDeath;
        }

        // 取消监听拍照成功事件
        if (PlayerColliderDetect.Instance != null)
        {
            PlayerColliderDetect.Instance.OnPhotoSuccess -= HandlePhotoSuccess;
        }
    }

    private void HandleMakePhoneCall()
    {
        Debug.Log("UIManager: 处理打电话事件");
        
        if (phonePanel != null)
        {
            // 显示电话面板
            phonePanel.SetActive(true);
            
            // 记录起始位置和目标位置
            phonePanelStartPos = phonePanel.transform.localPosition;
            phonePanelTargetPos = phonePanelStartPos + new Vector3(0, phonePanelMoveDistance, 0);
            
            // 开始移动
            isPhonePanelMoving = true;
            isPhonePanelActive = true;
            
            Debug.Log($"UIManager: 电话面板开始移动 - 起始位置: {phonePanelStartPos}, 目标位置: {phonePanelTargetPos}");
        }
    }

    private void HandlePlayerDeath()
    {
        Debug.Log("UIManager: 接收到玩家死亡事件");
        if (deathPanel != null)
        {
            Debug.Log("UIManager: deathPanel不为null，设置为激活");
            deathPanel.SetActive(true);
            Debug.Log("UIManager: deathPanel激活状态: " + deathPanel.activeSelf);
            PlayVideoAndHide(deathPanel);
        }
        else
        {
            Debug.Log("UIManager: 警告 - deathPanel为null");
        }
    }

    private void HandlePhotoSuccess()
    {
        if (photoPanel != null)
        {
            photoPanel.SetActive(true);
            PlayVideoAndHide(photoPanel);
        }
    }

    private void PlayVideoAndHide(GameObject panel)
    {
        Debug.Log($"UIManager: 尝试播放面板视频: {panel.name}");
        
        // 判断是否需要重生（死亡面板需要重生，拍照面板不需要）
        bool shouldRespawn = panel == deathPanel;
        
        VideoPlayer videoPlayer = panel.GetComponentInChildren<VideoPlayer>();
        if (videoPlayer != null)
        {
            Debug.Log("UIManager: 找到VideoPlayer组件");
            
            if (videoPlayer.clip != null)
            {
                Debug.Log($"UIManager: 视频剪辑: {videoPlayer.clip.name}, 长度: {videoPlayer.clip.length}秒");
                videoPlayer.Play();
                StartCoroutine(WaitForVideoEnd(videoPlayer, panel, shouldRespawn));
            }
            else
            {
                Debug.Log("UIManager: 警告 - VideoPlayer没有视频剪辑");
                // 如果没有视频剪辑，直接隐藏
                StartCoroutine(HidePanelAfterDelay(panel, 2f, shouldRespawn));
            }
        }
        else
        {
            Debug.Log("UIManager: 警告 - 没有找到VideoPlayer组件");
            // 如果没有视频播放器，直接隐藏
            StartCoroutine(HidePanelAfterDelay(panel, 2f, shouldRespawn));
        }
    }

    private IEnumerator WaitForVideoEnd(VideoPlayer videoPlayer, GameObject panel, bool shouldRespawn = false)
    {
        Debug.Log("UIManager: 开始等待视频结束");
        
        // 禁用玩家输入
        PlayerInputManager playerInputManager = FindObjectOfType<PlayerInputManager>();
        if (playerInputManager != null)
        {
            playerInputManager.enabled = false;
            Debug.Log("UIManager: 已禁用玩家输入");
        }
        
        // 等待视频开始播放
        yield return new WaitUntil(() => videoPlayer.isPlaying);
        Debug.Log("UIManager: 视频开始播放");
        
        // 使用视频长度作为等待时间
        float videoLength = videoPlayer.clip != null ? (float)videoPlayer.clip.length : 5f;
        Debug.Log($"UIManager: 等待 {videoLength} 秒");
        
        yield return new WaitForSeconds(videoLength);
        Debug.Log("UIManager: 视频播放时间已到，隐藏面板");
        
        panel.SetActive(false);
        
        // 让角色重生到初始位置（仅在死亡时）
        if (shouldRespawn)
        {
            // 重置所有物体到初始状态
            if (RespawnManager.Instance != null)
            {
                RespawnManager.Instance.ResetAllToInitialState();
                Debug.Log("UIManager: 已重置所有物体到初始状态");
            }
            
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.Respawn();
                Debug.Log("UIManager: 角色已重生到初始位置");
            }
        }
        
        // 恢复玩家输入
        if (playerInputManager != null)
        {
            playerInputManager.enabled = true;
            Debug.Log("UIManager: 已恢复玩家输入");
        }
    }

    private IEnumerator HidePanelAfterDelay(GameObject panel, float delay, bool shouldRespawn = false)
    {
        // 禁用玩家输入
        PlayerInputManager playerInputManager = FindObjectOfType<PlayerInputManager>();
        if (playerInputManager != null)
        {
            playerInputManager.enabled = false;
            Debug.Log("UIManager: 已禁用玩家输入");
        }
        
        yield return new WaitForSeconds(delay);
        panel.SetActive(false);
        
        // 让角色重生到初始位置（仅在死亡时）
        if (shouldRespawn)
        {
            // 重置所有物体到初始状态
            if (RespawnManager.Instance != null)
            {
                RespawnManager.Instance.ResetAllToInitialState();
                Debug.Log("UIManager: 已重置所有物体到初始状态");
            }
            
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.Respawn();
                Debug.Log("UIManager: 角色已重生到初始位置");
            }
        }
        
        // 恢复玩家输入
        if (playerInputManager != null)
        {
            playerInputManager.enabled = true;
            Debug.Log("UIManager: 已恢复玩家输入");
        }
    }
    
    private void Update()
    {
        // 处理电话面板移动
        if (isPhonePanelMoving && phonePanel != null)
        {
            // 向目标位置移动
            phonePanel.transform.localPosition = Vector3.MoveTowards(
                phonePanel.transform.localPosition,
                phonePanelTargetPos,
                phonePanelMoveSpeed * Time.deltaTime
            );
            
            // 检查是否到达目标位置
            if (Vector3.Distance(phonePanel.transform.localPosition, phonePanelTargetPos) < 0.1f)
            {
                isPhonePanelMoving = false;
                Debug.Log("UIManager: 电话面板移动完成");
            }
        }
        
        // 监听玩家输入，关闭电话面板
        if (isPhonePanelActive && phonePanel != null)
        {
            // 检测任何输入
            if (Input.anyKeyDown)
            {
                ClosePhonePanel();
            }
        }
    }
    
    private void ClosePhonePanel()
    {
        if (phonePanel != null)
        {
            phonePanel.SetActive(false);
            isPhonePanelActive = false;
            isPhonePanelMoving = false;
            
            // 重置电话面板位置
            phonePanel.transform.localPosition = phonePanelStartPos;
            
            Debug.Log("UIManager: 电话面板已关闭");
        }
    }
}
