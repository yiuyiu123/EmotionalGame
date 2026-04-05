using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // 暴露的插槽
    public GameObject[] enemyGuns;
    public GameObject[] friendGuns;
    public Transform[] enemyGunPivots;
    public Transform[] friendGunPivots;
    
    // 暴露的参数
    public float blinkFrequency = 0.5f; // 闪烁频率
    public float rotationSpeed = 100f; // 旋转速度
    public float maxRotationAngle = 45f; // 最大旋转角度
    public float surrenderTimeLimit = 1f; // 投降时间限制
    
    // 内部变量
    private int meetEnemyGunCount = -1;
    private int meetFriendGunCount = -1;
    private bool isInFriendGunArea = false;
    private float surrenderTimer = 0f;
    private int currentFriendGunIndex = -1;
    
    // 旋转状态
    private Dictionary<GameObject, float> gunRotationAngles = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, bool> gunRotationDirections = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, float> gunBlinkTimers = new Dictionary<GameObject, float>();
    
    // 保存初始状态
    private Vector3[] initialEnemyGunPositions;
    private Quaternion[] initialEnemyGunRotations;
    private bool[] initialEnemyGunActiveStates;
    private Vector3[] initialFriendGunPositions;
    private Quaternion[] initialFriendGunRotations;
    private bool[] initialFriendGunActiveStates;
    
    private void Start()
    {
        // 初始化
        InitializeGuns();
        
        // 订阅事件
        if (PlayerColliderDetect.Instance != null)
        {
            PlayerColliderDetect.Instance.OnEnterEnemyGunArea += HandleEnterEnemyGunArea;
            PlayerColliderDetect.Instance.OnEnterFriendGunArea += HandleEnterFriendGunArea;
        }
    }
    
    private void OnDisable()
    {
        // 取消订阅事件
        if (PlayerColliderDetect.Instance != null)
        {
            PlayerColliderDetect.Instance.OnEnterEnemyGunArea -= HandleEnterEnemyGunArea;
            PlayerColliderDetect.Instance.OnEnterFriendGunArea -= HandleEnterFriendGunArea;
        }
    }
    
    private void InitializeGuns()
    {
        // 初始化敌方枪支
        if (enemyGuns != null)
        {
            // 初始化状态数组
            initialEnemyGunPositions = new Vector3[enemyGuns.Length];
            initialEnemyGunRotations = new Quaternion[enemyGuns.Length];
            initialEnemyGunActiveStates = new bool[enemyGuns.Length];
            
            for (int i = 0; i < enemyGuns.Length; i++)
            {
                if (enemyGuns[i] != null)
                {
                    // 保存初始状态
                    initialEnemyGunPositions[i] = enemyGuns[i].transform.position;
                    initialEnemyGunRotations[i] = enemyGuns[i].transform.rotation;
                    initialEnemyGunActiveStates[i] = enemyGuns[i].activeSelf;
                    
                    // 设置初始状态
                    enemyGuns[i].tag = "EnemyGun";
                    enemyGuns[i].SetActive(true);
                    gunRotationAngles[enemyGuns[i]] = 0f;
                    gunRotationDirections[enemyGuns[i]] = true; // true = 顺时针
                    gunBlinkTimers[enemyGuns[i]] = 0f;
                    
                    Debug.Log($"GunController: 初始化敌方枪支 {i} - 位置: {initialEnemyGunPositions[i]}, 旋转: {initialEnemyGunRotations[i]}, 激活状态: {initialEnemyGunActiveStates[i]}");
                }
            }
        }
        
        // 初始化友方枪支
        if (friendGuns != null)
        {
            // 初始化状态数组
            initialFriendGunPositions = new Vector3[friendGuns.Length];
            initialFriendGunRotations = new Quaternion[friendGuns.Length];
            initialFriendGunActiveStates = new bool[friendGuns.Length];
            
            for (int i = 0; i < friendGuns.Length; i++)
            {
                if (friendGuns[i] != null)
                {
                    // 保存初始状态
                    initialFriendGunPositions[i] = friendGuns[i].transform.position;
                    initialFriendGunRotations[i] = friendGuns[i].transform.rotation;
                    initialFriendGunActiveStates[i] = friendGuns[i].activeSelf;
                    
                    // 设置初始状态
                    friendGuns[i].tag = "FriendGun";
                    friendGuns[i].SetActive(true);
                    gunRotationAngles[friendGuns[i]] = 0f;
                    gunRotationDirections[friendGuns[i]] = true; // true = 顺时针
                    gunBlinkTimers[friendGuns[i]] = 0f;
                    
                    Debug.Log($"GunController: 初始化友方枪支 {i} - 位置: {initialFriendGunPositions[i]}, 旋转: {initialFriendGunRotations[i]}, 激活状态: {initialFriendGunActiveStates[i]}");
                }
            }
        }
    }
    
    private void Update()
    {
        // 处理枪支旋转和闪烁
        UpdateGunEffects();
        
        // 处理友军枪支投降逻辑
        UpdateSurrenderLogic();
    }
    
    private void UpdateGunEffects()
    {
        // 处理敌方枪支
        if (enemyGuns != null)
        {
            for (int i = 0; i <= meetEnemyGunCount && i < enemyGuns.Length; i++)
            {
                GameObject gun = enemyGuns[i];
                if (gun != null)
                {
                    // 闪烁效果
                    UpdateGunBlink(gun);
                    
                    // 旋转效果
                    UpdateGunRotation(gun, enemyGunPivots[i]);
                }
            }
        }
        
        // 处理友方枪支
        if (friendGuns != null)
        {
            for (int i = 0; i <= meetFriendGunCount && i < friendGuns.Length; i++)
            {
                GameObject gun = friendGuns[i];
                if (gun != null)
                {
                    // 闪烁效果
                    UpdateGunBlink(gun);
                    
                    // 旋转效果
                    UpdateGunRotation(gun, friendGunPivots[i]);
                }
            }
        }
    }
    
    private void UpdateGunBlink(GameObject gun)
    {
        gunBlinkTimers[gun] += Time.deltaTime;
        if (gunBlinkTimers[gun] >= blinkFrequency)
        {
            gun.SetActive(!gun.activeSelf);
            gunBlinkTimers[gun] = 0f;
        }
    }
    
    private void UpdateGunRotation(GameObject gun, Transform pivot)
    {
        if (pivot != null)
        {
            // 更新旋转角度
            float rotationDirection = gunRotationDirections[gun] ? 1f : -1f;
            gunRotationAngles[gun] += rotationDirection * rotationSpeed * Time.deltaTime;
            
            // 检查是否到达最大角度
            if (Mathf.Abs(gunRotationAngles[gun]) >= maxRotationAngle)
            {
                gunRotationDirections[gun] = !gunRotationDirections[gun];
                gunRotationAngles[gun] = Mathf.Sign(gunRotationAngles[gun]) * maxRotationAngle;
            }
            
            // 应用旋转
            gun.transform.RotateAround(pivot.position, Vector3.forward, rotationDirection * rotationSpeed * Time.deltaTime);
        }
    }
    
    private void UpdateSurrenderLogic()
    {
        if (isInFriendGunArea)
        {
            surrenderTimer += Time.deltaTime;
            
            // 检查是否超时
            if (surrenderTimer >= surrenderTimeLimit)
            {
                SurrenderFailed();
            }
            
            // 检查是否按下左键
            if (Input.GetMouseButtonDown(0))
            {
                SurrenderSuccess();
            }
        }
    }
    
    private void HandleEnterEnemyGunArea()
    {
        meetEnemyGunCount++;
        Debug.Log($"即将进入敌方战火区，当前计数: {meetEnemyGunCount}");
    }
    
    private void HandleEnterFriendGunArea()
    {
        meetFriendGunCount++;
        Debug.Log($"即将进入友军战火区，当前计数: {meetFriendGunCount}");
    }
    
    public void OnPlayerEnterFriendGunArea(int gunIndex)
    {
        isInFriendGunArea = true;
        surrenderTimer = 0f;
        currentFriendGunIndex = gunIndex;
        Debug.Log($"玩家进入友军枪区域，开始投降计时: {surrenderTimeLimit}秒");
    }
    
    public void OnPlayerExitFriendGunArea()
    {
        isInFriendGunArea = false;
        surrenderTimer = 0f;
        currentFriendGunIndex = -1;
        Debug.Log("玩家离开友军枪区域，投降计时重置");
    }
    
    private void SurrenderSuccess()
    {
        Debug.Log("投降成功！");
        
        // 隐藏对应的友军枪
        if (currentFriendGunIndex >= 0 && currentFriendGunIndex < friendGuns.Length)
        {
            GameObject gun = friendGuns[currentFriendGunIndex];
            if (gun != null)
            {
                gun.SetActive(false);
                Debug.Log($"友军枪 {currentFriendGunIndex} 已隐藏");
            }
        }
        
        // 重置状态
        isInFriendGunArea = false;
        surrenderTimer = 0f;
        currentFriendGunIndex = -1;
        
        // 这里可以添加玩家举手动画的调用
        // playerController.PlayRaiseAnimation();
    }
    
    private void SurrenderFailed()
    {
        Debug.Log("投降失败！");
        
        // 广播玩家死亡到GM
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDeath();
        }
        
        // 重置状态
        isInFriendGunArea = false;
        surrenderTimer = 0f;
        currentFriendGunIndex = -1;
    }
    
    public void OnEnemyGunHit(int gunIndex)
    {
        // 隐藏对应的敌方枪
        if (gunIndex >= 0 && gunIndex < enemyGuns.Length)
        {
            GameObject gun = enemyGuns[gunIndex];
            if (gun != null)
            {
                gun.SetActive(false);
                Debug.Log($"敌方枪 {gunIndex} 已隐藏");
            }
        }
        
        // 广播玩家死亡到GM
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDeath();
        }
    }
    
    // 重置到初始状态
    public void ResetToInitialState()
    {
        Debug.Log("GunController: 开始重置到初始状态");
        
        // 重置敌方枪支
        if (enemyGuns != null)
        {
            for (int i = 0; i < enemyGuns.Length; i++)
            {
                if (enemyGuns[i] != null)
                {
                    // 恢复初始位置
                    enemyGuns[i].transform.position = initialEnemyGunPositions[i];
                    // 恢复初始旋转
                    enemyGuns[i].transform.rotation = initialEnemyGunRotations[i];
                    // 恢复初始激活状态
                    enemyGuns[i].SetActive(initialEnemyGunActiveStates[i]);
                    // 重置旋转角度
                    gunRotationAngles[enemyGuns[i]] = 0f;
                    gunRotationDirections[enemyGuns[i]] = true;
                    gunBlinkTimers[enemyGuns[i]] = 0f;
                    
                    Debug.Log($"GunController: 重置敌方枪支 {i} - 位置: {initialEnemyGunPositions[i]}, 旋转: {initialEnemyGunRotations[i]}, 激活状态: {initialEnemyGunActiveStates[i]}");
                }
            }
        }
        
        // 重置友方枪支
        if (friendGuns != null)
        {
            for (int i = 0; i < friendGuns.Length; i++)
            {
                if (friendGuns[i] != null)
                {
                    // 恢复初始位置
                    friendGuns[i].transform.position = initialFriendGunPositions[i];
                    // 恢复初始旋转
                    friendGuns[i].transform.rotation = initialFriendGunRotations[i];
                    // 恢复初始激活状态
                    friendGuns[i].SetActive(initialFriendGunActiveStates[i]);
                    // 重置旋转角度
                    gunRotationAngles[friendGuns[i]] = 0f;
                    gunRotationDirections[friendGuns[i]] = true;
                    gunBlinkTimers[friendGuns[i]] = 0f;
                    
                    Debug.Log($"GunController: 重置友方枪支 {i} - 位置: {initialFriendGunPositions[i]}, 旋转: {initialFriendGunRotations[i]}, 激活状态: {initialFriendGunActiveStates[i]}");
                }
            }
        }
        
        // 重置计数器
        meetEnemyGunCount = -1;
        meetFriendGunCount = -1;
        
        // 重置投降状态
        isInFriendGunArea = false;
        surrenderTimer = 0f;
        currentFriendGunIndex = -1;
        
        Debug.Log("GunController: 重置完成");
    }
}