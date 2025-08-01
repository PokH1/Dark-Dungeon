using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public int currentHealth;
    private Animator animator;
    public bool isDie = false;
    public GameObject weaponPrefab;
    public Transform weaponPoint;
    public Transform leftHandSlot;
    public Weapon weaponScript;
    public GameObject currentShield;
    public GameObject shieldInstance;
    public GameObject shieldPrefab;
    private bool isBlocking = false;
    public int maxShieldHealth = 10;
    private int currentShieldHealth;
    public GameObject equippedWeapon;
    private float originalSpeed;
    private bool isInvincible = false;
    private float originalRunSpeed;
    private float originalWalkSpeed;
    private float damageMultiplier = 1f;
    public List<GameObject> weaponInventory = new List<GameObject>();
    private int currentWeaponIndex = 0;

    void Start()
    {
        currentHealth = maxHealth;
        currentShieldHealth = maxShieldHealth;
        animator = GetComponent<Animator>();
        GameObject initialWeapon =  EquipedWeapon();
        weaponInventory.Add(initialWeapon);

        if (shieldPrefab != null)
            EquipShield(shieldPrefab);

        originalRunSpeed = runSpeed;
        originalWalkSpeed = walkSpeed;
    }

    void Update()
    {
        for (int i = 1; i <= 5; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                EquipWeaponByIndex(i - 1);
            }
        }
        OnGround = Physics.CheckSphere(inOnGround.position, distanceFloot, layerFloot);

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

        HandleShield();

        if (currentShield != null)
        {
            bool shouldBeVisible = !IsUsingBow();
            currentShield.SetActive(shouldBeVisible);
        }
    }


    public void TakeDamage(int amount)
    {
        if (isInvincible)
        {
            Debug.Log("Jugador invencible. No recibe daño");
            return;
        }

        if (isBlocking && currentShield != null)
        {
            currentShieldHealth -= amount;
            Debug.Log("El escudo recibio: " + amount + " de daño");

            if (currentShieldHealth <= 0)
            {
                DestroyShield();
            }
        }
        else
        {
            currentHealth -= amount;
            Debug.Log("Vida del jugador: " + currentHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    
    }

    public void DestroyShield()
    {
        Debug.Log("Escudo destruido");
        if (currentShield != null)
        {
            Destroy(currentShield);
        }

        isBlocking = false;
        currentShield = null;
        animator.SetBool("isBlocking", false);
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

    GameObject EquipedWeapon()
    {
        if (equippedWeapon != null)
            equippedWeapon.SetActive(false);

        equippedWeapon = Instantiate(weaponPrefab, weaponPoint);
        // equippedWeapon.transform.localPosition = Vector3.zero;
        // equippedWeapon.transform.localRotation = Quaternion.identity;

        weaponScript = equippedWeapon.GetComponent<Weapon>();
        Debug.Log("Arma equipada con script: " + weaponScript);

        return equippedWeapon;
    }

    //     // Método para restaurar salud o escudo si lo deseas
    // public void HealPlayer(int amount)
    // {
    //     currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    // }

    // public void RepairShield(int amount)
    // {
    //     currentShieldHealth = Mathf.Min(currentShieldHealth + amount, maxShieldHealth);
    // }

    public void EnableDamage()
    {
        weaponScript?.EnableDamage();
    }

    public void DisableDamage()
    {
        weaponScript?.DisableDamage();
    }

    public void BoostSpeed(float amount, float duration)
    {
        walkSpeed += amount;
        runSpeed += amount;
        StartCoroutine(ResetSpeedAfter(duration));
    }

    IEnumerator ResetSpeedAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        walkSpeed = originalWalkSpeed;
        runSpeed = originalRunSpeed;
    }

    public void IncreaseDamage(float multiplier, float duration)
    {
        damageMultiplier = multiplier;
        if (weaponScript != null)
            weaponScript.SetDamageMultiplier(damageMultiplier);

        StartCoroutine(ResetDamageAfter(duration));
    }

    IEnumerator ResetDamageAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        damageMultiplier = 1f;
        if (weaponScript != null)
            weaponScript.SetDamageMultiplier(damageMultiplier);
    }

    public void Invencible(float duration)
    {
        if (isInvincible) return;

        isInvincible = true;
        StartCoroutine(ResetInvincibility(duration));
    }

    IEnumerator ResetInvincibility(float duration)
    {
        float flashInterval = 0.2f;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            foreach (Renderer rend in renderers)
                rend.enabled = false;

            yield return new WaitForSeconds(flashInterval);

            foreach (Renderer rend in renderers)
                rend.enabled = true;

            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval * 2;
        }

        isInvincible = false;
    }

    public void SpeedDown(float amount, float duration)
    {
        walkSpeed = Mathf.Max(1f, walkSpeed - amount);
        runSpeed = Mathf.Max(1f, runSpeed - amount);
        StartCoroutine(ResetSpeedAfter(duration));

    }

    public void EquipNewWeapon(GameObject newWeaponPrefab)
    {
        if (weaponInventory.Count >= 5)
        {
            Debug.Log("Inventario lleno. No puedes llevar mas armas.");
            return;
        }

        GameObject newWeapon = Instantiate(newWeaponPrefab, weaponPoint);
        // newWeapon.transform.localPosition = Vector3.zero;
        // newWeapon.transform.localRotation = Quaternion.identity;
        newWeapon.SetActive(false); // Se activará cuando se seleccione

        weaponInventory.Add(newWeapon);
        Debug.Log("Arma añadida al inventario");

        if (weaponInventory.Count == 1)
        {
            // Equipar la primera arma automáticamente
            EquipWeaponByIndex(0);
        }
    }

    void EquipWeaponByIndex(int index)
    {
        if (index < 0 || index >= weaponInventory.Count) return;

        // Desactivar arma anterior y scripts
        if (equippedWeapon != null)
        {
            equippedWeapon.SetActive(false);

            Weapon oldWeaponScript = equippedWeapon.GetComponent<Weapon>();
            if (oldWeaponScript != null) oldWeaponScript.enabled = false;

            LaunchArrow oldBowScript = equippedWeapon.GetComponent<LaunchArrow>();
            if (oldBowScript != null) oldBowScript.enabled = false;
        }

        equippedWeapon = weaponInventory[index];
        equippedWeapon.SetActive(true);

        if (equippedWeapon.CompareTag("Bow"))
        {
            LaunchArrow bowScript = equippedWeapon.GetComponent<LaunchArrow>();
            if (bowScript != null)
            {
                bowScript.enabled = true;
                weaponScript = null;
            }
        }
        else
        {
            Weapon newWeaponScript = equippedWeapon.GetComponent<Weapon>();
            if (newWeaponScript != null)
            {
                newWeaponScript.enabled = true;
                weaponScript = newWeaponScript;
            }

            // Solo actualizas el script y el índice si NO es arco
            currentWeaponIndex = index;
        }

        Debug.Log("Arma equipada: " + equippedWeapon.name);
    }


    public void EquipShield(GameObject shieldPrefab)
    {

        if (currentShield != null)
            Destroy(currentShield);

        shieldInstance = Instantiate(shieldPrefab, leftHandSlot);
        // shieldInstance.transform.localPosition = Vector3.zero;
        // shieldInstance.transform.localRotation = Quaternion.identity;

        currentShield = shieldInstance;
        currentShield.SetActive(true);

        currentShieldHealth = maxShieldHealth;
    }

    void HandleShield()
    {

        if (Input.GetMouseButton(1))
        {
            if (!IsUsingBow())
            {
                ActivateShield(true);
            }
            else
            {
                ActivateShield(false);
            }
        }
        else
        {
            ActivateShield(false);
        }
    }
    bool IsUsingBow()
    {
        return equippedWeapon != null && equippedWeapon.CompareTag("Bow");
    }

    void ActivateShield(bool active)
    {

        if (currentShield == null) return;

        isBlocking = active;
        animator.SetBool("isBlocking", active);

        ShieldSettings settings = currentShield.GetComponent<ShieldSettings>();
        if (settings == null) return;

        if (settings.originalPosition == Vector3.zero && settings.originalRotation == Vector3.zero)
        {
            settings.originalPosition = currentShield.transform.localPosition;
            settings.originalRotation = currentShield.transform.localEulerAngles;
        }

        if (active)
        {
            currentShield.transform.localPosition = settings.protectPosition;
            currentShield.transform.localEulerAngles = settings.protectRotation;
        }
        else
        {
            currentShield.transform.localPosition = settings.originalPosition;
            currentShield.transform.localEulerAngles = settings.originalRotation;
        }

        currentShield.SetActive(true);
    }

}
