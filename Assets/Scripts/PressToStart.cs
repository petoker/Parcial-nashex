using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PressToStart : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup overlay;                 // CanvasGroup del Panel negro
    [Range(0.5f, 0.8f)]
    public float overlayOpacity = 0.6f;         // 50–80 % como pide la consigna

    [Header("Fade")]
    public bool fadeOut = true;
    public float fadeOutDuration = 0.25f;       // usa tiempo NO escalado

    [Header("Input")]
    public InputActionReference startAction;    // ← arrástrale tu acción "StartGame"

    private bool started;

    void Start()
    {
        // Mostrar overlay y pausar el juego
        if (overlay != null)
        {
            overlay.alpha = overlayOpacity;
            overlay.blocksRaycasts = true;
            overlay.interactable = true;
        }
        Time.timeScale = 0f; // pausa global
    }

    void OnEnable()
    {
        if (startAction != null)
        {
            // habilitamos la acción y escuchamos tanto started como performed
            startAction.action.started += OnStart;
            startAction.action.performed += OnStart;
            startAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (startAction != null)
        {
            startAction.action.started -= OnStart;
            startAction.action.performed -= OnStart;
            startAction.action.Disable();
        }
    }

    private void OnStart(InputAction.CallbackContext ctx)
    {
        if (started) return;    // que solo corra una vez
        started = true;
        StartCoroutine(BeginGame());
    }

    private IEnumerator BeginGame()
    {
        if (overlay != null && fadeOut)
        {
            float t = 0f;
            float from = overlayOpacity;
            while (t < fadeOutDuration)
            {
                t += Time.unscaledDeltaTime; // importante: NO escalado
                overlay.alpha = Mathf.Lerp(from, 0f, t / fadeOutDuration);
                yield return null;
            }
        }

        if (overlay != null) overlay.gameObject.SetActive(false);
        Time.timeScale = 1f; // reanudar
    }
}