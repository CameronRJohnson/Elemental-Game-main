using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Chunk
{
    public int chunkX;
    public int chunkZ;
    public List<GameObject> tiles = new List<GameObject>();
    public Collider collider;
    public GameObject chunkObject;
    private CreateWorld createWorld;
    public Vector3 offset = new Vector3(0,0,0);

    public Chunk(int x, int z, CreateWorld createWorld, float chunkSize)
    {
        chunkX = x;
        chunkZ = z;
        this.createWorld = createWorld;

        chunkObject = new GameObject($"Chunk ({chunkX}, {chunkZ})");
        chunkObject.transform.parent = createWorld.parentObject;

        float halfChunkSize = (chunkSize - 1) / 4f;
        chunkObject.transform.position = new Vector3(chunkX * chunkSize / 2, 0, chunkZ * chunkSize / 2) + new Vector3(halfChunkSize, 0, halfChunkSize);

        BoxCollider boxCollider = chunkObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        collider = boxCollider;

        TriggerHandler triggerHandler = chunkObject.AddComponent<TriggerHandler>();
        triggerHandler.Initialize(createWorld, chunkObject.transform.position);
    }

    public void AddTile(GameObject tile, float chunkSize)
    {
        tiles.Add(tile);
        SetColliderSize(chunkSize);
    }

    private void SetColliderSize(float chunkSize)
    {
        Renderer tileRenderer = createWorld.tilePrefab.GetComponent<Renderer>();
        Vector3 tileSize = tileRenderer.bounds.size;

        BoxCollider boxCollider = collider as BoxCollider;
        if (boxCollider != null)
        {
            boxCollider.size = new Vector3(chunkSize * tileSize.x, 50f, chunkSize * tileSize.z) + offset;
        }
    }

    public void SetActive(bool active)
    {
        foreach (GameObject tile in tiles)
        {
            tile.SetActive(active);
        }
    }
}
