using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGeneration : MonoBehaviour
{
   [Header("World Blocks")]
   public GameObject blockPrefab; // Default tile prefab

    [Header("Generation Settings")]
    public int radius = 10; // Radius of the circle

    // Start is called before the first frame update
    void Start()
    {
        GenerateCircle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateCircle()
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int z = -radius; z <= radius; z++)
                {
                    if (x * x + y * y + z * z <= radius * radius) // Check if within sphere
                    {
                        // Check if the cube is fully surrounded on all six sides
                        bool isSurrounded =
                            (x - 1) * (x - 1) + y * y + z * z <= radius * radius &&
                            (x + 1) * (x + 1) + y * y + z * z <= radius * radius &&
                            x * x + (y - 1) * (y - 1) + z * z <= radius * radius &&
                            x * x + (y + 1) * (y + 1) + z * z <= radius * radius &&
                            x * x + y * y + (z - 1) * (z - 1) <= radius * radius &&
                            x * x + y * y + (z + 1) * (z + 1) <= radius * radius;

                        if (!isSurrounded) // Only generate if not fully surrounded
                        {
                            Vector3 position = new Vector3(x, y, z);
                            Instantiate(blockPrefab, position, Quaternion.identity);
                        }
                    }
                }
            }
        }
    }
}
