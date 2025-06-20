using UnityEngine;

public class CloudContentManager : MonoBehaviour
{
    public Transform inventoryParent; // The parent object for inventory slots
    public GameObject inventorySlotPrefab; // Prefab for the inventory slot
    public GameObject cloudItemPrefab;

    void Start() {
        UpdateInventoryUI();
    }


    // Call this method to update the inventory UI
    public void UpdateInventoryUI()
    {
        // Clear existing inventory slots if needed
        foreach (Transform child in inventoryParent)
        {
            Destroy(child.gameObject);
        }

        // Update collected potions in the inventory
        foreach (Cloud cloud in PlayerInventory.Instance.clouds)
        {
            // Instantiate the inventory slot
            GameObject newSlot = Instantiate(inventorySlotPrefab, inventoryParent);

            // Instantiate the inventory item and set it as a child of the slot
            GameObject newItem = Instantiate(cloudItemPrefab, newSlot.transform);

            // Initialize the inventory item (assume an InventoryItem script is on the prefab)
            CloudItem cloudItem = newItem.GetComponent<CloudItem>();
            cloudItem.InitializeCloud(cloud);
        }
    }
}
