using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int maxHealt = 100;
    public int currentHealt;
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        currentHealt = maxHealt;
        animator = GetComponent<Animator>();

        // Die();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealt -= damage;
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

        Debug.Log("Has vencido al jefe");
    }

    void DestroyBoss()
    {
        Destroy(gameObject);
    }
}
