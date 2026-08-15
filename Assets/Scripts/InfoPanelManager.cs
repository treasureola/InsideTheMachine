using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InfoPanelManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject infoPanel;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text funFactText;
    public Button closeButton;

    [Header("Animation")]
    public float slideInDuration = 0.35f;

    [Header("Component Image")]
    public RawImage componentImage;
    public Texture2D[] componentTextures; // assign 10 images in Inspector, index 0-9

    private string[] componentNames = {
        "Motherboard", "CPU", "CPU Cooler", "RAM",
        "GPU", "PSU", "HDD", "SSD", "Fan", "Case"
    };

    private string[] descriptions = {
        "The main circuit board that connects every component in the computer. Everything plugs into or communicates through the motherboard.",
        "The Central Processing Unit processes all instructions and calculations. It is the brain of the computer, executing billions of operations every second.",
        "Sits directly on top of the CPU to draw heat away from it. Without active cooling, a CPU would overheat and throttle within seconds.",
        "Random Access Memory provides fast temporary storage for programs currently running. All data in RAM is lost when power is cut.",
        "The Graphics Processing Unit handles all visual rendering. It has thousands of small cores optimized for parallel tasks like drawing pixels.",
        "The Power Supply Unit converts AC power from the wall into the DC voltages required by each component in the system.",
        "A Hard Disk Drive stores files permanently using magnetized spinning platters and a read/write head that moves across them.",
        "A Solid State Drive stores data on flash memory chips with no moving parts — faster, silent, and more shock-resistant than an HDD.",
        "Case fans improve airflow by pulling cool air in and pushing warm air out, keeping all internal components at safe temperatures.",
        "The PC case houses and protects all internal components. It provides structure, airflow management, and mounting points for every part of the build."
    };

    private string[] funFacts = {
        "A modern motherboard contains over 2,000 individual components.",
        "Modern CPUs can perform billions of operations per second.",
        "Without cooling, a CPU would overheat in seconds.",
        "RAM loses all data when power is cut — that is why you save your work.",
        "GPUs have thousands of cores compared to a CPU's dozen.",
        "PSUs are rated by efficiency — 80 Plus Gold means 87% efficiency.",
        "HDDs can store up to 20TB of data on magnetic platters.",
        "SSDs have no moving parts — silent and shock resistant.",
        "Case fans can spin at up to 3,000 RPM.",
        "PC cases come in Full Tower, Mid Tower, and Mini-ITX sizes."
    };

    private RectTransform panelRect;
    private bool isShowing = false;
    private bool currentlyOnRight = false;

    void Start()
    {
        panelRect = infoPanel.GetComponent<RectTransform>();
        infoPanel.SetActive(false);
        if (closeButton != null)
            closeButton.onClick.AddListener(HidePanel);
    }

    public void ShowPanel(int componentIndex, bool componentIsOnLeft)
    {
        if (componentIndex < 0 || componentIndex >= componentNames.Length) return;

        nameText.text        = componentNames[componentIndex];
        descriptionText.text = descriptions[componentIndex];
        funFactText.text     = "FUN FACT: " + funFacts[componentIndex];

        currentlyOnRight = componentIsOnLeft;

        if (componentIsOnLeft)
        {
            panelRect.anchorMin = new Vector2(1, 0.5f);
            panelRect.anchorMax = new Vector2(1, 0.5f);
            panelRect.pivot     = new Vector2(1, 0.5f);
            panelRect.anchoredPosition = new Vector2(panelRect.rect.width + 50f, 0f);
        }
        else
        {
            panelRect.anchorMin = new Vector2(0, 0.5f);
            panelRect.anchorMax = new Vector2(0, 0.5f);
            panelRect.pivot     = new Vector2(0, 0.5f);
            panelRect.anchoredPosition = new Vector2(-(panelRect.rect.width + 50f), 0f);
        }

        if (componentImage != null && componentTextures != null && 
            componentIndex < componentTextures.Length && 
            componentTextures[componentIndex] != null)
        {
            componentImage.texture = componentTextures[componentIndex];
            componentImage.gameObject.SetActive(true);
        }
        else if (componentImage != null)
        {
            componentImage.gameObject.SetActive(false);
        }

        infoPanel.SetActive(true);
        StopAllCoroutines();

        Vector2 hiddenPos = panelRect.anchoredPosition;
        Vector2 shownPos  = new Vector2(0f, 0f);

        AudioManager.Instance?.PlayPanelSlideIn();
        StartCoroutine(SlidePanel(hiddenPos, shownPos));
        isShowing = true;
    }

    public void HidePanel()
    {
        if (!isShowing) return;
        AudioManager.Instance?.PlayPanelSlideOut();
        StopAllCoroutines();
        StartCoroutine(SlideOut());
    }

    IEnumerator SlidePanel(Vector2 from, Vector2 to)
    {
        float elapsed = 0f;
        panelRect.anchoredPosition = from;

        while (elapsed < slideInDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideInDuration);
            panelRect.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }
        panelRect.anchoredPosition = to;
    }

    IEnumerator SlideOut()
    {
        Vector2 currentPos = panelRect.anchoredPosition;
        Vector2 hiddenPos  = currentlyOnRight
            ? new Vector2(panelRect.rect.width + 50f, 0f)
            : new Vector2(-(panelRect.rect.width + 50f), 0f);

        yield return StartCoroutine(SlidePanel(currentPos, hiddenPos));
        infoPanel.SetActive(false);
        isShowing = false;
    }
}