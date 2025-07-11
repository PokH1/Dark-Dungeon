using UnityEngine;

public class CameraPlayerThird : MonoBehaviour
{
    public Transform player;              // El jugador
    public float distance = 5f;           // Qué tan lejos está la cámara
    public float height = 2f;             // Qué tan arriba de la cabeza está
    public float mouseSensitivity = 3f;   // Sensibilidad del mouse
    public float rotationYMin = -40f;
    public float rotationYMax = 60f;

    private float rotationX = 0f;         // Vertical
    private float rotationY = 0f;         // Horizontal

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;
    }

    void LateUpdate()
    {
        // Entrada del mouse
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationY = Mathf.Clamp(rotationY, rotationYMin, rotationYMax);

        // Rotación basada en el mouse
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);

        // Calcular posición de la cámara
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        Vector3 desiredPosition = player.position + Vector3.up * height + offset;

        transform.position = desiredPosition;
        transform.rotation = rotation;

        player.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity);
    }
}
