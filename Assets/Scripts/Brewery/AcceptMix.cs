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

    public void AcceptItem()
    {
        if (item != null)
        {
            string itemTag = item.tag;

            if (itemTag == "Cloud")
            {
                Debug.Log($"Accepted a Cloud: {item.name}");
                PlayerInventory.Instance.AddCloudToInventory(cloud);
            }
            else if (itemTag == "Weapon")
            {
                Debug.Log($"Accepted a Weapon: {item.name}");
                PlayerInventory.Instance.AddWeaponToInventory(weapon);
            }
            else
            {
                Debug.LogWarning($"Item with tag {itemTag} is not handled in AcceptItem.");
            }

            ClearItemInfo();
        }
        else
        {
            Debug.LogWarning("No item to accept.");
        }
        ClearElements();
    }

    public void RejectItem()
    {
        if (item != null)
        {
            Debug.Log($"Rejected: {item.name}");
            ClearItemInfo();
            reset.RemovePotions();
            
        }
        else
        {
            Debug.LogWarning("No item to reject.");
        }
    }

    public void ChangeItemInfo(Sprite newItemImage, string newItemName)
    {
        itemImage.sprite = newItemImage;
        itemInfoText.text = newItemName;
        Debug.Log($"Displaying item: {newItemName}");
    }

    private void ClearItemInfo()
    {
        itemImage.sprite = null;
        itemInfoText.text = string.Empty;

        item = null;
    }

    private void ClearElements() {
        GameObject[] elements = GameObject.FindGameObjectsWithTag("Potion");
        
        foreach (GameObject element in elements)
        {
            Destroy(element);
        }

    }
}
