using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damage = 20;
    private bool canDamage = false;

    public void EnableDamage()
    {
        canDamage = true;
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
                enemy.TakeDamage(damage);
                Debug.Log("Enemigo dañado con " + damage);
            }
            else
            {
                Debug.Log("EnemyHealth no encontrado");
            }
        }

    }
}
