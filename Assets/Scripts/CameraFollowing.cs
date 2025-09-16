using UnityEngine;
using System.Collections;

public class CameraFollowing : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;
    public Vector3 offset = new Vector3(0, 0, -10);
    public Vector2 deadZoneSize = new Vector2(2f, 1f);

    [Header("Shake Settings")]
    public AnimationCurve shakeFalloff = AnimationCurve.EaseInOut(0, 1, 1, 0);
    public float defaultShakeDuration = 0.2f;
    public float defaultShakeMagnitude = 0.3f;

    private Vector3 shakeOffset;
    private Vector3 currentTarget;

    void LateUpdate()
    {
        if (player == null) return;

        // Dead zone horizontal
        float leftLimit = currentTarget.x - deadZoneSize.x / 2;
        float rightLimit = currentTarget.x + deadZoneSize.x / 2;
        if (player.position.x < leftLimit) currentTarget.x = player.position.x + deadZoneSize.x / 2;
        else if (player.position.x > rightLimit) currentTarget.x = player.position.x - deadZoneSize.x / 2;

        // Dead zone vertical
        float bottomLimit = currentTarget.y - deadZoneSize.y / 2;
        float topLimit = currentTarget.y + deadZoneSize.y / 2;
        if (player.position.y < bottomLimit) currentTarget.y = player.position.y + deadZoneSize.y / 2;
        else if (player.position.y > topLimit) currentTarget.y = player.position.y - deadZoneSize.y / 2;

        // Posición final = offset + shake
        transform.position = currentTarget + offset + shakeOffset;
    }

    public void Shake(float duration, float magnitude)
    {
        StopAllCoroutines();
        StartCoroutine(DoShake(duration, magnitude));
    }

    public void ShakeDefault()
    {
        Shake(defaultShakeDuration, defaultShakeMagnitude);
    }

    IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float strength = shakeFalloff.Evaluate(elapsed / duration);
            shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * magnitude * strength,
                Random.Range(-1f, 1f) * magnitude * strength,
                0
            );

            elapsed += Time.deltaTime;
            yield return null;
        }
        shakeOffset = Vector3.zero;
    }
}