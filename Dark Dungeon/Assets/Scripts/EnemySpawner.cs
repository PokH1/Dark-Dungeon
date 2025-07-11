using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int maxEnemies = 10;
    public int enemiesPerWave = 5;
    public float spawnInterval = 1f;
    public float tiemBetweenWaves = 10f;

    private List<GameObject> currentEnemies = new List<GameObject>();
    private int waveNumber = 0;

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

            // Generar enemigos hasta completar la cantidad en la oleada
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

        // Por defecto, usa la posición del spawnPoint
        Vector3 spawnPosition = spawnPoint.position;

        // Ajustar con un raycast hacia abajo para encontrar el suelo
        if (Physics.Raycast(spawnPoint.position + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f))
        {
            spawnPosition = hit.point; // coloca al enemigo justo sobre el suelo
        }
        else
        {
            Debug.LogWarning("No se encontró suelo debajo del punto de spawn: " + spawnPoint.name);
        }

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        currentEnemies.Add(newEnemy);
    }

    // Coroutine para esperar hasta que no haya enemigos vivos
    IEnumerator WaitForNoEnemies()
    {
        // Esperar hasta que no haya enemigos activos
        while (currentEnemies.Count > 0)
        {
            // Eliminar enemigos que han sido destruidos
            currentEnemies.RemoveAll(e => e == null);
            yield return null; // Esperar un frame y revisar nuevamente
        }
    }
}
