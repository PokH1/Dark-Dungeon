using System;
using UnityEngine;

public class Weapon : MonoBehaviour, IWeaponUI
{
    public string weaponName;
    public int damage = 20;
    public bool canDamage = false;
    public float damageMultiplier = 1f;
    public Sprite icon; //Iconos del arma
    public AudioClip audioClip;
    public AudioSource audioSource;

    [Header("Hammer AoE")]
    public float hammerRadius = 3f;       // Radio del AoE
    public LayerMask enemyLayer;          // Capa de enemigos para el AoE
    private bool hammerHasHit = false;
    public bool appliesDot = false;  // solo para SwordRed
    public int dotDamage = 5;
    public float dotDuration = 5f;
    [Range(0f, 1f)] public float lifestealPercent = 0.2f;  // solo para SwordCrimson

    public Sprite GetIcon()
    {
        return icon;
    }

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void EnableDamage()
    {
        canDamage = true;
        hammerHasHit = false; // resetear golpe de hammer al inicio del ataque
        if (audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
        Debug.Log("Daño activado");
    }

    public void DisableDamage()
    {
        canDamage = false;
        Debug.Log("Daño desactivado");
    }

    public void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Entró en colisión con: " + other.name);

        if (!canDamage) return;

        // Solo enemigos
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy == null) return;

        int finalDamage = Mathf.RoundToInt(damage * damageMultiplier);

        // === SwordCrimson: Robo de vida ===
        if (CompareTag("SwordCrimson"))
        {
            enemy.TakeDamage(finalDamage);
            int healAmount = Mathf.RoundToInt(finalDamage * lifestealPercent);
            Player player = FindObjectOfType<Player>();
            if (player != null) player.Heal(healAmount);
            Debug.Log($"SwordCrimson: {finalDamage} daño, robó {healAmount} de vida");
        }

        // === SwordRed: Daño en el tiempo (DoT) ===
        else if (CompareTag("SwordRed"))
        {
            enemy.TakeDamage(finalDamage);
            if (appliesDot)
            {
                enemy.ApplyDamageOverTime(dotDamage, dotDuration);
                Debug.Log($"SwordRed aplica DoT: {dotDamage}/seg por {dotDuration} seg");
            }
        }

        // Si es un Hammer, aplica daño en área
        if (CompareTag("Hammer") && !hammerHasHit)
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, hammerRadius, enemyLayer);
            foreach (var enemyCollider in enemies)
            {
                EnemyHealth enemys = enemyCollider.GetComponent<EnemyHealth>();
                if (enemys != null)
                {
                    enemys.TakeDamage(finalDamage);
                }
            }
            hammerHasHit = true;
            Debug.Log($"Hammer hizo {finalDamage} de daño a {enemies.Length} enemigos");
            return; // No hacer daño individual otra vez
        }


        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Colisionó con enemigo");
            EnemyHealth enemyS = other.GetComponent<EnemyHealth>();
            if (enemyS != null)
            {
                // int finalDamage = Mathf.RoundToInt(damage * damageMultiplier);
                enemyS.TakeDamage(finalDamage);
                Debug.Log("Enemigo dañado con " + finalDamage);
            }
            else
            {
                Debug.Log("EnemyHealth no encontrado");
            }
        }

    }

    public void SetDamageMultiplier(float multiplier)
    {
        damageMultiplier = multiplier;
    }

    public void ResetDamageMultiplier()
    {
        damageMultiplier = 1f;
    }
    
        private void OnDrawGizmosSelected()
    {
        if (CompareTag("Hammer"))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, hammerRadius);
        }
    }
}
