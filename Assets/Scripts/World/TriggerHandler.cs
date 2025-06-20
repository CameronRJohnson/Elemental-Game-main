using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    private CreateWorld createWorld;
    private Vector3 chunkPosition;

    public void Initialize(CreateWorld createWorld, Vector3 chunkPosition)
    {
        this.createWorld = createWorld;
        this.chunkPosition = chunkPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (createWorld != null)
        {
            createWorld.ActivateNeighboringChunks(chunkPosition);
        }
    }
}