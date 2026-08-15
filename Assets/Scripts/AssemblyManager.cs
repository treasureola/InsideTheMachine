using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class AssemblyManager : MonoBehaviour
{
    [Header("Components — build order 0-8")]
    public GameObject[] components;
    public Vector3[] wallPositions;

    [Header("Case")]
    public GameObject pcCase;
    public Vector3 caseDisplayPosition  = new Vector3(15, 11.8f, -80.69f);
    public Vector3 caseDisplayScale     = new Vector3(0.75f, 0.75f, 0.75f);
    public Vector3 caseDisplayRotation  = new Vector3(0, 0, 0);
    public Vector3 caseAssemblyPosition = new Vector3(0, 8, 0);
    public Vector3 caseAssemblyScale    = new Vector3(5, 5, 5);
    public Vector3 caseAssemblyRotation = new Vector3(0, 0, 0);

    [Header("Case Slot Positions (world space)")]
    public Vector3[] caseSlotPositions = new Vector3[]
    {
        new Vector3( 0f,    8f,    0f),
        new Vector3( 0f,    9.5f,  0f),
        new Vector3( 0f,   11f,    0f),
        new Vector3( 1.5f,  9.5f,  0f),
        new Vector3(-1.5f,  8.5f,  0f),
        new Vector3( 0f,    6f,    0f),
        new Vector3( 2f,    7f,    0f),
        new Vector3( 2f,    8f,    0f),
        new Vector3( 0f,   12f,    0f),
    };

    [Header("Case Slot Scales")]
    public Vector3[] caseSlotScales = new Vector3[]
    {
        new Vector3(20f,   20f,   20f),
        new Vector3(15f,   15f,   15f),
        new Vector3( 1f,    1f,    1f),
        new Vector3( 1.25f, 1.25f, 1.25f),
        new Vector3(25f,   25f,   25f),
        new Vector3( 1.25f, 1.25f, 1.25f),
        new Vector3( 1f,    1f,    1f),
        new Vector3( 2f,    2f,    2f),
        new Vector3(0.02f, 0.02f, 0.02f),
    };

    [Header("Case Slot Rotations")]
    public Vector3[] caseSlotRotations = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),
    };

    [Header("Camera")]
    public CameraController cameraController;

    [Header("Info Panel")]
    public InfoPanelManager infoPanelManager;

    [Header("X-Ray")]
    public XRayMode xRayMode;

    [Header("UI")]
    public GameObject   menuPanel;
    public GameObject   assemblyPanel;
    public TMP_Text     questionText;
    public TMP_Dropdown componentDropdown;
    public Button       submitButton;
    public TMP_Text     feedbackText;
    public TMP_Text     placeholderText;

    private Vector3[] corridorPositions = new Vector3[]
    {
        new Vector3(-15, 8,   79.76f),
        new Vector3(-15, 9,    0f),
        new Vector3( 14, 12,   0.1f),
        new Vector3(-15.08f, 12.64f, -40.47f),
        new Vector3(-15, 12.43f, 40f),
        new Vector3(-14.67f, 11.1f, -79.8f),
        new Vector3( 15, 12.61f, 80f),
        new Vector3( 15, 11.46f, 40f),
        new Vector3( 15.22f, 11.99f, -42.46f),
    };

    private Vector3[] wallScales;
    private Vector3[] wallRotations;

    private string[] questions = {
        "What is the main circuit board that everything connects to?",
        "What component processes all the computer's instructions?",
        "What keeps the CPU from overheating?",
        "What provides temporary memory for running programs?",
        "What handles graphics and visual output?",
        "What supplies power to all components?",
        "What stores your files permanently?",
        "What is a faster alternative to a hard drive?",
        "What improves airflow inside the case?"
    };

    private string[] correctAnswers = {
        "Motherboard", "CPU", "CPU Cooler", "RAM", "GPU", "PSU", "HDD", "SSD", "Fan"
    };

    // Index 0 is dummy Motherboard — acts as placeholder
    // Real Motherboard is at index 1, rest follow
    private string[] allComponents = {
        "",
        "Motherboard", "CPU", "CPU Cooler",
        "RAM", "GPU", "PSU", "HDD", "SSD", "Fan"
    };

    private int    currentStep         = 0;
    private bool   withQuestions       = false;
    public  bool   assemblyActive      = false;
    private bool   isAnimating         = false;
    private bool   isResettingDropdown = false;

    void Start()
    {
        if (wallPositions == null || wallPositions.Length != components.Length)
            wallPositions = corridorPositions;

        wallScales    = new Vector3[components.Length];
        wallRotations = new Vector3[components.Length];
        for (int i = 0; i < components.Length; i++)
        {
            wallScales[i]    = components[i].transform.localScale;
            wallRotations[i] = components[i].transform.eulerAngles;
        }

        if (menuPanel != null) menuPanel.SetActive(false);
        assemblyPanel.SetActive(false);
        feedbackText.text = "";

        submitButton.onClick.AddListener(OnSubmit);
        SetupDropdown();
    }

    void SetupDropdown()
    {
        isResettingDropdown = true;
        componentDropdown.ClearOptions();
        componentDropdown.AddOptions(new List<string>(allComponents));
        componentDropdown.onValueChanged.RemoveAllListeners();
        componentDropdown.value = 0;
        componentDropdown.onValueChanged.AddListener(OnDropdownChanged);
        isResettingDropdown = false;

        // Hide caption — placeholder overlay shows instead
        if (componentDropdown.captionText != null)
            componentDropdown.captionText.color = new Color(0, 0, 0, 0);

        if (placeholderText != null) placeholderText.gameObject.SetActive(true);
    }

    void ResetDropdown()
    {
        isResettingDropdown = true;
        componentDropdown.onValueChanged.RemoveAllListeners();
        componentDropdown.value = 0;
        componentDropdown.onValueChanged.AddListener(OnDropdownChanged);
        isResettingDropdown = false;

        if (componentDropdown.captionText != null)
            componentDropdown.captionText.color = new Color(0, 0, 0, 0);

        if (placeholderText != null) placeholderText.gameObject.SetActive(true);
    }

    void OnDropdownChanged(int value)
    {
        if (isResettingDropdown) return;
        if (value <= 0) return; // 0 is dummy — ignore it
        if (value < componentDropdown.options.Count)
        {
            if (placeholderText != null) placeholderText.gameObject.SetActive(false);
            if (componentDropdown.captionText != null)
                componentDropdown.captionText.color = Color.blue;
        }
    }

    void SetComponentColliders(bool enabled)
    {
        foreach (GameObject comp in components)
        {
            Collider[] colliders = comp.GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
                c.enabled = enabled;
        }
    }

    public void StartAssemblyWithQuestions()
    {
        withQuestions = true;
        StartAssembly();
    }

    public void StartAssemblyWithoutQuestions()
    {
        withQuestions = false;
        StartAssembly();
    }

    void StartAssembly()
    {
        StopAllCoroutines();
        isAnimating = false;
        currentStep = 0;

        if (xRayMode != null && xRayMode.isXRay) xRayMode.ResetXRay();
        if (infoPanelManager != null) infoPanelManager.HidePanel();
        if (cameraController != null) cameraController.HideHintText();

        SetComponentColliders(false);

        ComponentRotator caseRotator = pcCase.GetComponent<ComponentRotator>();
        if (caseRotator != null) caseRotator.StopRotation();

        for (int i = 0; i < components.Length; i++)
        {
            components[i].transform.position   = wallPositions[i];
            components[i].transform.localScale  = wallScales[i];
            components[i].transform.eulerAngles = wallRotations[i];
            ComponentRotator rotator = components[i].GetComponent<ComponentRotator>();
            if (rotator != null) rotator.StopRotation();
        }

        if (assemblyActive)
        {
            assemblyPanel.SetActive(true);
            if (withQuestions) ShowQuestion();
            else ShowWithoutQuestion();
        }
        else
        {
            assemblyActive = true;
            assemblyPanel.SetActive(true);
            questionText.text = "Get ready...";
            feedbackText.text = "";
            componentDropdown.gameObject.SetActive(false);
            if (placeholderText != null) placeholderText.gameObject.SetActive(false);
            submitButton.gameObject.SetActive(false);
            StartCoroutine(AssemblyEnterSequence());
        }
    }

    IEnumerator AssemblyEnterSequence()
    {
        if (cameraController != null)
        {
            AudioManager.Instance?.PlayCameraWhoosh();
            cameraController.FlipToAssembly(1.8f);
        }

        yield return new WaitForSeconds(0.6f);

        AudioManager.Instance?.PlayCaseFly();
        yield return StartCoroutine(FlyCase(caseAssemblyPosition, caseAssemblyScale, caseAssemblyRotation, 1.4f));

        if (withQuestions) ShowQuestion();
        else ShowWithoutQuestion();
    }

    void ShowQuestion()
    {
        questionText.text = questions[currentStep];
        feedbackText.text = "";
        ResetDropdown();
        componentDropdown.gameObject.SetActive(true);
        if (placeholderText != null) placeholderText.gameObject.SetActive(true);
        submitButton.gameObject.SetActive(true);
    }

    void ShowWithoutQuestion()
    {
        questionText.text = "Choose the components in the correct build order.";
        feedbackText.text = "";
        ResetDropdown();
        componentDropdown.gameObject.SetActive(true);
        if (placeholderText != null) placeholderText.gameObject.SetActive(true);
        submitButton.gameObject.SetActive(true);
    }

    void OnSubmit()
    {
        if (isAnimating) return;
        if (infoPanelManager != null) infoPanelManager.HidePanel();

        // Block if still on dummy index 0
        if (componentDropdown.value <= 0)
        {
            feedbackText.color = Color.red;
            feedbackText.text  = "Please select a component!";
            return;
        }

        // Real options start at index 1 — text comparison handles correctness
        string selected = componentDropdown.options[componentDropdown.value].text;

        if (selected == correctAnswers[currentStep])
        {
            feedbackText.color = Color.green;
            feedbackText.text  = "Correct!";
            AudioManager.Instance?.PlayCorrectAnswer();
            StartCoroutine(FlyComponentToCase(currentStep));
        }
        else
        {
            feedbackText.color = Color.red;
            feedbackText.text  = "Wrong! Try again.";
            AudioManager.Instance?.PlayWrongAnswer();
        }
    }

    public void OnComponentClicked(int componentIndex)
    {
        if (!assemblyActive || withQuestions || isAnimating) return;
        if (infoPanelManager != null) infoPanelManager.HidePanel();

        if (componentIndex == currentStep)
        {
            feedbackText.color = Color.green;
            feedbackText.text  = "Correct!";
            AudioManager.Instance?.PlayCorrectAnswer();
            StartCoroutine(FlyComponentToCase(currentStep));
        }
        else
        {
            feedbackText.color = Color.red;
            feedbackText.text  = "Wrong order! Try again.";
            AudioManager.Instance?.PlayWrongAnswer();
        }
    }

    IEnumerator FlyComponentToCase(int index)
    {
        isAnimating = true;
        GameObject comp           = components[index];
        Vector3    targetPos      = caseSlotPositions[index];
        Vector3    targetScale    = caseSlotScales[index];
        Vector3    targetRotation = caseSlotRotations[index];
        float      duration       = 1.5f;
        float      elapsed        = 0f;
        Vector3    startPos       = comp.transform.position;
        Vector3    startScale     = comp.transform.localScale;
        Vector3    startRot       = comp.transform.eulerAngles;

        AudioManager.Instance?.PlayComponentFly();

        ComponentRotator rotator = comp.GetComponent<ComponentRotator>();
        if (rotator != null) rotator.StopRotation();

        ComponentLabel label = comp.GetComponent<ComponentLabel>();
        if (label != null) label.HideLabel();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            comp.transform.position    = Vector3.Lerp(startPos,   targetPos,   t);
            comp.transform.localScale  = Vector3.Lerp(startScale, targetScale, t);
            comp.transform.eulerAngles = new Vector3(
                Mathf.LerpAngle(startRot.x, targetRotation.x, t),
                Mathf.LerpAngle(startRot.y, targetRotation.y, t),
                Mathf.LerpAngle(startRot.z, targetRotation.z, t)
            );
            yield return null;
        }

        comp.transform.position    = targetPos;
        comp.transform.localScale  = targetScale;
        comp.transform.eulerAngles = targetRotation;
        AudioManager.Instance?.PlayComponentLand();

        currentStep++;
        isAnimating = false;

        if (currentStep >= components.Length)
            StartCoroutine(ShowCompletionMessage());
        else
        {
            if (withQuestions) ShowQuestion();
            else ShowWithoutQuestion();
        }
    }

    IEnumerator ShowCompletionMessage()
    {
        questionText.text  = "🎉 Assembly Complete!";
        feedbackText.color = Color.green;
        feedbackText.text  = "You built a PC!";
        componentDropdown.gameObject.SetActive(false);
        if (placeholderText != null) placeholderText.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
        AudioManager.Instance?.PlayAssemblyComplete();

        yield return new WaitForSeconds(4f);

        assemblyPanel.SetActive(false);

        if (cameraController != null)
            cameraController.SetOrbitMode(true, caseAssemblyPosition);
    }

    public void Disassemble()
    {
        if (infoPanelManager != null) infoPanelManager.HidePanel();
        if (xRayMode != null) xRayMode.ResetXRay();
        StartCoroutine(DisassembleAll());
    }

    IEnumerator DisassembleAll()
    {
        if (cameraController != null) cameraController.SetOrbitMode(false, Vector3.zero);
        if (cameraController != null) cameraController.HideHintText();
        assemblyActive = false;
        assemblyPanel.SetActive(false);
        isAnimating = true;

        if (currentStep > 0)
        {
            List<Coroutine> flyCoroutines = new List<Coroutine>();
            for (int i = 0; i < currentStep; i++)
                flyCoroutines.Add(StartCoroutine(FlyBack(
                    components[i], wallPositions[i], wallScales[i], wallRotations[i])));

            foreach (Coroutine c in flyCoroutines)
                yield return c;
        }

        AudioManager.Instance?.PlayCaseFly();
        StartCoroutine(FlyCase(caseDisplayPosition, caseDisplayScale, caseDisplayRotation, 1.4f));

        AudioManager.Instance?.PlayCameraWhoosh();
        if (cameraController != null)
            cameraController.FlipToExplore(1.8f);

        yield return new WaitForSeconds(1.8f);

        ComponentRotator caseRotator = pcCase.GetComponent<ComponentRotator>();
        if (caseRotator != null) caseRotator.StartRotation();

        foreach (GameObject comp in components)
        {
            ComponentRotator rotator = comp.GetComponent<ComponentRotator>();
            if (rotator != null) rotator.StartRotation();

            ComponentLabel label = comp.GetComponent<ComponentLabel>();
            if (label != null) label.ShowLabel();
        }

        SetComponentColliders(true);
        currentStep = 0;
        isAnimating = false;
    }

    IEnumerator FlyBack(GameObject comp, Vector3 targetPos, Vector3 targetScale, Vector3 targetRotation)
    {
        float   duration   = 1.5f;
        float   elapsed    = 0f;
        Vector3 startPos   = comp.transform.position;
        Vector3 startScale = comp.transform.localScale;
        Vector3 startRot   = comp.transform.eulerAngles;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            comp.transform.position    = Vector3.Lerp(startPos,   targetPos,   t);
            comp.transform.localScale  = Vector3.Lerp(startScale, targetScale, t);
            comp.transform.eulerAngles = new Vector3(
                Mathf.LerpAngle(startRot.x, targetRotation.x, t),
                Mathf.LerpAngle(startRot.y, targetRotation.y, t),
                Mathf.LerpAngle(startRot.z, targetRotation.z, t)
            );
            yield return null;
        }
        comp.transform.position    = targetPos;
        comp.transform.localScale  = targetScale;
        comp.transform.eulerAngles = targetRotation;
    }

    IEnumerator FlyCase(Vector3 targetPos, Vector3 targetScale, Vector3 targetRotation, float duration)
    {
        float   elapsed    = 0f;
        Vector3 startPos   = pcCase.transform.position;
        Vector3 startScale = pcCase.transform.localScale;
        Vector3 startRot   = pcCase.transform.eulerAngles;

        Vector3 midPos = new Vector3(
            (startPos.x + targetPos.x) / 2f,
            Mathf.Max(startPos.y, targetPos.y) + 40f,
            (startPos.z + targetPos.z) / 2f
        );

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            Vector3 a = Vector3.Lerp(startPos, midPos, t);
            Vector3 b = Vector3.Lerp(midPos, targetPos, t);
            pcCase.transform.position    = Vector3.Lerp(a, b, t);
            pcCase.transform.localScale  = Vector3.Lerp(startScale, targetScale, t);
            pcCase.transform.eulerAngles = new Vector3(
                Mathf.LerpAngle(startRot.x, targetRotation.x, t),
                Mathf.LerpAngle(startRot.y, targetRotation.y, t),
                Mathf.LerpAngle(startRot.z, targetRotation.z, t)
            );
            yield return null;
        }

        pcCase.transform.position    = targetPos;
        pcCase.transform.localScale  = targetScale;
        pcCase.transform.eulerAngles = targetRotation;
    }

    public void BackToMenu()
    {
        StopAllCoroutines();
        if (cameraController != null) cameraController.SetOrbitMode(false, Vector3.zero);
        assemblyActive = false;
        isAnimating    = false;
        currentStep    = 0;
        assemblyPanel.SetActive(false);

        if (xRayMode != null && xRayMode.isXRay) xRayMode.ResetXRay();
        if (infoPanelManager != null) infoPanelManager.HidePanel();
        AudioManager.Instance?.PlayCameraWhoosh();

        pcCase.transform.position    = caseDisplayPosition;
        pcCase.transform.localScale  = caseDisplayScale;
        pcCase.transform.eulerAngles = caseDisplayRotation;

        if (cameraController != null)
            cameraController.FlipToExplore(0.8f);

        for (int i = 0; i < components.Length; i++)
        {
            components[i].transform.position   = wallPositions[i];
            components[i].transform.localScale  = wallScales[i];
            components[i].transform.eulerAngles = wallRotations[i];

            ComponentRotator rotator = components[i].GetComponent<ComponentRotator>();
            if (rotator != null) rotator.StartRotation();

            ComponentLabel label = components[i].GetComponent<ComponentLabel>();
            if (label != null) label.ShowLabel();
        }

        SetComponentColliders(true);

        ComponentRotator caseRotator = pcCase.GetComponent<ComponentRotator>();
        if (caseRotator != null) caseRotator.StartRotation();
    }

    public void HideAssemblyPanel()
    {
        assemblyPanel.SetActive(false);
    }

    public IEnumerator FlyCaseToAssembly()
    {
        assemblyActive = true;
        SetComponentColliders(false);

        ComponentRotator caseRotator = pcCase.GetComponent<ComponentRotator>();
        if (caseRotator != null) caseRotator.StopRotation();

        yield return StartCoroutine(FlyCase(
            caseAssemblyPosition, caseAssemblyScale, caseAssemblyRotation, 1.4f));
    }

    public IEnumerator FlyCaseToDisplay()
    {
        yield return StartCoroutine(FlyCase(
            caseDisplayPosition, caseDisplayScale, caseDisplayRotation, 1.4f));

        ComponentRotator caseRotator = pcCase.GetComponent<ComponentRotator>();
        if (caseRotator != null) caseRotator.StartRotation();
    }

    public IEnumerator FlyRemainingToCase()
    {
        List<Coroutine> flies = new List<Coroutine>();

        for (int i = currentStep; i < components.Length; i++)
        {
            int idx = i;
            flies.Add(StartCoroutine(FlyComponentInstant(idx)));
        }

        foreach (Coroutine c in flies)
            yield return c;

        currentStep = components.Length;
    }

    IEnumerator FlyComponentInstant(int index)
    {
        GameObject comp           = components[index];
        Vector3    targetPos      = caseSlotPositions[index];
        Vector3    targetScale    = caseSlotScales[index];
        Vector3    targetRotation = caseSlotRotations[index];
        float      duration       = 1.2f;
        float      elapsed        = 0f;
        Vector3    startPos       = comp.transform.position;
        Vector3    startScale     = comp.transform.localScale;
        Vector3    startRot       = comp.transform.eulerAngles;

        AudioManager.Instance?.PlayComponentFly();

        ComponentRotator rotator = comp.GetComponent<ComponentRotator>();
        if (rotator != null) rotator.StopRotation();

        ComponentLabel label = comp.GetComponent<ComponentLabel>();
        if (label != null) label.HideLabel();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            comp.transform.position    = Vector3.Lerp(startPos,   targetPos,   t);
            comp.transform.localScale  = Vector3.Lerp(startScale, targetScale, t);
            comp.transform.eulerAngles = new Vector3(
                Mathf.LerpAngle(startRot.x, targetRotation.x, t),
                Mathf.LerpAngle(startRot.y, targetRotation.y, t),
                Mathf.LerpAngle(startRot.z, targetRotation.z, t)
            );
            yield return null;
        }

        comp.transform.position    = targetPos;
        comp.transform.localScale  = targetScale;
        comp.transform.eulerAngles = targetRotation;
    }

    public IEnumerator FlyAllBackToWalls()
    {
        assemblyActive = false;
        currentStep    = 0;

        List<Coroutine> flies = new List<Coroutine>();
        for (int i = 0; i < components.Length; i++)
            flies.Add(StartCoroutine(FlyBack(
                components[i], wallPositions[i], wallScales[i], wallRotations[i])));

        foreach (Coroutine c in flies)
            yield return c;

        foreach (GameObject comp in components)
        {
            ComponentRotator rotator = comp.GetComponent<ComponentRotator>();
            if (rotator != null) rotator.StartRotation();

            ComponentLabel label = comp.GetComponent<ComponentLabel>();
            if (label != null) label.ShowLabel();
        }

        SetComponentColliders(true);
    }
}