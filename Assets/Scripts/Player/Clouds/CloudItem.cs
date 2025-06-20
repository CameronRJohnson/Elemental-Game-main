using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloudItem : MonoBehaviour
{
    private Cloud cloud;
    public Image image;

    public void InitializeCloud(Cloud newCloud)
    {
        cloud = newCloud;
        image.sprite = cloud.image;
    }

    public void ChangeSelectedCloud()
    {
        var playerInventory = PlayerInventory.Instance;

        if (playerInventory != null && playerInventory.clouds.Contains(cloud))
        {
            playerInventory.selectedCloud = cloud;
            playerInventory.UpdateAllImages();
            playerInventory.SaveInventory();
        }
        else
        {
            Debug.LogWarning("WeaponItem: Weapon not found in player inventory.");
        }

    }
}
