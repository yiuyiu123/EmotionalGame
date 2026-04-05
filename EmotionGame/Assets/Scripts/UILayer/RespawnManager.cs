using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }
    
    // 保存Pre-类物体的初始状态
    private Dictionary<GameObject, bool> preObjectInitialStates = new Dictionary<GameObject, bool>();
    
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
    }
    
    private void Start()
    {
        // 查找所有Pre-类物体并保存初始状态
        SavePreObjectInitialStates();
        
        Debug.Log("RespawnManager: 初始化完成");
    }
    
    private void SavePreObjectInitialStates()
    {
        // 查找所有PreMine物体
        GameObject[] preMines = GameObject.FindGameObjectsWithTag("PreMine");
        foreach (GameObject preMine in preMines)
        {
            preObjectInitialStates[preMine] = preMine.activeSelf;
            Debug.Log($"RespawnManager: 保存PreMine初始状态 - {preMine.name}: {preMine.activeSelf}");
        }
        
        // 查找所有PreDrop物体
        GameObject[] preDrops = GameObject.FindGameObjectsWithTag("PreDrop");
        foreach (GameObject preDrop in preDrops)
        {
            preObjectInitialStates[preDrop] = preDrop.activeSelf;
            Debug.Log($"RespawnManager: 保存PreDrop初始状态 - {preDrop.name}: {preDrop.activeSelf}");
        }
        
        // 查找所有PreEnemyGun物体
        GameObject[] preEnemyGuns = GameObject.FindGameObjectsWithTag("PreEnemyGun");
        foreach (GameObject preEnemyGun in preEnemyGuns)
        {
            preObjectInitialStates[preEnemyGun] = preEnemyGun.activeSelf;
            Debug.Log($"RespawnManager: 保存PreEnemyGun初始状态 - {preEnemyGun.name}: {preEnemyGun.activeSelf}");
        }
        
        // 查找所有PreFriendGun物体
        GameObject[] preFriendGuns = GameObject.FindGameObjectsWithTag("PreFriendGun");
        foreach (GameObject preFriendGun in preFriendGuns)
        {
            preObjectInitialStates[preFriendGun] = preFriendGun.activeSelf;
            Debug.Log($"RespawnManager: 保存PreFriendGun初始状态 - {preFriendGun.name}: {preFriendGun.activeSelf}");
        }
        
        Debug.Log($"RespawnManager: 共保存 {preObjectInitialStates.Count} 个Pre-类物体的初始状态");
    }
    
    public void ResetAllToInitialState()
    {
        Debug.Log("RespawnManager: 开始重置所有物体到初始状态");
        
        // 重置所有控制器
        ResetControllers();
        
        // 重置所有Pre-类物体
        ResetPreObjects();
        
        Debug.Log("RespawnManager: 所有物体重置完成");
    }
    
    private void ResetControllers()
    {
        // 重置MineController
        MineController mineController = FindObjectOfType<MineController>();
        if (mineController != null)
        {
            mineController.ResetToInitialState();
        }
        
        // 重置DropController
        DropController dropController = FindObjectOfType<DropController>();
        if (dropController != null)
        {
            dropController.ResetToInitialState();
        }
        
        // 重置GunController
        GunController gunController = FindObjectOfType<GunController>();
        if (gunController != null)
        {
            gunController.ResetToInitialState();
        }
        
        // 重置PlayerColliderDetect
        if (PlayerColliderDetect.Instance != null)
        {
            PlayerColliderDetect.Instance.ResetToInitialState();
        }
    }
    
    private void ResetPreObjects()
    {
        // 重置所有Pre-类物体的激活状态
        foreach (var kvp in preObjectInitialStates)
        {
            GameObject obj = kvp.Key;
            bool initialState = kvp.Value;
            
            if (obj != null)
            {
                obj.SetActive(initialState);
                Debug.Log($"RespawnManager: 重置Pre-类物体 - {obj.name}: {initialState}");
            }
        }
    }
}
