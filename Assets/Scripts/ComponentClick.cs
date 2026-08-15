using UnityEngine;

public class ComponentClick : MonoBehaviour
{
    public AssemblyManager assemblyManager;
    public InfoPanelManager infoPanelManager;
    public XRayMode xRayMode;
    public int componentIndex;

    private static float lastUIClickTime = -1f;
    private const float UI_CLICK_COOLDOWN = 0.3f;

    void OnMouseDown()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            lastUIClickTime = Time.time;
            return;
        }

        if (Time.time - lastUIClickTime < UI_CLICK_COOLDOWN)
            return;

        // Block ALL clicks during X-Ray mode
        if (xRayMode != null && xRayMode.isXRay)
            return;

        if (assemblyManager != null)
            assemblyManager.OnComponentClicked(componentIndex);

        if (infoPanelManager != null &&
            assemblyManager != null &&
            !assemblyManager.assemblyActive)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            bool isOnLeftSide = screenPos.x < Screen.width / 2f;
            infoPanelManager.ShowPanel(componentIndex, isOnLeftSide);
        }
    }
}