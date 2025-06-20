using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcceptMix : MonoBehaviour
{
    public Text itemInfoText;
    public Image itemImage;
    public GameObject item;
    public Reset reset;
    public Cloud cloud;
    public Weapon weapon;

    // Called when the user accepts the item
    public void AcceptItem()
    {
        if (item != null)
        {
            string itemTag = item.tag;

            if (itemTag == "Cloud")
            {
                Debug.Log($"Accepted a Cloud: {item.name}");
                // Add specific behavior for accepting Cloud items here
                PlayerInventory.Instance.AddCloudToInventory(cloud);
            }
            else if (itemTag == "Weapon")
            {
                Debug.Log($"Accepted a Weapon: {item.name}");
                // Add specific behavior for accepting Weapon items here
                PlayerInventory.Instance.AddWeaponToInventory(weapon);
            }
            else
            {
                Debug.LogWarning($"Item with tag {itemTag} is not handled in AcceptItem.");
            }

            ClearItemInfo(); // Clear UI after accepting
        }
        else
        {
            Debug.LogWarning("No item to accept.");
        }
        ClearElements();
    }

    // Called when the user rejects the item
    public void RejectItem()
    {
        if (item != null)
        {
            Debug.Log($"Rejected: {item.name}");
            ClearItemInfo(); // Clear UI after rejecting
            reset.RemovePotions();
            
        }
        else
        {
            Debug.LogWarning("No item to reject.");
        }
    }

    // Updates the UI with new item info
    public void ChangeItemInfo(Sprite newItemImage, string newItemName)
    {
        itemImage.sprite = newItemImage;
        itemInfoText.text = newItemName;
        Debug.Log($"Displaying item: {newItemName}");
    }

    // Clears the UI when no item is selected
    private void ClearItemInfo()
    {
        itemImage.sprite = null;
        itemInfoText.text = string.Empty;

        item = null; // Clear reference to the current item
    }

    private void ClearElements() {
        GameObject[] elements = GameObject.FindGameObjectsWithTag("Potion");
        
        foreach (GameObject element in elements)
        {
            Destroy(element);
        }

    }
}
