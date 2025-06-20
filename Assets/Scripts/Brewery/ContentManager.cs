using UnityEngine;

public class ContentManager : MonoBehaviour
{
    public Transform inventoryParent;
    public GameObject inventorySlotPrefab;
    public GameObject inventoryItemPrefab;
    public PotionSpawner potionSpawner;

    public ElementInventoryItem worldStarUI;
    public ElementInventoryItem cloudStarUI;
    public ElementInventoryItem weaponStarUI;

    public void UpdateInventoryUI()
    {
        foreach (Transform child in inventoryParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Element element in PlayerInventory.Instance.collectedPotions)
        {
            if (!element.isCloudStar && !element.isWorldStar && !element.isWeaponStar)
            {
                GameObject newSlot = Instantiate(inventorySlotPrefab, inventoryParent);

                GameObject newItem = Instantiate(inventoryItemPrefab, newSlot.transform);

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
