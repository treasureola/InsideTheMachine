using UnityEngine;
using TMPro;

public class ComponentLabel : MonoBehaviour
{
    [Header("Label Settings")]
    public string componentName = "Component";
    public string description = "Description goes here.";
    public float heightOffset = 6f;

    [Header("Name Label")]
    public float nameFontSize = 10f;
    public Color nameColor = new Color(0.05f, 0.58f, 0.53f);
    public FontStyles nameStyle = FontStyles.Bold;

    [Header("Description Label")]
    public float descFontSize = 5f;
    public Color descColor = new Color(0.9f, 0.9f, 0.9f);
    public FontStyles descStyle = FontStyles.Normal;

    [Header("See More Button")]
    public float btnFontSize = 4f;
    public Color btnColor = new Color(0.31f, 0.27f, 0.9f);
    public FontStyles btnStyle = FontStyles.Bold;

    [Header("Outline")]
    public float outlineWidth = 0.2f;
    public Color outlineColor = Color.black;

    private GameObject nameObj;
    private GameObject descObj;
    private GameObject btnObj;

    void Start()
    {
        nameObj = CreateLabel("Name_" + componentName, componentName.ToUpper(), nameColor, nameFontSize, nameStyle, heightOffset);
        descObj = CreateLabel("Desc_" + componentName, description, descColor, descFontSize, descStyle, heightOffset - 2f);
        btnObj = CreateLabel("Btn_" + componentName, "[ SEE MORE ]", btnColor, btnFontSize, btnStyle, heightOffset - 4f);
    }

    GameObject CreateLabel(string objName, string text, Color color, float fontSize, FontStyles style, float yOffset)
    {
        GameObject obj = new GameObject(objName);
        obj.transform.SetParent(this.transform);
        obj.transform.localRotation = Quaternion.identity;

        TextMeshPro tmp = obj.AddComponent<TextMeshPro>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = style;
        tmp.outlineWidth = outlineWidth;
        tmp.outlineColor = new Color32(
            (byte)(outlineColor.r * 255),
            (byte)(outlineColor.g * 255),
            (byte)(outlineColor.b * 255),
            255);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(20f, 4f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        obj.AddComponent<LookAtCamera>();
        return obj;
    }

    void Update()
    {
        if (nameObj != null)
            nameObj.transform.position = this.transform.position + Vector3.up * heightOffset;
        if (descObj != null)
            descObj.transform.position = this.transform.position + Vector3.up * (heightOffset - 2f);
        if (btnObj != null)
            btnObj.transform.position = this.transform.position + Vector3.up * (heightOffset - 4f);
    }

    public void HideLabel() { nameObj?.SetActive(false); descObj?.SetActive(false); btnObj?.SetActive(false); }
    public void ShowLabel() { nameObj?.SetActive(true); descObj?.SetActive(true); btnObj?.SetActive(true); }
}

public class LookAtCamera : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main != null)
            transform.forward = Camera.main.transform.forward;
    }
}