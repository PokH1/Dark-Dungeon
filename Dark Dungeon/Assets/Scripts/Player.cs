using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using AbstractionServer;
using System.Linq;
using NFTType = AbstractionServer.NFT;



[Serializable]
public class RunFinishedRequest
{
    public uint monsters_defeated;
    public string[] items_found;
    public string[] new_items_selected;
    public ulong survival_time;
}

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
    public Slider healthSlider;
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
    public int maxShieldHealth = 120;
    private int currentShieldHealth;
    public Slider shieldSlider;
    public GameObject equippedWeapon;
    private float originalSpeed;
    private bool isInvincible = false;
    private float originalRunSpeed;
    private float originalWalkSpeed;
    private float damageMultiplier = 1f;
    public List<GameObject> weaponInventory = new List<GameObject>();
    private int currentWeaponIndex = 0;
    public List<WeaponSlotUI> weaponSlotUI = new List<WeaponSlotUI>();
    public AudioSource audioSource;
    public AudioClip damageSound; // sonido para recibir daño
    public AudioClip deathSound; // sonido de muerte
    public AudioClip shieldBrokenSound; // Sonido del escudo cuando se rompe
    public AudioClip shieldHitSound; // sonido cuando el escudo recibe el daño
    public AudioClip weaponChangeSound; // Sonido de cambio de arma
    public int currentArrows;
    public TMP_Text arrowCountText;
    public GameObject arrowUI;
    public GameObject bowPrefab;
    public GameObject bowCrosshair;  // Asigna aquí el UI del crosshair del arco

    // === DATOS DE LA RUN ===
    private int monstersDefeated = 0;
    private List<ulong> itemsFound = new List<ulong>();
    private List<ulong> newItemsSelected = new List<ulong>();
    private float survivalTime;
    // Lista temporal solo para los NFTs de esta run
    private List<ulong> itemsToSend = new List<ulong>();
    // Lista de prefabs de armas NFT seleccionadas antes de iniciar la run
    public List<NFTWeaponPrefab> nftWeaponPrefabs; // Aquí arrastras tus prefabs y les pones nombre


    [System.Serializable]
    public class NFTWeaponPrefab
    {
        public string weaponName;   // Nombre que coincide con el NFT
        public GameObject prefab;   // Prefab real del arma
    }



    void Start()
    {
        InitializePlayer();

        // 1️⃣ Cargar armas de NFTs seleccionados si hay
        if (GameData.SelectedNFTs != null && GameData.SelectedNFTs.Count > 0)
        {
            InitializeWeaponsFromSelectedNFTs();
        }

        // 2️⃣ Si no hay armas en el inventario, equipar el arma por defecto
        if (weaponInventory.Count == 0)
        {
            InitializeWeapons(); // Este método equipará weaponPrefab o bowPrefab
        }

        // 3️⃣ Actualizar UI de armas
        UpdateWeaponUI(currentWeaponIndex);

        // 4️⃣ Mostrar crosshair si el arma equipada es un arco
        if (bowCrosshair != null)
        {
            bowCrosshair.SetActive(IsUsingBow());
        }

        // 5️⃣ Iniciar datos de la run
        StartRun();
    }


    private void InitializePlayer()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        currentShieldHealth = maxShieldHealth;
        shieldSlider.maxValue = maxShieldHealth;
        shieldSlider.value = currentShieldHealth;
        shieldSlider.gameObject.SetActive(true);

        animator = GetComponent<Animator>();
        originalRunSpeed = runSpeed;
        originalWalkSpeed = walkSpeed;

        if (shieldPrefab != null)
            EquipShield(shieldPrefab);

        audioSource.volume = 20f;
    }

private void InitializeWeaponsFromSelectedNFTs()
{
    weaponInventory.Clear();

    foreach (var nft in GameData.SelectedNFTs)
    {
        // Buscar el prefab que coincida con el nombre del NFT
        var prefabData = nftWeaponPrefabs.FirstOrDefault(p => p.weaponName == nft.name);
        if (prefabData != null)
        {
            GameObject weapon = Instantiate(prefabData.prefab, weaponPoint);
            weapon.SetActive(false);
            weaponInventory.Add(weapon);
        }
    }

    // Equipar la arma que coincide con el InitialWeaponName
    var initialWeapon = weaponInventory.FirstOrDefault(w => w.name.Contains(GameData.InitialWeaponName));
    if (initialWeapon != null)
    {
        currentWeaponIndex = weaponInventory.IndexOf(initialWeapon);
        EquipWeaponByIndex(currentWeaponIndex);
    }
}

    private void InitializeWeapons()
    {
        if (weaponPrefab != null)
        {
            // The initial weapon is assumed to be a weapon other than a bow.
            EquipInitialWeapon(weaponPrefab);
        }
        else if (bowPrefab != null)
        {
            // If the initial weapon is a bow, handle it specifically.
            EquipInitialWeapon(bowPrefab);
        }
    }

    private void EquipInitialWeapon(GameObject initialWeaponPrefab)
    {
        equippedWeapon = Instantiate(initialWeaponPrefab);

        weaponInventory.Add(equippedWeapon);
        currentWeaponIndex = 0;

        if (bowCrosshair != null)
        {
            bowCrosshair.SetActive(IsUsingBow());
        }

        // Call the regular equip method to handle all logic.
        EquipWeaponByIndex(currentWeaponIndex);
        UpdateWeaponUI(currentWeaponIndex);
    }

    private void StartRun()
    {
        survivalTime = Time.time;
        monstersDefeated = 0;
        itemsFound.Clear();
        newItemsSelected.Clear();
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            LaunchArrow bowScript = equippedWeapon?.GetComponent<LaunchArrow>();
            if (bowScript != null)
            {
                bowScript.LoadArrow();
            }
            else
            {
                Debug.Log("El arma actual no es un arco");
            }
        }

        if (!isDie)
        {
            // Manejo centralizado del clic izquierdo
            if (Input.GetMouseButtonDown(0))
            {
                if (IsUsingBow())
                {
                    LaunchArrow bowScript = equippedWeapon.GetComponent<LaunchArrow>();
                    if (bowScript != null)
                    {
                        bowScript.StartCharging(); // Inicia la carga del arco
                        animator.SetBool("isCharging", true);
                    }
                }
                else
                {
                    animator.SetTrigger("attack");
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (IsUsingBow())
                {
                    LaunchArrow bowScript = equippedWeapon.GetComponent<LaunchArrow>();
                    if (bowScript != null)
                    {
                        animator.SetBool("isCharging", false);
                        animator.SetTrigger("shootArrow");
                        bowScript.ShootArrow(); // Dispara la flecha

                    }
                }
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

        HandleShield();

        if (currentShield != null)
        {
            bool shouldBeVisible = !IsUsingBow();
            currentShield.SetActive(shouldBeVisible);
        }
    }


    bool IsUsingBow()
    {
        return equippedWeapon != null && equippedWeapon.CompareTag("Bow");
    }

        // === REGISTRO DE DATOS DE LA RUN ===
    public void EnemyDefeated() => monstersDefeated++;

    public void ItemFound(ulong itemId)
    {
        if (!itemsToSend.Contains(itemId))
            itemsToSend.Add(itemId);

        Debug.Log("NFT recogido y listo para enviar: " + itemId);
    }

    public void NewItemSelected(ulong itemId) => newItemsSelected.Add(itemId);

    public void UpdateArrowCountUI()
    {
        // Esta función se llama desde LaunchArrow
        LaunchArrow bowScript = equippedWeapon?.GetComponent<LaunchArrow>();
        if (bowScript != null && arrowCountText != null)
        {
            arrowCountText.text = "X" + bowScript.cantArrows.ToString();
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
            currentShieldHealth = Mathf.Clamp(currentShieldHealth, 0, maxShieldHealth);
            shieldSlider.value = currentShieldHealth;
            Debug.Log("El escudo recibio: " + amount + " de daño");

            if (shieldHitSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(shieldHitSound);
            }

            if (currentShieldHealth <= 0)
            {
                DestroyShield();
            }
        }
        else
        {
            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            healthSlider.value = currentHealth;

            Debug.Log("Vida del jugador: " + currentHealth);

            if (currentHealth > 0)
            {
                if (damageSound != null && audioSource != null)
                {
                    Debug.Log("Reproduciendo sonido de daño");
                    audioSource.PlayOneShot(damageSound);
                    Debug.Log("Reproduciendo sonido de daño");
                }
            }
            else
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

        if (shieldBrokenSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shieldBrokenSound);
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

        AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);
        float deathAnimDuration = animInfo.length;

        if (deathSound != null && audioSource != null)
        {
            Debug.Log("Reproduciendo sonido de muerte");
            audioSource.PlayOneShot(deathSound);
            Debug.Log("Reproduciendo sonido de muerte");
        }

        // Aquí puedes desactivar movimiento o mostrar menú de muerte
        GetComponent<Player>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        this.enabled = false; // Desactiva el propio script

        StartCoroutine(WaitForDeathAnimation(deathAnimDuration));

        int maxItems = 6;
        var itemsFoundInt = itemsToSend
            .Distinct()
            .Select(i => (int)Mathf.Min(i, int.MaxValue))
            .Take(maxItems)
            .ToArray();

        // Mostrar en consola la cantidad y los IDs de items
        Debug.Log("Cantidad de items encontrados: " + itemsFoundInt.Length);
        Debug.Log("Items enviados: " + string.Join(", ", itemsFoundInt));
        Debug.Log("Items a enviar: " + string.Join(", ", itemsFoundInt));

        FindObjectOfType<AbstractionServer.FinishRunService>()
            .OnPlayerDeath(itemsFoundInt, survivalTime, monstersDefeated);

        itemsToSend.Clear();
    }

    private IEnumerator WaitForDeathAnimation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        FindAnyObjectByType<GameOver>().GameOverCanvas();
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
        if (weaponInventory.Count < 5)
        {
            // Instancia la nueva arma
            GameObject newWeapon = Instantiate(newWeaponPrefab, weaponPoint);
            newWeapon.SetActive(false); // Solo se activa cuando se equipa

            // Agregar nueva arma al inventario
            weaponInventory.Add(newWeapon);

            Debug.Log("Arma añadida al inventario");

            // Equipar automáticamente si es la única arma o se quiere auto-equipar
            currentWeaponIndex = weaponInventory.Count - 1;
            EquipWeaponByIndex(currentWeaponIndex);
        }
        else
        {
            Debug.LogWarning("Inventario lleno. No puedes llevar más armas.");
            // Aquí puedes abrir tu panel de reemplazo como se mencionó antes
        }

        // Asegurar que la lista de UI esté sincronizada
        UpdateWeaponUI(currentWeaponIndex);
    }


    void EquipWeaponByIndex(int index)
    {
        if (index < 0 || index >= weaponInventory.Count) return;

        audioSource?.PlayOneShot(weaponChangeSound);

        // Deactivate previous weapon
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

        // Handle new weapon logic
        bool isBow = equippedWeapon.CompareTag("Bow");

        if (isBow)
        {
            Vector3 originalScale = equippedWeapon.transform.localScale;
            equippedWeapon.transform.SetParent(leftHandSlot);
            equippedWeapon.transform.localPosition = Vector3.zero; // Ajusta según tu modelo
            equippedWeapon.transform.localRotation = Quaternion.Euler(0f, 0f, 180);
            equippedWeapon.transform.localScale = originalScale;
        }
        else
        {
            Vector3 originalScale = equippedWeapon.transform.localScale;
            equippedWeapon.transform.SetParent(weaponPoint, false);
            // equippedWeapon.transform.localPosition = Vector3.zero; // Ajusta según tu modelo
            // equippedWeapon.transform.localRotation = Quaternion.identity; // Ajusta según tu modelo
            equippedWeapon.transform.localScale = originalScale;

        }

        LaunchArrow bowScript = equippedWeapon.GetComponent<LaunchArrow>();
        Weapon newWeaponScript = equippedWeapon.GetComponent<Weapon>();

        if (isBow)
        {
            if (bowScript != null)
            {
                bowScript.enabled = true;
                bowScript.player = this;
                // currentArrows = bowScript.cantArrows;
                // UpdateArrowCountUI();
                arrowUI?.SetActive(true);
            }
            if (newWeaponScript != null) newWeaponScript.enabled = false;
            weaponScript = null; // Important: Clear the reference to the melee weapon script
        }
        else
        {
            if (newWeaponScript != null)
            {
                newWeaponScript.enabled = true;
                weaponScript = newWeaponScript;
                weaponScript.SetDamageMultiplier(damageMultiplier);
            }
            if (bowScript != null) bowScript.enabled = false;
            arrowUI?.SetActive(false);
        }

        currentWeaponIndex = index;
        UpdateWeaponUI(index);

        // Handle shield visibility after equipping a weapon
        if (currentShield != null)
        {
            currentShield.SetActive(!isBow);
        }

        if (bowCrosshair != null)
        {
            bowCrosshair.SetActive(IsUsingBow());
        }
    }
    void UpdateWeaponUI(int equippedIndex)
    {
        for (int i = 0; i < weaponSlotUI.Count; i++)
        {
            Sprite weaponIcon = null;

            // Comprueba si hay un arma en el inventario para este slot.
            if (i < weaponInventory.Count && weaponInventory[i] != null)
            {
                // Usa una interfaz para obtener el ícono, lo cual es más flexible.
                IWeaponUI weaponUI = weaponInventory[i].GetComponent<IWeaponUI>();
                if (weaponUI != null)
                {
                    weaponIcon = weaponUI.GetIcon();
                }
            }

            if (weaponIcon == null && weaponSlotUI[i].emptyIcon != null)
            {
                weaponIcon = weaponSlotUI[i].emptyIcon;
            }

            // Llama a la función del slot de UI con el ícono.
            // Si no se encontró un arma, weaponIcon será null, y el script del slot 
            // se encargará de mostrar el emptyIcon.
            weaponSlotUI[i].SetWeaponIcon(weaponIcon);

            // Resalta el slot si es el arma equipada.
            weaponSlotUI[i].SetHighlight(i == equippedIndex);
        }
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
        shieldSlider.maxValue = maxShieldHealth;
        shieldSlider.value = currentShieldHealth;
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
