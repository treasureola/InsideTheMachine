using UnityEngine;
using TMPro;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float moveSpeed   = 20f;
    public float rotateSpeed = 50f;
    public float minZ        = -120f;
    public float maxZ        = 90f;

    [Header("Menu Reveal")]
    public GameObject menuPanel;
    public TextMeshPro hintText;
    public float menuRevealZ       = 40f;
    public float menuSlideDuration = 0.5f;
    private bool menuRevealed = false;

    [Header("Orbit Mode")]
    public Vector3 orbitTarget   = new Vector3(0, 8, 0);
    public float   orbitDistance = 30f;
    public float   orbitHeight   = 15f;
    private float  orbitAngleY   = 180f;
    private bool   orbitMode     = false;

    private float currentZ      = -120f;
    private float currentHeight = 15f;
    private float currentAngleY = 0f;
    private bool  playerControlEnabled = true;
    private bool  freeMovement         = false;

    private float savedZ;
    private float savedHeight;
    private float savedAngleY;
    private bool  stateSaved = false;

    void Start()
    {
        currentZ      = minZ;
        currentHeight = 15f;
        currentAngleY = 0f;

        if (menuPanel != null)
        {
            RectTransform rt = menuPanel.GetComponent<RectTransform>();
            if (rt != null) rt.anchoredPosition = new Vector2(0f, 100f);
            menuPanel.SetActive(false);
        }

        if (hintText != null) hintText.gameObject.SetActive(false);

        ApplyTransform(8f);
    }

    void Update()
    {
        if (!playerControlEnabled) return;

        // ── Orbit mode — full 360° around case ───────────────────────────
        if (orbitMode)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                orbitAngleY -= rotateSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                orbitAngleY += rotateSpeed * Time.deltaTime;

            if (Input.GetMouseButton(1))
            {
                orbitAngleY += Input.GetAxis("Mouse X") * 3f;
                orbitHeight  = Mathf.Clamp(
                    orbitHeight - Input.GetAxis("Mouse Y") * 3f, 3f, 40f);
            }

            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                orbitDistance = Mathf.Clamp(
                    orbitDistance - Input.mouseScrollDelta.y * 2f, 10f, 80f);
            }

            // W/S moves closer/further from target
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                orbitDistance = Mathf.Clamp(orbitDistance - moveSpeed * Time.deltaTime, 10f, 80f);
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                orbitDistance = Mathf.Clamp(orbitDistance + moveSpeed * Time.deltaTime, 10f, 80f);

            float rad = orbitAngleY * Mathf.Deg2Rad;
            float x   = orbitTarget.x + Mathf.Sin(rad) * orbitDistance;
            float z   = orbitTarget.z + Mathf.Cos(rad) * orbitDistance;

            transform.position = new Vector3(x, orbitHeight, z);
            transform.LookAt(new Vector3(orbitTarget.x, orbitHeight, orbitTarget.z));
            return;
        }

        // ── Normal corridor movement ──────────────────────────────────────
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            currentZ += moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            currentZ -= moveSpeed * Time.deltaTime;

        if (!freeMovement)
            currentZ = Mathf.Clamp(currentZ, minZ, maxZ);

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            currentAngleY -= rotateSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            currentAngleY += rotateSpeed * Time.deltaTime;

        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            float scroll = Input.mouseScrollDelta.y;
            currentHeight -= scroll * 2f;
            currentHeight = Mathf.Clamp(currentHeight, 3f, 40f);
        }

        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * 3f;
            float mouseY = Input.GetAxis("Mouse Y") * 3f;
            currentAngleY += mouseX;
            currentHeight  = Mathf.Clamp(currentHeight - mouseY, 3f, 40f);
        }

        ApplyTransform(8f);

        if (!menuRevealed && currentZ >= menuRevealZ)
        {
            menuRevealed = true;
            StartCoroutine(SlideMenuIn());
        }
    }

    public void SetFreeMovement(bool free)
    {
        freeMovement = free;
    }

    public void SetOrbitMode(bool enabled, Vector3 target)
    {
        orbitMode   = enabled;
        orbitTarget = target;
        orbitHeight = currentHeight;

        if (enabled)
        {
            // Calculate starting angle based on current camera position relative to target
            Vector3 offset = transform.position - target;
            orbitAngleY    = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
            orbitDistance  = Mathf.Clamp(
                new Vector3(offset.x, 0, offset.z).magnitude, 10f, 80f);
        }
    }

    IEnumerator SlideMenuIn()
    {
        menuPanel.SetActive(true);
        if (hintText != null) hintText.gameObject.SetActive(true);
        AudioManager.Instance?.PlayMenuReveal();

        RectTransform rt = menuPanel.GetComponent<RectTransform>();
        Vector2 hiddenPos = new Vector2(rt.anchoredPosition.x, 100f);
        Vector2 shownPos  = new Vector2(rt.anchoredPosition.x, 0f);

        float elapsed = 0f;
        rt.anchoredPosition = hiddenPos;

        while (elapsed < menuSlideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / menuSlideDuration);
            rt.anchoredPosition = Vector2.Lerp(hiddenPos, shownPos, t);
            yield return null;
        }

        rt.anchoredPosition = shownPos;
    }

    public void HideHintText()
    {
        if (hintText != null) hintText.gameObject.SetActive(false);
    }

    void ApplyTransform(float tiltX)
    {
        transform.position = new Vector3(0, currentHeight, currentZ);
        transform.rotation = Quaternion.Euler(tiltX, currentAngleY, 0);
    }

    public void FlipToAssembly(float duration = 1.8f)
    {
        if (!stateSaved)
        {
            savedZ      = currentZ;
            savedHeight = currentHeight;
            savedAngleY = currentAngleY;
            stateSaved  = true;
        }
        StartCoroutine(SmoothMove(147f, 15f, 180f, 8f, duration));
    }

    public void FlipToExplore(float duration = 1.8f)
    {
        orbitMode    = false;
        freeMovement = false;
        stateSaved   = false;
        // menuRevealed = false;
        // if (menuPanel != null) menuPanel.SetActive(false);
        StartCoroutine(SmoothMove(minZ, 15f, 0f, 8f, duration));
    }

    IEnumerator SmoothMove(float toZ, float toHeight, float toAngleY,
                           float toTiltX, float duration)
    {
        playerControlEnabled = false;

        float elapsed    = 0f;
        float fromZ      = currentZ;
        float fromHeight = currentHeight;
        float fromAngleY = currentAngleY;
        float fromTiltX  = transform.rotation.eulerAngles.x;
        if (fromTiltX > 180f) fromTiltX -= 360f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            currentZ      = Mathf.Lerp(fromZ,      toZ,      t);
            currentHeight = Mathf.Lerp(fromHeight, toHeight, t);
            currentAngleY = Mathf.LerpAngle(fromAngleY, toAngleY, t);
            float tiltX   = Mathf.Lerp(fromTiltX,  toTiltX, t);

            transform.position = new Vector3(0, currentHeight, currentZ);
            transform.rotation = Quaternion.Euler(tiltX, currentAngleY, 0);

            yield return null;
        }

        currentZ      = toZ;
        currentHeight = toHeight;
        currentAngleY = toAngleY;
        transform.position = new Vector3(0, currentHeight, currentZ);
        transform.rotation = Quaternion.Euler(toTiltX, currentAngleY, 0);

        playerControlEnabled = true;
    }
}