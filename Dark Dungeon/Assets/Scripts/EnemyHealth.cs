using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject healthSliderPrefab;
    private Slider healthSliderInstance;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip idleGrowlSound;
    public AudioSource loopAudioSource;
    public AudioSource audioSource;

    [Header("NFTs")]
    public GameObject[] nfts;
    [Range(0f, 1f)]
    public float nftsItemDropChance = 0.05f;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        loopAudioSource = gameObject.AddComponent<AudioSource>();
        loopAudioSource.clip = idleGrowlSound;
        loopAudioSource.playOnAwake = false;

        if (idleGrowlSound != null)
        {
            loopAudioSource.PlayOneShot(idleGrowlSound);
        }

        currentHealt = maxHealt;
        // Instancia el prefab del slider.
        GameObject sliderGO = Instantiate(healthSliderPrefab, transform.position, Quaternion.identity, transform);

        // Obtiene la referencia al componente Slider dentro del objeto instanciado.
        healthSliderInstance = sliderGO.GetComponentInChildren<Slider>();

        if (healthSliderInstance != null)
        {
            healthSliderInstance.gameObject.SetActive(true);
            healthSliderInstance.maxValue = maxHealt;
            healthSliderInstance.value = currentHealt;
        }

        animator = GetComponent<Animator>();
    }

    // void Update()
    // {
    //     if (healthSliderInstance != null && healthSliderInstance.activeSelf)
    //     {
    //         Vector3 headPosition = transform.position + Vector3.up * 2;
    //         healthSliderInstance.transform.position = headPosition;
    //     }
    // }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealt -= damage;
        Debug.Log("Vida del enemigo: " + currentHealt);
        currentHealt = Mathf.Clamp(currentHealt, 0, maxHealt);

        if (healthSliderInstance != null)
        {
            healthSliderInstance.value = currentHealt;
        }

        if (hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

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

        if (nfts.Length > 0 && Random.value <= nftsItemDropChance)
        {
            int nftsIndex = Random.Range(0, nfts.Length);
            Instantiate(nfts[nftsIndex], transform.position, Quaternion.identity);
            Debug.Log("El NFTs aparecio");
        }
    }

    void Die()
    {
        isDead = true;

        animator.SetTrigger("Die");

        GetComponent<EnemyAttack>().enabled = false;
        GetComponent<CharacterController>().enabled = false;

        DropLoot();

        if (healthSliderInstance != null)
        {
            Destroy(healthSliderInstance);
        }

        StartCoroutine(PlayDeathAndDestroy());
    }
    
    private IEnumerator PlayDeathAndDestroy()
    {
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
            yield return new WaitForSeconds(deathSound.length); // ⏳ Espera la duración del sonido
        }

        Destroy(gameObject);
        Debug.Log("Enemigo muerto x_x");
    }
}
