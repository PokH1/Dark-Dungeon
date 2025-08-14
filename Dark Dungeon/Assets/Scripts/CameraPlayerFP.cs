using System;
using UnityEngine;

public class CameraPlayerFP : MonoBehaviour
{
    public float speed = 100;
    public Transform Player;
    float rotationX = 0f;
    public LayerMask collisionMask;
    public float cameraRadius = 0.2f;
    public float cameraDistance = 0.1f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (Player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
            {
                Player = foundPlayer.transform;
            }
            else
            {
                Debug.LogError("No se encontró ningún objeto con la etiqueta 'Player'.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * speed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * speed * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Math.Clamp(rotationX, -90, 90);

        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        Player.Rotate(Vector3.up * mouseX);
    }

    void LateUpdate()
    {
            Vector3 origin = Player.position + Vector3.up * 1.6f; // altura de ojos
        Vector3 direction = transform.position - origin;

        if (Physics.SphereCast(origin, cameraRadius, direction.normalized, out RaycastHit hit, direction.magnitude, collisionMask))
        {
            transform.position = hit.point + hit.normal * cameraDistance;
        }
    }
}
