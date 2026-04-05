using UnityEngine;

public class CameraPos : MonoBehaviour
{
    public Transform player;
    public float distance = 0f;
    public float yFollowFactor = 0.2f; // 玩家y移动的1/5

    private void Update()
    {
        if (player != null)
        {
            Vector3 cameraPosition = transform.position;
            cameraPosition.x = player.position.x + distance;
            cameraPosition.y = player.position.y * yFollowFactor;
            transform.position = cameraPosition;
        }
    }
}
