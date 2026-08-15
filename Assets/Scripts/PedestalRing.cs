using UnityEngine;

public class PedestalRing : MonoBehaviour
{
    public float rotateSpeed = 30f;
    public float pulseSpeed = 2f;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2f;

    private Material mat;
    private float time;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // Rotate ring
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);

        // Pulse emission intensity
        time += Time.deltaTime;
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, 
            (Mathf.Sin(time * pulseSpeed) + 1f) / 2f);

        // Cycle through teal and purple
        float t = (Mathf.Sin(time * 0.5f) + 1f) / 2f;
        Color teal   = new Color(0.05f, 0.58f, 0.53f);
        Color purple = new Color(0.31f, 0.27f, 0.9f);
        Color current = Color.Lerp(teal, purple, t);

        mat.SetColor("_EmissionColor", current * intensity);
        mat.EnableKeyword("_EMISSION");
    }
}