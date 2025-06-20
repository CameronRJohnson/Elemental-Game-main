using UnityEngine;

public class CloudContentManager : MonoBehaviour
{
    public Transform inventoryParent;
    public GameObject inventorySlotPrefab;
    public GameObject cloudItemPrefab;

    void Start() {
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        foreach (Transform child in inventoryParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Cloud cloud in PlayerInventory.Instance.clouds)
        {
            GameObject newSlot = Instantiate(inventorySlotPrefab, inventoryParent);
            GameObject newItem = Instantiate(cloudItemPrefab, newSlot.transform);
            CloudItem cloudItem = newItem.GetComponent<CloudItem>();
            cloudItem.InitializeCloud(cloud);
        }
    }
}
