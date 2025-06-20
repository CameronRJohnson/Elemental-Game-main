using UnityEngine;
using System.Collections.Generic;

public class TreeSpawner : MonoBehaviour
{
    [Header("Tree Settings")]
    public GameObject treePrefab; // The tree prefab to spawn
    public int numberOfTrees; // Total number of trees to spawn
    public float minimumDistance; // Minimum distance between trees

    [Header("Spawn Area")]
    public float spawnRadius; // Radius of the circular spawn area
    public Vector3 areaCenter = Vector3.zero; // Center of the spawning area

    [Header("No-Spawn Zone")]
    public float exclusionRadius; // Radius of the no-spawn zone around the center

    [Header("World Reference")]
    public GameObject worldObject; // Reference to the world GameObject

    private List<Vector3> treePositions = new List<Vector3>(); // List of placed tree positions
    private List<GameObject> spawnedTreeInstances = new List<GameObject>(); // List of tree instances
    public int maxAttempts; // Safety limit for spawn attempts

    public void Start() {
        ClearTrees();
        SpawnTrees();
    }

    public void SpawnTrees()
    {
        if (treePrefab == null)
        {
            Debug.LogError("Tree prefab is not assigned!");
            return;
        }

        // Clear existing trees before spawning new ones
        ClearTrees();

        treePositions.Clear(); // Reset stored tree positions
        int spawnedTrees = 0;
        int attempts = 0;

        while (spawnedTrees < numberOfTrees && attempts < maxAttempts)
        {
            attempts++;

            // Generate a random position within the circular area
            Vector3 randomPosition = RandomPointInCircle(areaCenter, spawnRadius);

            // Check if position is within the exclusion zone
            if (Vector3.Distance(new Vector3(randomPosition.x, areaCenter.y, randomPosition.z), areaCenter) <= exclusionRadius)
                continue;

            // Raycast down to check collision with the worldObject
            if (Physics.Raycast(randomPosition + Vector3.up * 100, Vector3.down, out RaycastHit hit))
            {
                if (hit.collider.gameObject == worldObject)
                {
                    Vector3 spawnPosition = hit.point;

                    // Ensure trees are sufficiently spaced apart
                    if (IsPositionValid(spawnPosition))
                    {
                        GameObject treeInstance = Instantiate(treePrefab, spawnPosition, Quaternion.identity);
                        spawnedTreeInstances.Add(treeInstance); // Keep track of spawned instance
                        treePositions.Add(spawnPosition);
                        spawnedTrees++;
                    }
                }
            }
        }

        Debug.Log($"Spawned {spawnedTrees}/{numberOfTrees} trees with {attempts} attempts.");
    }

    bool IsPositionValid(Vector3 position)
    {
        // Ensure the position is not within the exclusion zone
        if (Vector3.Distance(new Vector3(position.x, areaCenter.y, position.z), areaCenter) <= exclusionRadius)
            return false;

        // Ensure the position is not too close to other trees
        foreach (Vector3 existingPosition in treePositions)
        {
            if (Vector3.Distance(existingPosition, position) < minimumDistance)
            {
                return false; // Too close to an existing tree
            }
        }
        return true;
    }

    Vector3 RandomPointInCircle(Vector3 center, float radius)
    {
        // Generate a random point within a circle
        float angle = Random.Range(0f, 2 * Mathf.PI);  // Random angle
        float distance = Random.Range(0f, radius);  // Random distance from center

        // Convert polar to Cartesian coordinates
        float x = center.x + Mathf.Cos(angle) * distance;
        float z = center.z + Mathf.Sin(angle) * distance;

        return new Vector3(x, center.y, z);
    }

    void ClearTrees()
    {
        // Destroy all previously spawned tree instances
        foreach (GameObject tree in spawnedTreeInstances)
        {
            if (tree != null)
            {
                Destroy(tree);
            }
        }

        // Clear the list of tree instances
        spawnedTreeInstances.Clear();
    }
}
