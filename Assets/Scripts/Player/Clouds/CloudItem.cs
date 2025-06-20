using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloudItem : MonoBehaviour
{
    private Cloud cloud;
    public Image image;

    // Initialize the InventoryItem with Potion data
    public void InitializeCloud(Cloud newCloud)
    {
        cloud = newCloud;
        image.sprite = cloud.image;
    }

    public void ChangeSelectedCloud()
    {
        // Find the player's inventory
        var playerInventory = PlayerInventory.Instance;

        if (playerInventory != null && playerInventory.clouds.Contains(cloud))
        {
            // Changes the selected cloud
            playerInventory.selectedCloud = cloud;
            // Changes the image
            playerInventory.UpdateAllImages();
            // Saves this in the inventory
            playerInventory.SaveInventory();
        }
        else
        {
            Debug.LogWarning("WeaponItem: Weapon not found in player inventory.");
        }

    }
}
