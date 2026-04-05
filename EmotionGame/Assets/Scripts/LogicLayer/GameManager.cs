using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action OnPlayerDeathEvent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnPlayerDeath()
    {
        
        Debug.Log("GameManager: OnPlayerDeathEvent 事件监听器数量: " + (OnPlayerDeathEvent?.GetInvocationList().Length ?? 0));
        OnPlayerDeathEvent?.Invoke();
        Debug.Log("GameManager: OnPlayerDeathEvent 事件触发完成");
    }
}
