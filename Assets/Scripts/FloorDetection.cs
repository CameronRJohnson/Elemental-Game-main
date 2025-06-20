using UnityEngine;

public class FloorDetection : MonoBehaviour
{
    public ContentManager contentManager;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Potion"))
        {
            // Get the ElementReference component attached to the potion GameObject
            ElementReference potionComponent = collision.gameObject.GetComponent<ElementReference>();

            if (potionComponent != null && potionComponent.element != null)
            {
                // Add the potion back to the player's inventory
                PlayerInventory.Instance.AddPotionToInventory(potionComponent.element);

                // Destroy the potion GameObject
                Destroy(collision.gameObject);

                Debug.Log($"{potionComponent.element.name} was added back to the inventory.");
            }
            else
            {
                Debug.LogWarning($"No ElementReference or Element found on {collision.gameObject.name}");
            }
        }
        contentManager.UpdateInventoryUI();
    }
}
