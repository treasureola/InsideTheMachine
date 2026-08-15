using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler
{
    public Color normalColor = new Color(0.31f, 0.27f, 0.9f);
    public Color hoverColor  = new Color(0.05f, 0.58f, 0.53f);
    public Color clickColor  = Color.white;
    public float animSpeed   = 8f;

    private Image img;
    private Color targetColor;

    void Start()
    {
        img = GetComponent<Image>();
        targetColor = normalColor;
        img.color   = normalColor;
    }

    void Update()
    {
        img.color = Color.Lerp(img.color, targetColor, animSpeed * Time.deltaTime);
    }

    public void OnPointerEnter(PointerEventData e) 
    {
        targetColor = hoverColor;
        transform.localScale = Vector3.one * 1.05f;
    }

    public void OnPointerExit(PointerEventData e)  
    {
        targetColor = normalColor;
        transform.localScale = Vector3.one;
    }

    public void OnPointerDown(PointerEventData e)
    {
        // Sound fires instantly on press, not on release
        AudioManager.Instance?.PlayButtonClick();
        StartCoroutine(ClickFlash());
    }

    public void OnPointerClick(PointerEventData e) 
    {
        // Click flash only, no sound here
    }

    IEnumerator ClickFlash()
    {
        targetColor = clickColor;
        yield return new WaitForSeconds(0.1f);
        targetColor = normalColor;
    }
}