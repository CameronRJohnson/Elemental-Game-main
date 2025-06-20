using UnityEngine;
using UnityEngine.UI;

public class ElementInventoryItem : MonoBehaviour
{
    public Image itemImage; // Image to display the potion sprite
    private Element potion; // ScriptableObject potion reference
    private PotionSpawner potionSpawner; // Reference to the PotionSpawner script
    public Text numberCollected;

    // Initialize the InventoryItem with Potion data
    public void InitialisePotion(Element newPotion, PotionSpawner spawner)
    {
        potion = newPotion;
        potionSpawner = spawner;
        itemImage.sprite = potion.image;
        numberCollected.text = potion.numberCollected.ToString();
    }

    // Update the selected potion in PotionSpawner
    public void ChangeSelectedPotion()
    {
        if (potionSpawner != null && potion != null)
        {
            potionSpawner.potionObject = potion.potionGameObject;
            potionSpawner.element = potion;
            potionSpawner.UpdateUI();
        }
    }

    public void Set() {
        numberCollected.text = "0";
    }
    
}
