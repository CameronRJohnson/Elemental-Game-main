using UnityEngine;

public class FloorDetection : MonoBehaviour
{
    public ContentManager contentManager;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Potion"))
        {
            ElementReference potionComponent = collision.gameObject.GetComponent<ElementReference>();

            if (potionComponent != null && potionComponent.element != null)
            {
                PlayerInventory.Instance.AddPotionToInventory(potionComponent.element);

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
