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

        // Update the player's MeshFilter and MeshRenderer with the new cloud object
        if (playerRenderer != null)
        {
            playerRenderer.materials = newRenderer.sharedMaterials; // Update materials
        }
        else
        {
            Debug.LogError("Player Renderer is not assigned!");
        }

        if (playerFilter != null)
        {
            playerFilter.mesh = newFilter.sharedMesh; // Update the mesh
        }
        else
        {
            Debug.LogError("Player Filter is not assigned!");
        }
    }
}
