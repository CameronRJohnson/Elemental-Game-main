using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public float spawnRadius = 10f;
    public float minimumSeparation = 5f;
    public int maxSpawnAttempts = 10;
    public Vector3 exclusionZoneCenter = Vector3.zero;
    public float exclusionZoneRadius = 3f;

    [System.Serializable]
    public class EnemySpawnSettings
    {
        public GameObject enemyPrefab;
        public int numberToSpawn;
        public float spawnDelay;
    }

    public List<EnemySpawnSettings> spawnSettings = new List<EnemySpawnSettings>();

    private List<Vector3> spawnedPositions = new List<Vector3>();
    private List<(GameObject enemyPrefab, float spawnDelay)> spawnQueue = new List<(GameObject, float)>();

    public void StartSpawning()
    {
        PrepareSpawnQueue();
        StartCoroutine(SpawnEnemiesFromQueue());
    }

    private void PrepareSpawnQueue()
    {
        spawnedPositions.Clear();
        spawnQueue.Clear();

        foreach (EnemySpawnSettings settings in spawnSettings)
        {
            for (int i = 0; i < settings.numberToSpawn; i++)
            {
                spawnQueue.Add((settings.enemyPrefab, settings.spawnDelay));
            }
        }

        for (int i = spawnQueue.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (spawnQueue[i], spawnQueue[randomIndex]) = (spawnQueue[randomIndex], spawnQueue[i]);
        }
    }

    private IEnumerator SpawnEnemiesFromQueue()
    {
        foreach ((GameObject enemyPrefab, float spawnDelay) in spawnQueue)
        {
            Vector3 spawnPosition;
            bool positionFound = false;
            int attempts = 0;

            while (!positionFound && attempts < maxSpawnAttempts)
            {
                spawnPosition = spawnPoint.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius));

                if (IsPositionSafe(spawnPosition) && IsOutsideExclusionZone(spawnPosition))
                {
                    positionFound = true;
                    spawnedPositions.Add(spawnPosition);
                    InstantiateEnemy(enemyPrefab, spawnPosition);
                }
                else
                {
                    attempts++;
                }
            }

            if (!positionFound)
            {
                Debug.LogWarning($"Could not find a safe position for enemy of type {enemyPrefab.name}. Skipping spawn.");
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private bool IsPositionSafe(Vector3 position)
    {
        foreach (Vector3 spawnedPosition in spawnedPositions)
        {
            if (Vector3.Distance(position, spawnedPosition) < minimumSeparation)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsOutsideExclusionZone(Vector3 position)
    {
        return Vector3.Distance(position, exclusionZoneCenter) > exclusionZoneRadius;
    }

    private void InstantiateEnemy(GameObject enemyPrefab, Vector3 position)
    {
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.target = GameObject.FindWithTag("Player");
        }
    }

}
