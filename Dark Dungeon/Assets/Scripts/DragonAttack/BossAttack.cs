using System.Collections;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float timeBetweenAttacks = 1.5f;
    public int damage = 10;
    public GameObject fireballPrefab; // Prefab de la bola de fuego
    public Transform firePoint;
    public float fireballSpeed = 10f; // Velocidad de la bola de fuego
    public float fireballCooldown = 3f; // Tiempo entre lanzamientos de bolas de fuego
    private bool canAttackWithFireball = true;

    private Transform player;
    private CharacterController controller;
    private Animator animator;
    private float gravity = -9.81f;
    private Vector3 velocity;
    private bool alreadyAttacked = false;
    private Player playerHealth;
    public float structureDestructionRadius = 1.5f;
    public LayerMask destructionLayer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<Player>();
        animator.SetTrigger("BattleStance");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth == null || playerHealth.isDie)
        {
            animator.SetTrigger("IdleAgressive");
            return;
        }

        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        // Apuntar hacia el jugador
        if (direction != Vector3.zero)
        {
            Vector3 lookDir = new Vector3(direction.x, 0, direction.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);
        }

        // Movimiento hacia el jugador si está fuera del rango de ataque
        if (distance > attackRange)
        {
            animator.SetBool("Walk", true);
            Vector3 moveDir = direction.normalized;
            velocity.y += gravity * Time.deltaTime;
            controller.Move(moveDir * moveSpeed * Time.deltaTime + velocity * Time.deltaTime);

            DestroyStructuresNearBoss(); // ✅ Aquí la destrucción
        }
        else
        {
            animator.SetBool("Walk", false);
            controller.Move(Vector3.zero); // detener movimiento

            if (!alreadyAttacked && canAttackWithFireball)
            {
                ChooseAttack();
            }
        }
    }

    void AttackWithFireball()
    {
        animator.SetTrigger("FlyingAttack"); // Aquí puedes añadir una animación de lanzar la bola de fuego
        LaunchFireball();

        canAttackWithFireball = false;
        alreadyAttacked = true;

        Invoke(nameof(ResetAttack), timeBetweenAttacks);
        Invoke(nameof(ResetFireballCooldown), fireballCooldown); // Resetear el cooldown
    }

    void LaunchFireball()
    {
        // Instanciar la bola de fuego y darle dirección
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        fireball.transform.LookAt(player);
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = fireball.transform.forward * fireballSpeed;
        }
    }

    IEnumerator LaunchFireballWithDelay()
    {
        // Instanciar la bola de fuego en la boca
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        fireball.transform.LookAt(player);

        // Asegúrate de que esté quieta al principio
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true; // desactiva física temporalmente
        }

        yield return new WaitForSeconds(0.5f); // espera medio segundo (ajustable)

        if (rb != null)
        {
            rb.isKinematic = false; // reactiva la física
            rb.linearVelocity = fireball.transform.forward * fireballSpeed;
        }
    }
    void ResetFireballCooldown()
    {
        canAttackWithFireball = true;
    }

    void AttackPlayer()
    {
        animator.SetTrigger("attack"); // animación de ataque

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    void DestroyStructuresNearBoss()
    {
        Collider[] nearbyStructures = Physics.OverlapSphere(transform.position, structureDestructionRadius, destructionLayer);

        foreach (Collider col in nearbyStructures)
        {
            if (col.CompareTag("Structure"))
            {
                Destroy(col.gameObject);
                Debug.Log("Estructura destruida por el jefe en movimiento.");
            }
        }
    }

    void ChooseAttack()
{
    int randomAttack = Random.Range(0, 2); // 0 o 1

    if (randomAttack == 0 && !alreadyAttacked)
    {
        AttackPlayer(); // ataque cuerpo a cuerpo
    }
    else if (randomAttack == 1 && canAttackWithFireball)
    {
        AttackWithFireball(); // ataque a distancia
    }
}
}
