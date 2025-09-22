using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PressToStart : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup overlay;                 // CanvasGroup of the black Panel
    [Range(0.5f, 0.8f)]
    [SerializeField] private float overlayOpacity = 0.6f;         // 50–80 % as requested in the assignment

    [Header("Fade")]
    public bool fadeOut = true;
    [SerializeField] private float fadeOutDuration = 0.25f;       // uses UNscaled time

    [Header("Input")]
    public InputActionReference startAction;    // ← drag your "StartGame" action here

    private bool started;

    void Start()
    {
        // Show overlay and pause the game
        if (overlay != null)
        {
            overlay.alpha = overlayOpacity;
            overlay.blocksRaycasts = true;
            overlay.interactable = true;
        }
        Time.timeScale = 0f; // global pause
    }

    void OnEnable()
    {
        if (startAction != null)
        {
            // enable the action and listen to both started and performed
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
        if (started) return;    // make sure it runs only once
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
                t += Time.unscaledDeltaTime; // important: UNscaled
                overlay.alpha = Mathf.Lerp(from, 0f, t / fadeOutDuration);
                yield return null;
            }
        }

        if (overlay != null) overlay.gameObject.SetActive(false);
        Time.timeScale = 1f; // resume
    }
}