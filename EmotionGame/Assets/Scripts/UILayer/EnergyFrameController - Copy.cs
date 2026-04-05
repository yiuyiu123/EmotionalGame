using System;
using UnityEngine;

public class EnergyFrameController : MonoBehaviour
{
    public GameObject energy;
    public float initialLength = 1f;
    public float photoAdd = 0.2f;
    public float reduceSpeed = 0.01f;
    public float deathLine = 0.1f;

    private RectTransform energyRectTransform;
    private bool isPlayerDead;


    private void Start()
    {
        if (energy != null)
        {
            energyRectTransform = energy.GetComponent<RectTransform>();
            if (energyRectTransform != null)
            {
                Vector3 scale = energyRectTransform.localScale;
                scale.x = initialLength;
                energyRectTransform.localScale = scale;
            }
        }

        // 监听拍照成功事件
        if (PlayerColliderDetect.Instance != null)
        {
            PlayerColliderDetect.Instance.OnPhotoSuccess += HandlePhotoSuccess;
        }
    }

    private void Update()
    {
        if (energyRectTransform != null)
        {
            Vector3 scale = energyRectTransform.localScale;
            scale.x = Mathf.Max(0, scale.x - reduceSpeed * Time.deltaTime);
            energyRectTransform.localScale = scale;

            if (scale.x <= deathLine && !isPlayerDead)
            {
                isPlayerDead = true;
                // 广播玩家死亡
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnPlayerDeath();
                }
            }
        }
    }

    private void HandlePhotoSuccess()
    {
        if (energyRectTransform != null)
        {
            Vector3 scale = energyRectTransform.localScale;
            scale.x = Mathf.Min(1, scale.x + photoAdd);
            energyRectTransform.localScale = scale;
        }
    }

    private void OnDisable()
    {
        if (PlayerColliderDetect.Instance != null)
        {
            PlayerColliderDetect.Instance.OnPhotoSuccess -= HandlePhotoSuccess;
        }
    }
}
