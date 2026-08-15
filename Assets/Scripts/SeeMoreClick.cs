using UnityEngine;

public class SeeMoreClick : MonoBehaviour
{
    public InfoPanelManager infoPanelManager;
    public XRayMode xRayMode;
    public int componentIndex;

    void OnMouseDown()
    {
        // Block during X-Ray mode
        if (xRayMode != null && xRayMode.isXRay)
            return;

        if (infoPanelManager == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        bool isOnLeftSide = screenPos.x < Screen.width / 2f;
        infoPanelManager.ShowPanel(componentIndex, isOnLeftSide);
    }
}