using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreateWorld : MonoBehaviour
{
   [Header("World Blocks")]
   public GameObject tilePrefab; // Default tile prefab
   public GameObject grassBlock; // Grass block prefab
   public GameObject waterBlock;  // Dirt block prefab
   public GameObject stoneBlock; // Stone block prefab
   public GameObject mountainBlock; // Mountain block prefab


   [Header("World Specs")]
   public float radius = 100f; // Radius of the circular grid
   public int chunkSize = 15; // Number of tiles per chunk (both width and height)


   [Header("Noise Settings")]
   public float noiseScale = 0.1f; // Frequency of the Perlin noise
   public float heightMultiplier = 5f; // Height multiplier for terrain variation
   public int seed; // Random seed for Perlin noise


   [Header("Flat Terrain Settings")]
   public float flatnessFactor = 1f; // Factor to reduce terrain height
   public float flatRadius = 20f;    // Radius of the flat area


   [Header("Parent Object")]
   public Transform parentObject; // Parent object for the tiles
   private List<Chunk> allChunks = new List<Chunk>(); // List of all chunks


   void Start()
   {
       Generate();
   }


    public void Generate()
    {
    if (!ValidateInputs()) return;


    seed = Random.Range(0, 10000); // Generate a random seed
    ClearOldGrid(); 


    Renderer tileRenderer = tilePrefab.GetComponent<Renderer>();
    Vector3 tileSize = tileRenderer.bounds.size;
    float heightStep = tileSize.y;


    int gridSize = Mathf.CeilToInt(radius / tileSize.x); // Max tiles along one axis
    Vector3 centerPosition = Vector3.zero; // Center the grid at the origin

    for (int x = -gridSize; x <= gridSize; x++)
    {
        for (int z = -gridSize; z <= gridSize; z++)
        {
            Vector3 position = new Vector3(
                centerPosition.x + (x * tileSize.x),
                0,
                centerPosition.z + (z * tileSize.z)
            );

            if (Vector2.Distance(new Vector2(position.x, position.z), Vector2.zero) > radius) continue;

            float noiseX = (position.x + 1000f) * noiseScale + seed;
            float noiseZ = (position.z + 1000f) * noiseScale + seed;

            float baseHeight = Mathf.PerlinNoise(noiseX, noiseZ) * heightMultiplier;
            float largeScaleNoise = Mathf.PerlinNoise(noiseX * 0.5f, noiseZ * 0.5f) * (heightMultiplier * 2);
            float smallScaleNoise = Mathf.PerlinNoise(noiseX * 2f, noiseZ * 2f) * (heightMultiplier * 0.5f);
            float blendedHeight = Mathf.Lerp(baseHeight, largeScaleNoise, 0.6f);
            blendedHeight = Mathf.Lerp(blendedHeight, smallScaleNoise, 0.4f);
            float finalHeight = Mathf.PerlinNoise(noiseX * 0.1f, noiseZ * 0.1f) * blendedHeight;
            finalHeight = Mathf.Clamp(finalHeight, 1, heightMultiplier * 3);

            float distanceToCenter = Vector2.Distance(new Vector2(position.x, position.z), Vector2.zero);
            if (distanceToCenter > (radius - flatRadius) && distanceToCenter <= radius)
            {
                float edgeDistance = (distanceToCenter - (radius - flatRadius)) / flatRadius;
                float blendedHeightEdge = Mathf.Lerp(finalHeight, 1f, edgeDistance);
                finalHeight = blendedHeightEdge;
            }

            int maxHeight = Mathf.RoundToInt(finalHeight / heightStep);

            (int chunkX, int chunkZ) = CalculateChunkIndices(position);
            Chunk chunk = GetOrCreateChunk(chunkX, chunkZ);

            for (int y = 0; y <= maxHeight; y++)  // Iterate through all layers up to the maximum height
            {
                Vector3 blockPosition = new Vector3(position.x, y * tileSize.y, position.z);
                GameObject blockToPlace = GetBlockByHeight(y);

                // Check neighboring positions to ensure no holes
                bool hasNeighbor = false;
                Vector3[] neighborOffsets = {
                    new Vector3(tileSize.x, 0, 0), new Vector3(-tileSize.x, 0, 0),
                    new Vector3(0, 0, tileSize.z), new Vector3(0, 0, -tileSize.z)
                };

                foreach (Vector3 offset in neighborOffsets)
                {
                    Vector3 neighborPosition = blockPosition + offset;
                    float neighborHeight = Mathf.PerlinNoise(
                        (neighborPosition.x + 1000f) * noiseScale + seed,
                        (neighborPosition.z + 1000f) * noiseScale + seed
                    ) * heightMultiplier;

                    if (Mathf.RoundToInt(neighborHeight / heightStep) >= y)
                    {
                        hasNeighbor = true;
                        break;
                    }
                }

                if (hasNeighbor || y == maxHeight)  // Place block if it has a neighbor or is the top layer
                {
                    GameObject tile = Instantiate(blockToPlace, blockPosition, Quaternion.identity, chunk.chunkObject.transform);
                    tile.name = $"Tile ({x}, {z}, {y})";
                    chunk.AddTile(tile, chunkSize);
                }
            }
        }
    }


    }


       // Method to return the correct block prefab based on the height
   private GameObject GetBlockByHeight(float height)
   {
       // Now check the height with slight variation
       if (height <= 2f)
       {
           return waterBlock;
       }
       else if (height >= 2f && height < 5f)
       {
           return grassBlock;
       }
       else if (height == 5f)
       {
           float transitionChance = Random.Range(0f, 1f);
           return transitionChance < 0.5f ? grassBlock : stoneBlock;
       }
       else if (height >= 5f && height < 7f)
       {
           return stoneBlock;
       }
       else
       {
           return mountainBlock;
       }
   }




   private bool ValidateInputs()
   {
       if (parentObject == null)
       {
           Debug.LogError("Parent object is not assigned!");
           return false;
       }
       if (tilePrefab == null)
       {
           Debug.LogError("Tile prefab is not assigned!");
           return false;
       }
       return true;
   }


   private Chunk GetOrCreateChunk(int chunkX, int chunkZ)
   {
       Chunk chunk = allChunks.Find(c => c.chunkX == chunkX && c.chunkZ == chunkZ);
       if (chunk == null)
       {
           chunk = new Chunk(chunkX, chunkZ, this, chunkSize);
           allChunks.Add(chunk);
       }
       return chunk;
   }


   private void ClearOldGrid()
   {
       foreach (Transform child in parentObject)
       {
           DestroyImmediate(child.gameObject);
       }


       foreach (Chunk chunk in allChunks)
       {
           DestroyImmediate(chunk.chunkObject);
       }
       allChunks.Clear();
   }


   public (int, int) CalculateChunkIndices(Vector3 position)
   {
       Renderer tileRenderer = tilePrefab.GetComponent<Renderer>();
       Vector3 tileSize = tileRenderer.bounds.size;


       int chunkX = Mathf.FloorToInt(position.x / (tileSize.x * chunkSize));
       int chunkZ = Mathf.FloorToInt(position.z / (tileSize.z * chunkSize));


       return (chunkX, chunkZ);
   }


   public void ActivateNeighboringChunks(Vector3 position)
   {
       foreach (Chunk chunk in allChunks)
       {
           chunk.SetActive(false);
       }


       (int chunkX, int chunkZ) = CalculateChunkIndices(position);


       for (int offsetX = -2; offsetX <= 2; offsetX++)
       {
           for (int offsetZ = -2; offsetZ <= 2; offsetZ++)
           {
               Chunk neighborChunk = allChunks.Find(c => c.chunkX == chunkX + offsetX && c.chunkZ == chunkZ + offsetZ);
               neighborChunk?.SetActive(true);
           }
       }
   }
}



