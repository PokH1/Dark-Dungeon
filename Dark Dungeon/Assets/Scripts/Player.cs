using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterController controller;
    public float walkSpeed = 15f;
    public float runSpeed = 20f;
    public float gravedad = -10f;
    public float jump = 3f;
    public Transform inOnGround;
    public float distanceFloot;
    public LayerMask layerFloot;
    Vector3 speedDown;
    bool OnGround;
    public int maxHealth = 100;
    private int currentHealth;
    private Animator animator;
    public bool isDie = false;
    public GameObject weaponPrefab;
    public Transform weaponPoint;
    public Weapon weaponScript;

    private GameObject equippedWeapon;
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        EquipedWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        OnGround = Physics.CheckSphere(inOnGround.position, distanceFloot, layerFloot);
        // Debug.Log("En el suelo: " + OnGround);

        if (OnGround && speedDown.y < 0)
        {
            speedDown.y = -2;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Debug.Log("Shift");
        }

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && OnGround)
        {
            Debug.Log("Saltando");
            speedDown.y = Mathf.Sqrt(jump * -2f * gravedad);
        }

        speedDown.y += gravedad * Time.deltaTime;
        controller.Move(speedDown * Time.deltaTime);

        float movimiento = move.magnitude;
        animator.SetFloat("speedWalk", movimiento);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (Input.GetMouseButtonDown(0) && !isDie)
        {
            animator.SetTrigger("attack");
        }

    }


    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Vida del jugador: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("El jugador ha muerto.");
        isDie = true;
        animator.SetTrigger("Die"); // si tienes animación de muerte

        // Aquí puedes desactivar movimiento o mostrar menú de muerte
        GetComponent<Player>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        this.enabled = false; // Desactiva el propio script
    }

    void EquipedWeapon()
    {
        if (equippedWeapon != null)
            Destroy(equippedWeapon);

        equippedWeapon = Instantiate(weaponPrefab, weaponPoint);
        equippedWeapon.transform.localPosition = Vector3.zero;
        equippedWeapon.transform.localRotation = Quaternion.identity;

        weaponScript = equippedWeapon.GetComponent<Weapon>();
        Debug.Log("Arma equipada con script: " + weaponScript);
    }

    public void EnableDamage()
    {
        weaponScript?.EnableDamage();
    }

    public void DisableDamage()
    {
        weaponScript?.DisableDamage();
    }
}
