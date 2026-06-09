using UnityEngine;
using UnityEngine.InputSystem;

public class FG_CameraController : MonoBehaviour
{
    public Transform target;
    public float distance = 8.0f;
    public float height = 4.0f;
    public float sensitivity = 0.5f;

    private float currentX = 0.0f;
    private float currentY = 20.0f; // start looking slightly down

    void Start()
    {
        // Optionally lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (Mouse.current != null)
        {
            currentX += Mouse.current.delta.x.ReadValue() * sensitivity;
            currentY -= Mouse.current.delta.y.ReadValue() * sensitivity;
            currentY = Mathf.Clamp(currentY, -10f, 70f);
        }

        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = target.position + new Vector3(0, height / 2f, 0) + rotation * dir;
        transform.LookAt(target.position + new Vector3(0, height / 2f, 0));
    }
}
