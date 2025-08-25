using System.Collections;
using UnityEngine;

public class PickupWeapon : MonoBehaviour
{

    public ulong itemId;
    public bool isNFT;
    public GameObject pickupUI;
    public GameObject weaponPrefab;
    public AudioClip pickupSound;
    public AudioSource audioSource;

    private bool isPlayerNearby = false;
    private Player player;
    private bool hasBeenPickedUp = false;

    void Start()
    {
        if (pickupUI != null)
        {
            pickupUI.SetActive(false);
        }
    }

    void Update()
    {
        // Solo permite la interacción si no ha sido recogido
        if (isPlayerNearby && player != null && !hasBeenPickedUp && !HasWeaponInInventory())
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Inmediatamente establece el "candado" para evitar más interacciones
                hasBeenPickedUp = true;

                // Inicia la corutina para manejar la recogida y la destrucción
                StartCoroutine(PickupWeaponAndDestroy());
            }
        }
        else if (isPlayerNearby && HasWeaponInInventory())
        {
            if (pickupUI != null)
            {
                pickupUI.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenPickedUp)
        {
            isPlayerNearby = true;
            player = other.GetComponent<Player>();

            if (pickupUI != null && !HasWeaponInInventory())
            {
                pickupUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenPickedUp)
        {
            isPlayerNearby = false;
            player = null;
            if (pickupUI != null)
            {
                pickupUI.SetActive(false);
            }
        }
    }

    private IEnumerator PickupWeaponAndDestroy()
    {
        hasBeenPickedUp = true;

        // Desactiva el collider y la UI inmediatamente
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        if (pickupUI != null)
        {
            pickupUI.SetActive(false);
        }

        // Reproduce el sonido y espera a que termine
        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
            yield return new WaitForSeconds(pickupSound.length);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Agrega el item a la lista del jugador
        if (player != null)
        {
            if (isNFT)
            {
                Debug.Log("NFT agregado con ID: " + itemId);
                player.ItemFound(itemId); // <-- aquí es correcto                
            }

            player.EquipNewWeapon(weaponPrefab);
        }
        // Destruye el objeto
        Destroy(gameObject);
    }

        // Nuevo método para comprobar si el jugador ya tiene el arma
    private bool HasWeaponInInventory()
    {
        if (player == null || weaponPrefab == null)
        {
            return false;
        }

        // Comprueba el nombre del prefab para ver si ya está en el inventario
        foreach (GameObject weapon in player.weaponInventory)
        {
            if (weapon != null && weapon.name.Replace("(Clone)", "") == weaponPrefab.name)
            {
                return true;
            }
        }

        return false;
    }
}