using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject boss;
    public Transform[] spawnPoints;
    public int maxEnemies = 15;
    public int enemiesPerWave = 4;
    private GameObject bossInstance;
    public float spawnInterval = 1f;
    public float tiemBetweenWaves = 10f;
    public TextMeshProUGUI waveText;
    private List<GameObject> currentEnemies = new List<GameObject>();
    private int waveNumber = 0;
    private bool bossSpawned = false; // ✅ Nueva bandera
    public AudioSource backgroundMusic;
    public AudioSource bossMusic;

    void Start()
    {
        StartCoroutine(SpawnWaves());

        if (bossMusic != null)
        {
            bossMusic.Stop();
            bossMusic.loop = true;
        }
    }

    IEnumerator SpawnWaves()
    {
        while (true)
        {
            waveNumber++;
            
            // Aumentamos los enemigos por oleada
            enemiesPerWave = Mathf.Min(enemiesPerWave + 1, maxEnemies);

            waveText.text = $"Wave {waveNumber}";

            Debug.Log($"Oleada: {waveNumber} - Enemigos en esta oleada: {enemiesPerWave}");

            int enemiesSpawnedThisWave = 0;

            // Invocar jefe SOLO en la segunda oleada
            if (waveNumber == 2 && !bossSpawned)
            {
                SpawnBoss();
                bossSpawned = true;
                StartCoroutine(SpawnEnemiesWithBoss());
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

        if (waveNumber > 1) {
            
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

        if (backgroundMusic != null)
            backgroundMusic.Stop();

        if (bossMusic != null)
            bossMusic.Play();
    }

    IEnumerator SpawnEnemiesWithBoss()
    {
        while (bossInstance != null) // mientras el jefe esté vivo
        {
            // Calcular enemigos acompañantes según oleada
            int extraEnemies = Mathf.Min(3 + (waveNumber - 2), maxEnemies - 1);

            int activeSmallEnemies = 0;
            foreach (var enemy in currentEnemies)
            {
                if (enemy != null && enemy != bossInstance)
                    activeSmallEnemies++;
            }

            // Generar más si faltan
            if (activeSmallEnemies < extraEnemies && currentEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(3f); // cada 3 segundos revisa y genera si faltan
        }

        Debug.Log("✅ Jefe derrotado, se detiene el spawn de enemigos pequeños.");
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
