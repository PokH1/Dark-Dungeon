using UnityEngine;

public class PickupShield : MonoBehaviour
{
    public GameObject pickupUI; // UI de "Presiona E para recoger"
    public GameObject shieldPrefab; // Prefab del escudo a equipar

    private bool isPlayerNearby = false;
    private Player player;

    void Start()
    {
        if (pickupUI != null)
            pickupUI.SetActive(true);
    }

    void Update()
    {
        if (isPlayerNearby && player != null && Input.GetKeyDown(KeyCode.E))
        {
            player.EquipShield(shieldPrefab);

            if (pickupUI != null)
                pickupUI.SetActive(false);

            Destroy(gameObject);
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
