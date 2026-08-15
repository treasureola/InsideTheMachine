using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WelcomeScreen : MonoBehaviour
{
    public GameObject welcomePanel;
    public CameraController cameraController;
    public Button startButton;
    public CanvasGroup canvasGroup;

    void Start()
    {
        welcomePanel.SetActive(true);
        if (cameraController != null)
            cameraController.enabled = false;
        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;
        float elapsed = 0f;
        float duration = 1.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        float duration = 0.8f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.SmoothStep(1f, 0f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        welcomePanel.SetActive(false);
        if (cameraController != null)
            cameraController.enabled = true;
    }

    public void OnStartClicked()
    {
        StartCoroutine(FadeOut());
    }

    public void ShowWelcome()
    {
        welcomePanel.SetActive(true);
        if (cameraController != null) cameraController.enabled = false;
        StartCoroutine(FadeIn());
    }
}