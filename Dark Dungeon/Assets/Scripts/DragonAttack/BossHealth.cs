using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public int maxHealt = 100;
    public int currentHealt;
    private Animator animator;
    private bool isDead = false;
    public Slider healthSliderDragon;
    public GameObject bossHealthBarCanvas;
    public BossAttack bossAttack;
    public AudioSource deathAudioSource;  // Sonido de muerte
    [Header("Boss Drop NFTs")]
    public GameObject[] bossDrops;
    [Range(0f, 1f)]
    public float dropChance = 0.5f;

    void Start()
    {
        currentHealt = maxHealt;

        if (healthSliderDragon != null)
        {
            healthSliderDragon.maxValue = maxHealt;
            healthSliderDragon.value = currentHealt;
        }
        animator = GetComponent<Animator>();
        bossAttack = GetComponent<BossAttack>();

        // Die();
        
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        // ⬅️ Si la barra de vida no está activa, la activamos en el primer golpe.
        if (bossHealthBarCanvas != null && !bossHealthBarCanvas.activeSelf)
        {
            bossHealthBarCanvas.SetActive(true);
        }

        currentHealt -= damage;
        currentHealt = Mathf.Clamp(currentHealt, 0, maxHealt);
        healthSliderDragon.value = currentHealt;
        Debug.Log("Vida del Dragon: " + currentHealt);

        if (currentHealt <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");

        GetComponent<BossAttack>().enabled = false;
        GetComponent<CharacterController>().enabled = false;

        float deathAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Invoke(nameof(DestroyBoss), deathAnimationLength);

        // ⬅️ Desactivamos la barra de vida cuando el jefe muere.
        if (bossHealthBarCanvas != null)
        {
            bossHealthBarCanvas.SetActive(false);
        }

        if (bossAttack != null)
        {
            bossAttack.StopFlyingSound();
        }

        if (deathAudioSource != null)
        {
            deathAudioSource.Play();
        }

        DropItem();

        Debug.Log("Has vencido al jefe");
    }

    void DropItem()
    {
        if (bossDrops.Length == 0) return;

        if (Random.value <= dropChance)
        {
            int index = Random.Range(0, bossDrops.Length);
            Instantiate(bossDrops[index], transform.position, Quaternion.identity);
            Debug.Log("Boss solto un item");
        }
    }

    void DestroyBoss()
    {
        Destroy(gameObject);
    }
}
