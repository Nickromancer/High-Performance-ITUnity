using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 30f;
    public float fastMultiplier = 2f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    public float verticalClamp = 85f; // Prevent flipping upside down

    private float pitch = 0f; // X rotation (looking up/down)
    private float yaw = 0f;   // Y rotation (looking left/right)

    void Start()
    {
        // Initialize rotation from current camera rotation
        Vector3 euler = transform.eulerAngles;
        yaw = euler.y;
        pitch = euler.x;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    void HandleMovement()
    {
        float speed = moveSpeed;

        // Left shift for faster movement
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= fastMultiplier;

        Vector3 direction = Vector3.zero;

        // WASD movement (camera relative)
        if (Input.GetKey(KeyCode.W)) direction += transform.forward;
        if (Input.GetKey(KeyCode.S)) direction -= transform.forward;
        if (Input.GetKey(KeyCode.A)) direction -= transform.right;
        if (Input.GetKey(KeyCode.D)) direction += transform.right;

        // Q/E vertical movement
        if (Input.GetKey(KeyCode.E)) direction += Vector3.up;
        if (Input.GetKey(KeyCode.Q)) direction -= Vector3.up;

        transform.position += direction * speed * Time.deltaTime;
    }

    void HandleMouseLook()
    {
        // Only rotate camera while holding RIGHT mouse button
        if (Input.GetMouseButton(1))
        {
            // Lock cursor ONLY while rotating (feels natural in editor)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            yaw += mouseX;
            pitch -= mouseY;

            // Clamp vertical rotation to prevent flipping
            pitch = Mathf.Clamp(pitch, -verticalClamp, verticalClamp);

            // Apply rotation
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        else
        {
            // Unlock cursor when not looking around
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
