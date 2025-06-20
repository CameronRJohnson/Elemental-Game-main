using UnityEngine;

public class ContentManager : MonoBehaviour
{
    public Transform inventoryParent; // The parent object for inventory slots
    public GameObject inventorySlotPrefab; // Prefab for the inventory slot
    public GameObject inventoryItemPrefab; // Prefab for the inventory item
    public PotionSpawner potionSpawner; // Reference to PotionSpawner

    // Reference to the World Star UI object
    public ElementInventoryItem worldStarUI; // Non-prefab object in the hierarchy
    public ElementInventoryItem cloudStarUI;
    public ElementInventoryItem weaponStarUI;

    // Call this method to update the inventory UI
    public void UpdateInventoryUI()
    {
        // Clear existing inventory slots if needed
        foreach (Transform child in inventoryParent)
        {
            Destroy(child.gameObject);
        }

        // Update collected potions in the inventory
        foreach (Element element in PlayerInventory.Instance.collectedPotions)
        {
            if (!element.isCloudStar && !element.isWorldStar && !element.isWeaponStar)
            {
                // Instantiate the inventory slot
                GameObject newSlot = Instantiate(inventorySlotPrefab, inventoryParent);

                // Instantiate the inventory item and set it as a child of the slot
                GameObject newItem = Instantiate(inventoryItemPrefab, newSlot.transform);

                // Initialize the inventory item (assume an InventoryItem script is on the prefab)
                ElementInventoryItem inventoryItem = newItem.GetComponent<ElementInventoryItem>();
                inventoryItem.InitialisePotion(element, potionSpawner);
            }
            else if (element.isWorldStar)
            {
                worldStarUI.InitialisePotion(element, potionSpawner);
            }
            else if (element.isCloudStar)
            {
                cloudStarUI.InitialisePotion(element, potionSpawner);
            }
            else if (element.isWeaponStar)
            {
                weaponStarUI.InitialisePotion(element, potionSpawner);
            }
        }
    }
    public void SetTextToZero() {
        cloudStarUI.Set();
        weaponStarUI.Set();
        worldStarUI.Set();
    }

}
