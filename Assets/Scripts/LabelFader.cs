using UnityEngine;
using TMPro;

public class LabelFader : MonoBehaviour
{
    public float visibleDistance = 25f;
    public float fadeDistance = 35f;
    private TextMeshPro tmp;
    private Camera cam;

    void Start()
    {
        tmp = GetComponent<TextMeshPro>();
        cam = Camera.main;
    }

    void Update()
    {
        if (tmp == null || cam == null) return;

        // Use Z distance only so left/right components fade at the same time
        float dist = Mathf.Abs(cam.transform.position.z - transform.position.z);
        float alpha;

        if (dist <= visibleDistance)
            alpha = 1f;
        else if (dist >= fadeDistance)
            alpha = 0f;
        else
            alpha = 1f - ((dist - visibleDistance) / (fadeDistance - visibleDistance));

        Color c = tmp.color;
        c.a = alpha;
        tmp.color = c;
    }
}