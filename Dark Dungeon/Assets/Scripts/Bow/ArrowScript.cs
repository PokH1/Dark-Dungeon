using System.Collections;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    public int damage;
    public bool arrowInFly;

    Quaternion rotate;
    public float powerFinal;
    public LaunchArrow launchArrowScript;

    void Start()
    {
        // plusArrow = GameObject.FindGameObjectWithTag("Bow");
        arrowInFly = false;
        rotate = Quaternion.Euler(90, 0, 0);
    }

private void Update()
{
    if (arrowInFly)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null && rb.linearVelocity.magnitude > 0.1f)
        {
            transform.right = -rb.linearVelocity.normalized;
        }
    }
}
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Flecha colisionó con: {collision.gameObject.name} - Tag: {collision.gameObject.tag}");
        if (!arrowInFly) return; // Evita múltiples colisiones

        arrowInFly = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col != null) col.isTrigger = true;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Flecha daño al enemigo con: " + damage);
            }
        }
        else if (collision.gameObject.CompareTag("Boss"))
        {
            BossHealth boss = collision.gameObject.GetComponent<BossHealth>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                Debug.Log("Flecha daño al jefe con: " + damage);
            }
        }

        Debug.Log("Generaste: " + damage + " al enemigo");
        StartCoroutine(DestructionTime());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!arrowInFly && other.CompareTag("Player"))
        {
            ArrowGet();
        }
    }

    public void ArrowGet()
    {
        if (launchArrowScript != null)
        {
            launchArrowScript.AddArrows(1);
            Debug.Log("Flecha recogida por el jugador");
            Destroy(gameObject);
        }

    }

    IEnumerator DestructionTime()
    {
        yield return new WaitForSeconds(60);
        Destroy(gameObject);
    }
}
