using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XRayMode : MonoBehaviour
{
    [Header("References")]
    public GameObject pcCase;
    public AssemblyManager assemblyManager;
    public CameraController cameraController;

    [Header("X-Ray Material")]
    public Material xrayMaterial;

    [Header("X-Ray Settings")]
    public float xrayAlpha          = 0.15f;
    public float transitionDuration = 0.8f;

    [Header("Component Tint")]
    public float componentXrayAlpha = 0.6f;

    public bool isXRay = false;

    private List<Renderer>   caseRenderers      = new List<Renderer>();
    private List<Material[]> originalMaterials  = new List<Material[]>();
    private List<Material>   caseMaterials      = new List<Material>();
    private List<Renderer>   mbRenderers        = new List<Renderer>();
    private List<Material[]> mbOriginalMaterials = new List<Material[]>();

    void Start()
    {
        CollectMaterials();
    }

    void CollectMaterials()
    {
        caseRenderers.Clear();
        originalMaterials.Clear();

        Renderer[] renderers = pcCase.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            caseRenderers.Add(r);
            originalMaterials.Add(r.materials);
        }
    }

    void SwapToXRayMaterials()
    {
        caseMaterials.Clear();

        foreach (Renderer r in caseRenderers)
        {
            Material[] xrayMats = new Material[r.materials.Length];
            for (int i = 0; i < xrayMats.Length; i++)
                xrayMats[i] = xrayMaterial;
            r.materials = xrayMats;
        }

        foreach (Renderer r in caseRenderers)
            foreach (Material mat in r.materials)
                caseMaterials.Add(mat);
    }

    void RestoreOriginalMaterials()
    {
        for (int i = 0; i < caseRenderers.Count; i++)
            caseRenderers[i].materials = originalMaterials[i];
        caseMaterials.Clear();
    }

    void RestoreMotherboardMaterials()
    {
        for (int i = 0; i < mbRenderers.Count; i++)
            mbRenderers[i].materials = mbOriginalMaterials[i];
        mbRenderers.Clear();
        mbOriginalMaterials.Clear();
    }

    public void ToggleXRay()
    {
        if (!isXRay)
            StartCoroutine(EnterXRay());
        else
            StartCoroutine(FadeInCase());
    }

    IEnumerator EnterXRay()
    {
        isXRay = true;

        if (cameraController != null) cameraController.HideHintText();

        if (!assemblyManager.assemblyActive)
        {
            AudioManager.Instance?.PlayCameraWhoosh();
            cameraController.FlipToAssembly(1.8f);
            yield return new WaitForSeconds(0.6f);

            AudioManager.Instance?.PlayCaseFly();
            yield return StartCoroutine(assemblyManager.FlyCaseToAssembly());
        }
        else
        {
            assemblyManager.HideAssemblyPanel();
        }

        yield return StartCoroutine(assemblyManager.FlyRemainingToCase());

        CollectMaterials();
        SwapToXRayMaterials();
        yield return StartCoroutine(TransitionXRay(true));
        yield return StartCoroutine(TintMotherboard());

        // Enable free movement so player can walk around the case
        if (cameraController != null) cameraController.SetOrbitMode(true, assemblyManager.caseAssemblyPosition);
    }

    IEnumerator FadeInCase()
    {
        isXRay = false;

        // Disable free movement
        if (cameraController != null) cameraController.SetOrbitMode(false, Vector3.zero);

        RestoreMotherboardMaterials();
        yield return StartCoroutine(TransitionXRay(false));
        RestoreOriginalMaterials();
    }

    IEnumerator TransitionXRay(bool goingTransparent)
    {
        float elapsed     = 0f;
        float targetAlpha = goingTransparent ? xrayAlpha : 1f;

        float[] startAlphas = new float[caseMaterials.Count];
        for (int i = 0; i < caseMaterials.Count; i++)
            startAlphas[i] = caseMaterials[i].color.a;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);

            for (int i = 0; i < caseMaterials.Count; i++)
            {
                Color c = caseMaterials[i].color;
                c.a = Mathf.Lerp(startAlphas[i], targetAlpha, t);
                caseMaterials[i].color = c;
            }
            yield return null;
        }

        for (int i = 0; i < caseMaterials.Count; i++)
        {
            Color c = caseMaterials[i].color;
            c.a = targetAlpha;
            caseMaterials[i].color = c;
            if (!goingTransparent) SetMaterialOpaque(caseMaterials[i]);
        }
    }

    IEnumerator TintMotherboard()
    {
        if (assemblyManager.components.Length == 0) yield break;

        GameObject motherboard = assemblyManager.components[0];
        Renderer[] renderers   = motherboard.GetComponentsInChildren<Renderer>();

        mbRenderers.Clear();
        mbOriginalMaterials.Clear();
        foreach (Renderer r in renderers)
        {
            mbRenderers.Add(r);
            mbOriginalMaterials.Add(r.materials);
        }

        float elapsed  = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            foreach (Renderer r in renderers)
            {
                foreach (Material mat in r.materials)
                {
                    if (mat.HasProperty("_BaseColor"))
                    {
                        Color c = mat.GetColor("_BaseColor");
                        c.a = Mathf.Lerp(1f, componentXrayAlpha, t);
                        mat.SetColor("_BaseColor", c);
                    }
                    else if (mat.HasProperty("_BaseColorFactor"))
                    {
                        Color c = mat.GetColor("_BaseColorFactor");
                        c.a = Mathf.Lerp(1f, componentXrayAlpha, t);
                        mat.SetColor("_BaseColorFactor", c);
                    }
                    else if (mat.HasProperty("_Color"))
                    {
                        Color c = mat.GetColor("_Color");
                        c.a = Mathf.Lerp(1f, componentXrayAlpha, t);
                        mat.SetColor("_Color", c);
                    }
                }
            }
            yield return null;
        }
    }

    public void ResetXRay()
    {
        isXRay = false;
        StopAllCoroutines();
        if (cameraController != null) cameraController.SetOrbitMode(false, Vector3.zero);
        RestoreMotherboardMaterials();
        RestoreOriginalMaterials();
    }

    void SetMaterialTransparent(Material mat)
    {
        mat.SetFloat("_Surface", 1);
        mat.SetFloat("_Blend", 0);
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        mat.SetShaderPassEnabled("ShadowCaster", false);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
    }

    void SetMaterialOpaque(Material mat)
    {
        mat.SetFloat("_Surface", 0);
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
        mat.SetShaderPassEnabled("ShadowCaster", true);
        mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
    }
}