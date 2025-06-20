using UnityEngine;

public class Reset : MonoBehaviour
{
    public ContentManager contentManager;

    public void RemovePotions()
    {
        // Find all GameObjects tagged as "Potion"
        GameObject[] potionGameObjects = GameObject.FindGameObjectsWithTag("Potion");

        foreach (GameObject potionGameObject in potionGameObjects)
        {
            // Get the PotionSpawnedComponent attached to the GameObject
            ElementReference potionComponent = potionGameObject.GetComponent<ElementReference>();

            if (potionComponent != null && potionComponent.element != null)
            {
                // Add the potion to the player's inventory
                PlayerInventory.Instance.AddPotionToInventory(potionComponent.element);

                // Remove the potion GameObject from the scene
                Destroy(potionGameObject);
            }
            else
            {
                Debug.LogWarning($"No PotionSpawnedComponent or Potion found on {potionGameObject.name}");
            }
        }

        // Update the inventory UI
        contentManager.UpdateInventoryUI();
    }
}
