using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("UI Sounds")]
    public AudioClip buttonClick;
    public AudioClip wrongAnswer;
    public AudioClip correctAnswer;
    public AudioClip assemblyComplete;

    [Header("Panel Sounds")]
    public AudioClip panelSlideIn;
    public AudioClip panelSlideOut;
    public AudioClip menuReveal;

    [Header("Camera Sounds")]
    public AudioClip cameraWhoosh;

    [Header("Component Sounds")]
    public AudioClip componentLand;
    public AudioClip componentFly;
    public AudioClip caseFly;

    [Header("Ambient")]
    public AudioClip ambientLoop;

    [Header("Settings")]
    [Range(0f, 1f)] public float masterVolume  = 1f;
    [Range(0f, 1f)] public float sfxVolume     = 1f;
    [Range(0f, 1f)] public float ambientVolume = 0.3f;

    private AudioSource sfxSource;
    private AudioSource ambientSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        ambientSource = gameObject.AddComponent<AudioSource>();
        ambientSource.playOnAwake = false;
        ambientSource.loop        = true;

        PlayAmbient();
    }

    public void PlayButtonClick()       => Play(buttonClick);
    public void PlayWrongAnswer()       => Play(wrongAnswer);
    public void PlayCorrectAnswer()     => Play(correctAnswer);
    public void PlayAssemblyComplete()  => Play(assemblyComplete);
    public void PlayPanelSlideIn()      => Play(panelSlideIn);
    public void PlayPanelSlideOut()     => Play(panelSlideOut);
    public void PlayMenuReveal()        => Play(menuReveal);
    public void PlayCameraWhoosh()      => Play(cameraWhoosh);
    public void PlayComponentLand()     => Play(componentLand);
    public void PlayComponentFly()      => Play(componentFly);
    public void PlayCaseFly()           => Play(caseFly);

    void Play(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
    }

    void PlayAmbient()
    {
        if (ambientLoop == null) return;
        ambientSource.clip   = ambientLoop;
        ambientSource.volume = ambientVolume * masterVolume;
        ambientSource.Play();
    }
}