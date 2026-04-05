using System.Collections;
using UnityEngine;

public class MineController : MonoBehaviour
{
    public GameObject[] mines;
    public float riseDistance = 0.5f;
    public float riseDuration = 0.5f;

    private void Start()
    {
        // 初始化所有地雷为不显示，并添加Tag
        foreach (GameObject mine in mines)
        {
            if (mine != null)
            {
                mine.SetActive(false);
                mine.tag = "Mine";
                Debug.Log($"设置地雷Tag: {mine.name}");
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
            PlayerColliderDetect.Instance.OnEnterMineArea += HandleEnterMineArea;
            PlayerColliderDetect.Instance.OnMineHit += HandleMineHit;
            Debug.Log("MineController: 成功监听OnEnterMineArea和OnMineHit事件");
        }
        else
        {
            Debug.Log("MineController: 警告 - PlayerColliderDetect.Instance为null");
        }
    }

    private void OnDisable()
    {
        if (PlayerColliderDetect.Instance != null)
        {
            PlayerColliderDetect.Instance.OnEnterMineArea -= HandleEnterMineArea;
            PlayerColliderDetect.Instance.OnMineHit -= HandleMineHit;
        }
    }

    private void HandleEnterMineArea()
    {
        int mineCount = PlayerColliderDetect.Instance.MeetMineCount;
        if (mineCount >= 0 && mineCount < mines.Length)
        {
            GameObject mine = mines[mineCount];
            if (mine != null)
            {
                mine.SetActive(true);
                // 启动上升动画协程
                StartCoroutine(RiseAnimation(mine));
                Debug.Log($"显示地雷: {mineCount}，上升距离: {riseDistance}，动画时长: {riseDuration}");
            }
        }
    }

    private IEnumerator RiseAnimation(GameObject mine)
    {
        if (mine == null)
        {
            yield break;
        }

        Vector3 startPosition = mine.transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, riseDistance, 0);
        float elapsedTime = 0f;

        while (elapsedTime < riseDuration && mine != null)
        {
            float t = elapsedTime / riseDuration;
            // 使用平滑的插值
            mine.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最终位置准确
        if (mine != null)
        {
            mine.transform.position = endPosition;
        }
    }

    private void HandleMineHit()
    {
        int mineCount = PlayerColliderDetect.Instance.MeetMineCount;
        if (mineCount >= 0 && mineCount < mines.Length)
        {
            GameObject mine = mines[mineCount];
            if (mine != null)
            {
                Destroy(mine);
                Debug.Log($"销毁地雷: {mineCount}");
            }
        }
    }
}
