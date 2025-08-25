using System;
using UnityEngine;

public class Weapon : MonoBehaviour, IWeaponUI
{
    public string weaponName;
    public int damage = 20;
    private bool canDamage = false;
    private float damageMultiplier = 1f;
    public Sprite icon; //Iconos del arma
    public AudioClip audioClip;
    public AudioSource audioSource;
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

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Entró en colisión con: " + other.name);

        if (!canDamage) return;

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Colisionó con enemigo");
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                int finalDamage = Mathf.RoundToInt(damage * damageMultiplier);
                enemy.TakeDamage(finalDamage);
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
}
