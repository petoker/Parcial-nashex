using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;                        // Follow player
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10); // Distance camera-player
    [SerializeField] private Vector2 deadZoneSize = new Vector2(2f, 1f); // Width/height of the “dead zone”

    private Vector3 currentTarget; // Center of the dead-zone (in world space)

    void Start()
    {
        // At start, center the dead-zone on the player (if exists)
        currentTarget = player ? (Vector3)player.position : transform.position - offset;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Horizontal limits of the dead-zone
        float leftLimit = currentTarget.x - deadZoneSize.x * 0.5f;
        float rightLimit = currentTarget.x + deadZoneSize.x * 0.5f;

        // If the player exits left/right, move the dead-zone center
        if (player.position.x < leftLimit)
            currentTarget.x = player.position.x + deadZoneSize.x * 0.5f;
        else if (player.position.x > rightLimit)
            currentTarget.x = player.position.x - deadZoneSize.x * 0.5f;

        // Vertical limits of the dead-zone
        float bottomLimit = currentTarget.y - deadZoneSize.y * 0.5f;
        float topLimit = currentTarget.y + deadZoneSize.y * 0.5f;

        // If the player exits bottom/top, move the dead-zone center
        if (player.position.y < bottomLimit)
            currentTarget.y = player.position.y + deadZoneSize.y * 0.5f;
        else if (player.position.y > topLimit)
            currentTarget.y = player.position.y - deadZoneSize.y * 0.5f;

        // Final position: dead-zone center + offset (without shake)
        transform.position = currentTarget + offset;
    }
}