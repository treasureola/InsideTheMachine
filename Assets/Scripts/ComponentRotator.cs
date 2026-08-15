using UnityEngine;

public class ComponentRotator : MonoBehaviour
{
    public float autoRotateSpeed = 30f;
    public bool isRotating = true;

    private bool isDragging = false;
    private Vector3 lastMousePosition;
    private float dragRotateSpeed = 0.3f;

    void Update()
    {
        if (!isDragging && isRotating)
        {
            transform.Rotate(0, autoRotateSpeed * Time.deltaTime, 0, Space.Self);
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        lastMousePosition = Input.mousePosition;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            transform.Rotate(Vector3.up, -delta.x * dragRotateSpeed, Space.Self);
            transform.Rotate(Vector3.right, delta.y * dragRotateSpeed, Space.Self);
            lastMousePosition = Input.mousePosition;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    public void StopRotation()
    {
        isRotating = false;
    }

    public void StartRotation()
    {
        isRotating = true;
    }
}