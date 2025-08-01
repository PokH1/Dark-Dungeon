using UnityEngine;
using UnityEngine.VFX;

public class Fireball : MonoBehaviour
{
    private VisualEffect vfx;
    public int damage = 20;
    public float lifetime = 5f;
    public GameObject explosionEffect;
    public AudioClip explosionSound;
    private AudioSource audioSource;
    private bool hasExploded = false;  // Bandera para evitar que la explosión ocurra más de una vez

    void Start()
    {
        // Destruir la bola de fuego después de un tiempo
        Destroy(gameObject, lifetime);
        vfx = GetComponent<VisualEffect>();
        vfx.SetVector3("M_Fireball_01", new Vector3(0, 0, 10));

        // // Inicializar AudioSource para sonido de explosión
        // audioSource = gameObject.AddComponent<AudioSource>();
        // audioSource.clip = explosionSound;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Verificar si la bola de fuego colide con algo que no sea el propio dragón
        if (hasExploded) return;  // Si ya explotó, no hacer nada más

        if (collision.gameObject.CompareTag("Player"))
        {
            Player playerHealth = collision.gameObject.GetComponent<Player>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        // Destruir la bola de fuego después de la colisión
        Explode();
    }

    void Explode()
    {
        hasExploded = true; // Marcar que la explosión ya ocurrió

        // Instanciar el efecto de explosión
        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Reproducir el sonido de explosión
        // audioSource.Play();

        // Destruir el objeto de la explosión después de que termine la animación (ajusta el tiempo según sea necesario)
        Destroy(explosion, 5f); // Aquí el 2f es el tiempo en segundos después del cual destruirá la explosión

        // Destruir la bola de fuego
        Destroy(gameObject, 5f);
    }
}
