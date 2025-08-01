using UnityEngine;

public class PickupWeapon : MonoBehaviour
{
    public GameObject pickupUI; // Este debe estar en la escena, no dentro del arma
    public GameObject weaponPrefab;
    // public float lifeTime = 5;

    private bool isPlayerNearby = false;
    private Player player;
    private float timer;

    void Start()
    {
        if (weaponPrefab.scene.name != null)
        {
            Debug.LogWarning("¡Estás usando una instancia de escena como prefab! Se destruirá junto al pickup.");
        }
        else
        {
            Debug.Log("Weapon es un prefab del proyecto");
        }

        // timer = lifeTime;

        if (pickupUI != null)
        {
            pickupUI.SetActive(true);
        }
    }

    void Update()
    {
        // timer -= Time.deltaTime;

        // if (timer <= 0)
        // {
        //     if (pickupUI != null)
        //     {
        //         pickupUI.SetActive(false);
        //     }

        //     Destroy(gameObject);
        // }
        if (isPlayerNearby && player != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                player.EquipNewWeapon(weaponPrefab);

                // Ocultar el texto justo antes de destruir el objeto
                if (pickupUI != null)
                    pickupUI.SetActive(false);

                // Desactivar antes de destruir para evitar errores de referencia
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            player = other.GetComponent<Player>();

            if (pickupUI != null)
                pickupUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            player = null;

            if (pickupUI != null)
                pickupUI.SetActive(false);
        }
    }
}
