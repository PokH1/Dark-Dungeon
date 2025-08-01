using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealt = 100;
    public int currentHealt;
    private Animator animator;
    private bool isDead = false;

    // Prefabs de pickups que se ven en el suelo
    public GameObject[] weaponPickups;

    // Prefabs de armas reales que se asignarán al jugador
    public GameObject[] weaponPrefabs;

    public GameObject[] powerUpDrops;

    [Range(0f, 1f)]
    public float weaponDropChance = 1f;

    [Range(0f, 1f)]
    public float poweUpDropChance = 1f;

    void Start()
    {
        currentHealt = maxHealt;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealt -= damage;
        Debug.Log("Vida del enemigo: " + currentHealt);

        if (currentHealt <= 0)
        {
            Die();
        }
    }

    void DropLoot()
    {
        // DROP DE ARMA
        if (Random.value <= weaponDropChance && weaponPickups.Length > 0 && weaponPrefabs.Length > 0)
        {
            int index = Random.Range(0, Mathf.Min(weaponPickups.Length, weaponPrefabs.Length));

            // Instanciar el pickup en el suelo
            GameObject pickupInstance = Instantiate(weaponPickups[index], transform.position, Quaternion.identity);

            // Obtener el script del pickup y asignarle el arma real
            PickupWeapon pickupScript = pickupInstance.GetComponent<PickupWeapon>();
            if (pickupScript != null)
            {
                pickupScript.weaponPrefab = weaponPrefabs[index]; // ✅ Aquí se asigna el prefab real del arma
                Debug.Log("Pickup instanciado con arma: " + weaponPrefabs[index].name);
            }
            else
            {
                Debug.LogWarning("Pickup instanciado pero sin script PickupWeapon.");
            }
        }

        // DROP DE POWER-UP
        if (Random.value <= poweUpDropChance && powerUpDrops.Length > 0)
        {
            int index = Random.Range(0, powerUpDrops.Length);
            Instantiate(powerUpDrops[index], transform.position, Quaternion.identity);
            Debug.Log("El power-up apareció");
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");

        GetComponent<EnemyAttack>().enabled = false;
        GetComponent<CharacterController>().enabled = false;

        DropLoot();

        Destroy(gameObject, 2f);
        Debug.Log("Enemigo muerto x_x");
    }
}
