using UnityEngine;

public class LaunchArrow : MonoBehaviour
{
    public GameObject arrow;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform respawnArrow, chargeArrow;
    public GameObject crosshairUI;
    private CameraController camController;
    private bool IsUsingBow;

    private float power = 10;
    public float force;
    public int cantArrows;

    void Start()
    {
        camController = FindObjectOfType<CameraController>();
    }
    private void Update()
    {
        IsUsingBow = GetComponent<Player>().equippedWeapon != null && GetComponent<Player>().equippedWeapon.CompareTag("Bow");

        if (IsUsingBow)
        {

            crosshairUI.SetActive(true);

            float movement = 0.5f + force;

            // Recargar flecha con R
            if (cantArrows > 0 && arrow == null && Input.GetKeyDown(KeyCode.R))
            {
                arrow = Instantiate(arrowPrefab, respawnArrow.position, respawnArrow.rotation);
                arrow.transform.SetParent(respawnArrow);
            }

            // Cargando la flecha y apuntando
            if (Input.GetMouseButton(0) && arrow != null)
            {
                crosshairUI.SetActive(true);
                power += 0.2f + force;
                power = Mathf.Clamp(power, 0f, 30f);

                ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
                if (arrowScript != null)
                {
                    arrowScript.powerFinal = power;
                }

                arrow.transform.position = Vector3.Lerp(
                    arrow.transform.position,
                    chargeArrow.position,
                    movement * Time.deltaTime
                );
            }
            else
            {
                crosshairUI.SetActive(false);
            }

            // Lanzando la flecha
            if (Input.GetMouseButtonUp(0) && arrow != null)
            {
                Rigidbody rb = arrow.GetComponent<Rigidbody>();
                ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();

                if (rb != null)
                {
                    // 👉 CORREGIDO: usar .velocity y dirección del respawn
                    rb.isKinematic = false;

                    Camera cam = camController.currentCamera;

                    Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
                    Vector3 direction = ray.direction;

                    rb.linearVelocity = direction * power; // Cambia por .right si quieres eje X
                    arrow.transform.right = direction.normalized;
                }

                if (arrowScript != null)
                {
                    arrowScript.arrowInFly = true;
                }

                arrow.transform.SetParent(null);
                arrow = null;
                power = 0f;
                cantArrows -= 1;
            }
        }
        else
        {
            crosshairUI.SetActive(false);
        }

       
    }
}
