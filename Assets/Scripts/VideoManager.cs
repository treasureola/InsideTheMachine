using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public GameObject videoPanel;
    public VideoPlayer videoPlayer;
    public Button closeButton;

    void Start()
    {
        videoPanel.SetActive(false);
        if (closeButton != null)
            closeButton.onClick.AddListener(HideVideo);
    }

    public void ShowVideo()
    {
        videoPanel.SetActive(true);
        videoPlayer.Play();
    }

    public void HideVideo()
    {
        videoPlayer.Stop();
        videoPanel.SetActive(false);
    }
}