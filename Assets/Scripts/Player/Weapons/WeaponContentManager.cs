using UnityEngine;

public class WeaponContentManager : MonoBehaviour
{
    public Transform inventoryParent;
    public GameObject inventorySlotPrefab;
    public GameObject weaponItemPrefab;

    void Start() {
        UpdateInventoryUI();
    }


    public void UpdateInventoryUI()
    {
        foreach (Transform child in inventoryParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Weapon weapon in PlayerInventory.Instance.weapons)
        {
            GameObject newSlot = Instantiate(inventorySlotPrefab, inventoryParent);

            GameObject newItem = Instantiate(weaponItemPrefab, newSlot.transform);

            WeaponItem weaponItem = newItem.GetComponent<WeaponItem>();
            weaponItem.InitializeWeapon(weapon);
        }
    }
}
