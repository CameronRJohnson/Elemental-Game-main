using UnityEngine;
using UnityEngine.UI;

public class ElementInventoryItem : MonoBehaviour
{
    public Image itemImage;
    private Element potion;
    private PotionSpawner potionSpawner;
    public Text numberCollected;

    public void InitialisePotion(Element newPotion, PotionSpawner spawner)
    {
        potion = newPotion;
        potionSpawner = spawner;
        itemImage.sprite = potion.image;
        numberCollected.text = potion.numberCollected.ToString();
    }

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
