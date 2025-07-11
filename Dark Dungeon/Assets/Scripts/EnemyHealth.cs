using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealt = 100;
    private int currentHealt;
    private Animator animator;
    private bool isDead = false;

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

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");

        GetComponent<EnemyAttack>().enabled = false;
        GetComponent<CharacterController>().enabled = false;

        Destroy(gameObject, 0f);
        Debug.Log("Enemigo muerto x_x");
    }
}
