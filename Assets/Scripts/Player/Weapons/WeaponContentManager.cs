using UnityEngine;

public class WeaponContentManager : MonoBehaviour
{
    public Transform inventoryParent; // The parent object for inventory slots
    public GameObject inventorySlotPrefab; // Prefab for the inventory slot
    public GameObject weaponItemPrefab; // Prefab for the weapon item

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
        foreach (Weapon weapon in PlayerInventory.Instance.weapons)
        {
            // Instantiate the inventory slot
            GameObject newSlot = Instantiate(inventorySlotPrefab, inventoryParent);

            // Instantiate the inventory item and set it as a child of the slot
            GameObject newItem = Instantiate(weaponItemPrefab, newSlot.transform);

            // Initialize the inventory item (assume an InventoryItem script is on the prefab)
            WeaponItem weaponItem = newItem.GetComponent<WeaponItem>();
            weaponItem.InitializeWeapon(weapon);
        }
    }
}
