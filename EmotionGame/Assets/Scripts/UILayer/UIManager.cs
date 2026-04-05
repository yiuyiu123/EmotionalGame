using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class UIManager : MonoBehaviour
{
    public GameObject deathPanel;
    public GameObject photoPanel;

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
        VideoPlayer videoPlayer = panel.GetComponentInChildren<VideoPlayer>();
        if (videoPlayer != null)
        {
            videoPlayer.Play();
            StartCoroutine(WaitForVideoEnd(videoPlayer, panel));
        }
        else
        {
            // 如果没有视频播放器，直接隐藏
            StartCoroutine(HidePanelAfterDelay(panel, 2f));
        }
    }

    private IEnumerator WaitForVideoEnd(VideoPlayer videoPlayer, GameObject panel)
    {
        yield return new WaitUntil(() => !videoPlayer.isPlaying);
        panel.SetActive(false);
    }

    private IEnumerator HidePanelAfterDelay(GameObject panel, float delay)
    {
        yield return new WaitForSeconds(delay);
        panel.SetActive(false);
    }
}
