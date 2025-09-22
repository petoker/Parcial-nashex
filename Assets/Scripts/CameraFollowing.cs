using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;                       // A qui�n seguir
    public Vector3 offset = new Vector3(0, 0, -10); // Distancia c�mara�player (z negativo en 2D)
    public Vector2 deadZoneSize = new Vector2(2f, 1f); // Ancho/alto de la �zona muerta�

    private Vector3 currentTarget; // Centro de la dead-zone (en mundo)

    void Start()
    {
        // Al iniciar, centra la dead-zone sobre el jugador (si existe)
        currentTarget = player ? (Vector3)player.position : transform.position - offset;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // L�mites horizontales de la dead-zone
        float leftLimit = currentTarget.x - deadZoneSize.x * 0.5f;
        float rightLimit = currentTarget.x + deadZoneSize.x * 0.5f;

        // Si el player sale por izquierda/derecha, movemos el centro de la dead-zone
        if (player.position.x < leftLimit)
            currentTarget.x = player.position.x + deadZoneSize.x * 0.5f;
        else if (player.position.x > rightLimit)
            currentTarget.x = player.position.x - deadZoneSize.x * 0.5f;

        // L�mites verticales de la dead-zone
        float bottomLimit = currentTarget.y - deadZoneSize.y * 0.5f;
        float topLimit = currentTarget.y + deadZoneSize.y * 0.5f;

        // Si el player sale por abajo/arriba, movemos el centro de la dead-zone
        if (player.position.y < bottomLimit)
            currentTarget.y = player.position.y + deadZoneSize.y * 0.5f;
        else if (player.position.y > topLimit)
            currentTarget.y = player.position.y - deadZoneSize.y * 0.5f;

        // Posici�n final: centro de la dead-zone + offset (sin temblor)
        transform.position = currentTarget + offset;

        // (Opcional: suavizado)
        // transform.position = Vector3.Lerp(transform.position, currentTarget + offset, 0.15f);
    }
}