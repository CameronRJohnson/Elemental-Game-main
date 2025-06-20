using UnityEngine;

public class Reset : MonoBehaviour
{
    public ContentManager contentManager;

    public void RemovePotions()
    {
        GameObject[] potionGameObjects = GameObject.FindGameObjectsWithTag("Potion");

        foreach (GameObject potionGameObject in potionGameObjects)
        {
            ElementReference potionComponent = potionGameObject.GetComponent<ElementReference>();

            if (potionComponent != null && potionComponent.element != null)
            {
                PlayerInventory.Instance.AddPotionToInventory(potionComponent.element);

                Destroy(potionGameObject);
            }
            else
            {
                Debug.LogWarning($"No PotionSpawnedComponent or Potion found on {potionGameObject.name}");
            }
        }

        contentManager.UpdateInventoryUI();
    }
}
