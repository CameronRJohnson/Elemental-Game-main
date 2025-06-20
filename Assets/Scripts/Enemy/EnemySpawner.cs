using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public Transform spawnPoint;   // Center point where the enemies will spawn
    public float spawnRadius = 10f; // Radius around the spawn point within which enemies will be spawned
    public float minimumSeparation = 5f; // Minimum distance between spawned enemies
    public int maxSpawnAttempts = 10; // Maximum attempts to find a safe spawn position
    public Vector3 exclusionZoneCenter = Vector3.zero; // Exclusion zone center
    public float exclusionZoneRadius = 3f; // Exclusion zone radius

    [System.Serializable]
    public class EnemySpawnSettings
    {
        public GameObject enemyPrefab;
        public int numberToSpawn;
        public float spawnDelay; // Delay between spawns
    }

    public List<EnemySpawnSettings> spawnSettings = new List<EnemySpawnSettings>(); // List of spawn settings

    private List<Vector3> spawnedPositions = new List<Vector3>(); // Store positions of already spawned enemies
    private List<(GameObject enemyPrefab, float spawnDelay)> spawnQueue = new List<(GameObject, float)>(); // Randomized spawn queue

    public void StartSpawning()
    {
        PrepareSpawnQueue();
        StartCoroutine(SpawnEnemiesFromQueue());
    }

    private void PrepareSpawnQueue()
    {
        spawnedPositions.Clear(); // Clear any previously stored positions
        spawnQueue.Clear(); // Clear the spawn queue

        // Add enemies to the queue based on spawn settings
        foreach (EnemySpawnSettings settings in spawnSettings)
        {
            for (int i = 0; i < settings.numberToSpawn; i++)
            {
                spawnQueue.Add((settings.enemyPrefab, settings.spawnDelay));
            }
        }

        // Shuffle the queue to randomize the spawn order
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
                // Generate a random position within the spawn radius
                spawnPosition = spawnPoint.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius));

                // Check if this position is safe and outside the exclusion zone
                if (IsPositionSafe(spawnPosition) && IsOutsideExclusionZone(spawnPosition))
                {
                    positionFound = true;
                    spawnedPositions.Add(spawnPosition); // Record this position
                    InstantiateEnemy(enemyPrefab, spawnPosition);
                }
                else
                {
                    attempts++;
                }
            }

            // If no valid position was found after max attempts, log a warning
            if (!positionFound)
            {
                Debug.LogWarning($"Could not find a safe position for enemy of type {enemyPrefab.name}. Skipping spawn.");
            }

            yield return new WaitForSeconds(spawnDelay); // Wait for the specified delay before spawning the next enemy
        }
    }

    private bool IsPositionSafe(Vector3 position)
    {
        foreach (Vector3 spawnedPosition in spawnedPositions)
        {
            if (Vector3.Distance(position, spawnedPosition) < minimumSeparation)
            {
                return false; // Position is too close to another enemy
            }
        }
        return true; // Position is safe
    }

    private bool IsOutsideExclusionZone(Vector3 position)
    {
        // Check if the position is outside the exclusion zone
        return Vector3.Distance(position, exclusionZoneCenter) > exclusionZoneRadius;
    }

private void InstantiateEnemy(GameObject enemyPrefab, Vector3 position)
{

    // Instantiate the enemy at the given position
    GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);

    // Initialize the enemy (start spawning animation)
    Enemy enemyScript = enemy.GetComponent<Enemy>();
    if (enemyScript != null)
    {
        enemyScript.target = GameObject.FindWithTag("Player"); // Assign the player as the target
    }
}

}
