using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerUpTypes type;
    public float attractionRange = 3f;       // Distancia a la que el jugador lo recoge
    public float moveSpeed = 5f;             // Velocidad a la que el ítem vuela al jugador
    public float rotationSpeed = 90f;

    private Transform player;
    private bool movingToPlayer = false;

    void Start()
    {
        Debug.Log("Power up creado");
        // Buscar al jugador por su tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        // Rotación continua
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attractionRange)
        {
            movingToPlayer = true;
        }

        if (movingToPlayer)
        {
            // Volar hacia el jugador
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

            // Si llegó al jugador
            if (distance <= 0.5f)
            {
                Player playerScript = player.GetComponent<Player>();
                if (playerScript != null)
                {

                    ApplyEffect();
                    Debug.Log("¡Power up recogido automáticamente!");
                }

                Destroy(gameObject);
            }
        }
    }

    void ApplyEffect()
    {
        Player playerScript = player.GetComponent<Player>();
        if (playerScript == null) return;

        switch (type)
        {
            case PowerUpTypes.Heal:
                playerScript.currentHealth = Mathf.Min(playerScript.currentHealth + 3, playerScript.maxHealth);
                Debug.Log("Power Up: Heal");
                break;

            case PowerUpTypes.SpeedBoost:
                playerScript.BoostSpeed(5f, 3f);
                Debug.Log("Power Up: SpeedBoost");
                break;

            case PowerUpTypes.DamageBoost:
                playerScript.IncreaseDamage(2f, 5f);
                Debug.Log("Power Up: Damage Boost");
                break;

            case PowerUpTypes.Invencible:
                playerScript.Invencible(2f);
                Debug.Log("Power Up: Invencible");
                break;

            case PowerUpTypes.SpeedDown:
                playerScript.SpeedDown(10f, 3f);
                Debug.Log("Power Up: Speed Down");
                break;
        }
    }
}
