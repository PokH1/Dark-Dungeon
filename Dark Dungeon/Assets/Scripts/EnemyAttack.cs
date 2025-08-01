using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float timeBetweenAttacks = 1.5f;
    public int damage = 10;

    private Transform player;
    private CharacterController controller;
    private Animator animator;
    private float gravity = -9.81f;
    private Vector3 velocity;
    private bool alreadyAttacked = false;
    private Player playerHealth;
    public GameObject weapon;
    public Transform weaponPoint;
    private GameObject equippedWeapon;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<Player>();
        EquipWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth == null || playerHealth.isDie)
        {
            animator.SetTrigger("rug");
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
            animator.SetBool("isMoving", true);
            Vector3 moveDir = direction.normalized;
            velocity.y += gravity * Time.deltaTime;
            controller.Move(moveDir * moveSpeed * Time.deltaTime + velocity * Time.deltaTime);
        }
        else
        {
            animator.SetBool("isMoving", false);
            controller.Move(Vector3.zero); // detener movimiento

            if (!alreadyAttacked)
            {
                AttackPlayer();
            }
        }
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

    void EquipWeapon()
    {
        equippedWeapon = Instantiate(weapon, weaponPoint.position, weaponPoint.rotation);
        equippedWeapon.transform.SetParent(weaponPoint);  // Hacemos que el arma sea hija del transform del punto
        equippedWeapon.transform.localPosition = Vector3.zero;  // Ajustamos la posición
        equippedWeapon.transform.localRotation = Quaternion.identity;  // Ajustamos la rotación
    }
}
