using System.Collections;
using UnityEngine;

public class DropController : MonoBehaviour
{
    public GameObject[] drops;
    public float initialLength = 0.1f;
    public float scaleSpeed = 0.1f;
    public float maxLength = 1.0f;
    
    // 保存初始状态
    private Vector3[] initialPositions;
    private Vector3[] initialScales;
    private bool[] initialActiveStates;

    private void Start()
    {
        // 初始化状态数组
        initialPositions = new Vector3[drops.Length];
        initialScales = new Vector3[drops.Length];
        initialActiveStates = new bool[drops.Length];
        
        // 初始化所有掉落物为不显示，并添加Tag，同时保存初始状态
        for (int i = 0; i < drops.Length; i++)
        {
            if (drops[i] != null)
            {
                // 先设置初始状态
                drops[i].SetActive(false);
                drops[i].tag = "Drop";
                // 初始化缩放为初始长度
                Vector3 scale = drops[i].transform.localScale;
                scale.x = initialLength;
                drops[i].transform.localScale = scale;
                
                // 再保存初始状态（游戏开始时的状态）
                initialPositions[i] = drops[i].transform.position;
                initialScales[i] = drops[i].transform.localScale;
                initialActiveStates[i] = drops[i].activeSelf;
                
                Debug.Log($"设置掉落物Tag: {drops[i].name}, 初始位置: {initialPositions[i]}, 初始缩放: {initialScales[i]}, 初始激活状态: {initialActiveStates[i]}");
            }
        }

        // 确保监听事件
        SetupEventListeners();
    }

    private void OnEnable()
    {
        SetupEventListeners();
    }

    private void SetupEventListeners()
    {
        if (PlayerColliderDetect.Instance != null)
        {
            PlayerColliderDetect.Instance.OnEnterDropArea += HandleEnterDropArea;
            PlayerColliderDetect.Instance.OnDropHit += HandleDropHit;
            Debug.Log("DropController: 成功监听OnEnterDropArea和OnDropHit事件");
        }
        else
        {
            Debug.Log("DropController: 警告 - PlayerColliderDetect.Instance为null");
        }
    }

    private void OnDisable()
    {
        if (PlayerColliderDetect.Instance != null)
        {
            PlayerColliderDetect.Instance.OnEnterDropArea -= HandleEnterDropArea;
            PlayerColliderDetect.Instance.OnDropHit -= HandleDropHit;
        }
    }

    private void HandleEnterDropArea()
    {
        int dropCount = PlayerColliderDetect.Instance.MeetDropCount;
        if (dropCount >= 0 && dropCount < drops.Length)
        {
            GameObject drop = drops[dropCount];
            if (drop != null)
            {
                drop.SetActive(true);
                // 启动缩放动画协程
                StartCoroutine(ScaleAnimation(drop));
                Debug.Log($"显示掉落物: {dropCount}，初始长度: {initialLength}，缩放速度: {scaleSpeed}，最大长度: {maxLength}");
            }
        }
    }

    private IEnumerator ScaleAnimation(GameObject drop)
    {
        if (drop == null)
        {
            yield break;
        }

        while (drop != null)
        {
            Vector3 scale = drop.transform.localScale;
            if (scale.x >= maxLength)
            {
                // 达到最大长度，停止缩放
                scale.x = maxLength;
                drop.transform.localScale = scale;
                break;
            }

            // 按速度增加缩放
            scale.x += scaleSpeed * Time.deltaTime;
            drop.transform.localScale = scale;
            yield return null;
        }
    }

    private void HandleDropHit()
    {
        int dropCount = PlayerColliderDetect.Instance.MeetDropCount;
        if (dropCount >= 0 && dropCount < drops.Length)
        {
            GameObject drop = drops[dropCount];
            if (drop != null)
            {
                drop.SetActive(false);
                Debug.Log($"隐藏掉落物: {dropCount}");
            }
        }
    }
    
    // 重置到初始状态
    public void ResetToInitialState()
    {
        Debug.Log("DropController: 开始重置到初始状态");
        
        for (int i = 0; i < drops.Length; i++)
        {
            if (drops[i] != null)
            {
                // 恢复初始位置
                drops[i].transform.position = initialPositions[i];
                // 恢复初始缩放
                drops[i].transform.localScale = initialScales[i];
                // 恢复初始激活状态
                drops[i].SetActive(initialActiveStates[i]);
                Debug.Log($"DropController: 重置掉落物 {i} - 位置: {initialPositions[i]}, 缩放: {initialScales[i]}, 激活状态: {initialActiveStates[i]}");
            }
        }
        
        Debug.Log("DropController: 重置完成");
    }
}
