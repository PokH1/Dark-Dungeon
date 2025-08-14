using UnityEngine;
using System.Collections; // Necesario para usar Coroutines

public class PickupShield : MonoBehaviour
{
    public GameObject pickupUI;
    public GameObject shieldPrefab;

    private bool isPlayerNearby = false;
    private Player player;

    // Variables para el sonido
    public AudioSource audioSource;
    public AudioClip pickupSound;
    private bool hasBeenPickedUp = false; // Variable de control para evitar interacciones múltiples

    void Start()
    {
        if (pickupUI != null)
        {
            // Es buena práctica desactivar la UI al inicio si no está el jugador cerca.
            // Aunque tu OnTriggerEnter la activa, esta línea previene que se muestre por defecto.
            pickupUI.SetActive(true);
        }
    }

    void Update()
    {
        // Solo permite la interacción si el jugador está cerca, no se ha recogido y presiona 'E'.
        if (isPlayerNearby && player != null && Input.GetKeyDown(KeyCode.E) && !hasBeenPickedUp)
        {
            // Marca el objeto como "recogido" inmediatamente
            hasBeenPickedUp = true;
            
            // Inicia la corutina para recoger el escudo
            StartCoroutine(PickupAndDestroy());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenPickedUp)
        {
            isPlayerNearby = true;
            player = other.GetComponent<Player>();
            if (pickupUI != null)
                pickupUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenPickedUp)
        {
            isPlayerNearby = false;
            player = null;
            if (pickupUI != null)
                pickupUI.SetActive(false);
        }
    }

    // Corutina para manejar la recogida del escudo y la destrucción
    private IEnumerator PickupAndDestroy()
    {
        // Desactiva el collider para evitar más interacciones
        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().enabled = false;
        }

        // Reproduce el sonido si está asignado
        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
            yield return new WaitForSeconds(pickupSound.length);
        }
        else
        {
            // Si no hay sonido, espera un tiempo mínimo para evitar errores
            yield return new WaitForSeconds(0.1f);
        }

        // Equipa el escudo al jugador
        player.EquipShield(shieldPrefab);
        
        // Desactiva la UI
        if (pickupUI != null)
        {
            pickupUI.SetActive(false);
        }

        // Destruye el objeto de forma segura
        Destroy(gameObject);
    }
}