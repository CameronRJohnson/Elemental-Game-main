using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCloud : MonoBehaviour
{
    public MeshRenderer playerRenderer;
    public MeshFilter playerFilter;

    public void SetCloudObject(MeshRenderer newRenderer, MeshFilter newFilter)
    {
        if (newRenderer == null || newFilter == null)
        {
            Debug.LogError("New Renderer or Filter is null!");
            return;
        }

        if (playerRenderer != null)
        {
            playerRenderer.materials = newRenderer.sharedMaterials;
        }
        else
        {
            Debug.LogError("Player Renderer is not assigned!");
        }

        if (playerFilter != null)
        {
            playerFilter.mesh = newFilter.sharedMesh;
        }
        else
        {
            Debug.LogError("Player Filter is not assigned!");
        }
    }
}
