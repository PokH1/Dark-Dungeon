using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject boss;
    public Transform[] spawnPoints;
    public int maxEnemies = 10;
    public int enemiesPerWave = 5;
    public float spawnInterval = 1f;
    public float tiemBetweenWaves = 10f;

    private List<GameObject> currentEnemies = new List<GameObject>();
    private int waveNumber = 0;
    private bool bossSpawned = false; // ✅ Nueva bandera

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (true)
        {
            waveNumber++;
            Debug.Log("Oleada: " + waveNumber);
            int enemiesSpawnedThisWave = 0;

            // ✅ Invocar jefe SOLO en la segunda oleada
            if (waveNumber == 2 && !bossSpawned)
            {
                SpawnBoss();
                bossSpawned = true;
            }

            while (enemiesSpawnedThisWave < enemiesPerWave)
            {
                if (currentEnemies.Count < maxEnemies)
                {
                    SpawnEnemy();
                    enemiesSpawnedThisWave++;
                }

                yield return new WaitForSeconds(spawnInterval);
            }

            // Esperar hasta que no queden enemigos
            yield return StartCoroutine(WaitForNoEnemies());

            // Esperar el tiempo entre oleadas antes de iniciar la siguiente
            yield return new WaitForSeconds(tiemBetweenWaves);
        }
    }

    void SpawnEnemy()
    {
        currentEnemies.RemoveAll(e => e == null);

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        Vector3 spawnPosition = spawnPoint.position;

        if (Physics.Raycast(spawnPoint.position + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f))
        {
            spawnPosition = hit.point;
        }
        else
        {
            Debug.LogWarning("No se encontró suelo debajo del punto de spawn: " + spawnPoint.name);
        }

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        currentEnemies.Add(newEnemy);

        // Aumento de vida por oleada

        EnemyHealth healthScript = newEnemy.GetComponent<EnemyHealth>();
        if (healthScript != null)
        {
            int newHealth = 100 + (waveNumber - 1) * 25;
            healthScript.maxHealt = newHealth;
            healthScript.currentHealt = newHealth;
            Debug.Log("La nueva vida es: " + newHealth);
        }

        // Aumento de daño por oleada
        EnemyAttack attackScript = newEnemy.GetComponent<EnemyAttack>();
        if (attackScript != null)
        {
            int newDamage = 10 + (waveNumber - 1) * 5;
            attackScript.damage = newDamage;
            Debug.Log("La nueva vida es: " + newDamage);
        }
    }

    void SpawnBoss()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        Vector3 spawnPosition = spawnPoint.position;

        if (Physics.Raycast(spawnPoint.position + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f))
        {
            spawnPosition = hit.point;
        }

        GameObject bossSpawn = Instantiate(boss, spawnPosition, Quaternion.identity);
        currentEnemies.Add(bossSpawn);

        bossSpawn.SetActive(true);

        Debug.Log("⚠️ Jefe (Dragón) Invocado en la Oleada 2!");
    }

    IEnumerator WaitForNoEnemies()
    {
        while (currentEnemies.Count > 0)
        {
            currentEnemies.RemoveAll(e => e == null);
            yield return null;
        }
    }
}
