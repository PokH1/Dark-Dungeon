using UnityEngine;
using TMPro;
using System.Collections;

public class LaunchArrow : MonoBehaviour, IWeaponUI
{
    public GameObject arrow;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform respawnArrow, chargeArrow;
    public GameObject crosshairUI;
    private CameraController camController;

    private float power = 10;
    public float force;
    public int cantArrows;
    [SerializeField] public Sprite icon;
    public Sprite GetIcon() => icon;
    public Player player;

    private bool isCharging = false;
    private Vector3 arrowVelocity = Vector3.zero;

    void Start()
    {
        camController = FindObjectOfType<CameraController>();
    }

    private void Update()
    {
        if (player.equippedWeapon == null || !player.equippedWeapon.CompareTag("Bow"))
        {
            crosshairUI.SetActive(false);
            isCharging = false; // Asegúrate de resetear el estado
            return;
        }

        // Lógica de carga solo si el estado es 'isCharging'
        if (isCharging)
        {
            crosshairUI.SetActive(true);
            power += 0.2f + force;
            power = Mathf.Clamp(power, 0f, 30f);

            if (arrow != null)
            {
                ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
                if (arrowScript != null)
                {
                    arrowScript.powerFinal = power;
                }    
            }

            float speed = 5f;
            arrow.transform.position = Vector3.Lerp(
                arrow.transform.position,
                chargeArrow.position,
                Time.deltaTime * speed
            );
            
        }
        else
        {
            crosshairUI.SetActive(false);
        }
    }
    
    // Método público para iniciar la carga (llamado desde Player.cs)
    public void StartCharging()
    {
        if (arrow != null)
        {
            isCharging = true;

            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            arrow.transform.SetParent(respawnArrow);
            arrow.transform.localPosition = Vector3.zero;
            arrow.transform.localRotation = Quaternion.Euler(0f,180f,0f);

            Debug.Log("Comenzando a cargar el arco: " + arrow.name);
        }
    }

    // Método público para disparar la flecha (llamado desde Player.cs)
    public void ShootArrow()
    {
        if (isCharging && arrow != null)
        {
            isCharging = false;

            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            Collider col = arrow.GetComponent<Collider>();

            if (rb != null && col != null)
            {
                arrow.transform.SetParent(null);
                rb.isKinematic = false;
                col.enabled = true;
                col.isTrigger = false;
                // rb.useGravity = true;

                Vector3 targetPoint;
                Camera cam = camController.currentCamera;
                Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f))
                    targetPoint = hitInfo.point;
                else
                    targetPoint = ray.origin + ray.direction * 30;

                Vector3 direction = (targetPoint - arrow.transform.position).normalized;
                rb.linearVelocity = direction * power;

                if (direction != Vector3.zero)
                    arrow.transform.right = -direction.normalized;

                // Ahora ignoramos colisiones por un tiempo MUY corto para evitar que choque con el arco al salir
                StartCoroutine(TemporarilyIgnoreCollisions(col, player.GetComponent<Collider>(), 0.1f));
                if (player.equippedWeapon != null)
                {
                    StartCoroutine(TemporarilyIgnoreCollisions(col, player.equippedWeapon.GetComponent<Collider>(), 0.1f));
                }
            }

            ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
            if (arrowScript != null)
                arrowScript.arrowInFly = true;

            arrow = null;
            power = 0f;
            cantArrows--;
            player.UpdateArrowCountUI();
        }
    }
    public void LoadArrow()
    {
        if (cantArrows > 0 && arrow == null)
        {
            arrow = Instantiate(arrowPrefab, respawnArrow.position, respawnArrow.rotation, respawnArrow);

            Collider arrowCollider = arrow.GetComponent<Collider>();
            Rigidbody rb = arrow.GetComponent<Rigidbody>();

            // Posición y rotación limpias
            arrow.transform.localPosition = Vector3.zero;
            arrow.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            arrow.transform.localScale = Vector3.one;

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (arrowCollider != null)
            {
                arrowCollider.enabled = false;  // desactivar collider para evitar colisiones
                // arrowCollider.isTrigger = true; // para que no choque mientras está en arco
            }

            ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
            if (arrowScript != null)
            {
                arrowScript.launchArrowScript = this;
            }

            IgnorePlayerAndWeaponCollisions(arrowCollider);
            Debug.Log("Flecha Cargada: " + arrow.name);
        }
    }

    // Coroutine para ignorar colisiones temporalmente
    private IEnumerator TemporarilyIgnoreCollisions(Collider col1, Collider col2, float delay)
    {
        if (col1 != null && col2 != null)
        {
            Physics.IgnoreCollision(col1, col2, true);
            yield return new WaitForSeconds(delay);
            Physics.IgnoreCollision(col1, col2, false);
        }
    }

    void IgnorePlayerAndWeaponCollisions(Collider arrowCollider)
    {
        if (arrowCollider == null) return;

        Collider playerCollider = player.GetComponent<Collider>();
        if (playerCollider != null)
            Physics.IgnoreCollision(arrowCollider, playerCollider, true);

        if (player.equippedWeapon != null)
        {
            Collider weaponCollider = player.equippedWeapon.GetComponent<Collider>();
            if (weaponCollider != null)
                Physics.IgnoreCollision(arrowCollider, weaponCollider, true);
        }
    }
    public void AddArrows(int amount)
    {
        cantArrows += amount;
        player.UpdateArrowCountUI();
    }
}