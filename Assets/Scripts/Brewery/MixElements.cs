using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MixElements : MonoBehaviour
{
    public ContentManager contentManager;
    public GameObject acceptOrNotDisplay;
    public Image CreateButton;
    public AcceptMix acceptMix;
    public List<string> elements = new List<string>();
    public List<string> specialElements = new List<string> { "World", "Weapon", "Cloud" };
    public List<string> basicElements = new List<string> { "Fire", "Water", "Lightning" };

    [Header("Mixable Weapons")]
    public Weapon fireWeapon;
    public Weapon lightningWeapon;
    public Weapon waterWeapon;

    [Header("Mixable Clouds")]
    public Cloud fireCloud;
    public Cloud lightningCloud;
    public Cloud waterCloud;

    public void Mix()
    {
        // Clear previous elements to avoid duplicates
        elements.Clear();

        // Find all GameObjects tagged as "Potion"
        GameObject[] potionGameObjects = GameObject.FindGameObjectsWithTag("Potion");

        foreach (GameObject potionGameObject in potionGameObjects)
        {
            // Get the ElementReference component attached to the GameObject
            ElementReference elementReference = potionGameObject.GetComponent<ElementReference>();

            if (elementReference != null && elementReference.element != null)
            {
                // Add the potion's type to the list
                elements.Add(elementReference.element.potionType.ToString());
            }
            else
            {
                Debug.LogWarning($"No ElementReference component or element found on {potionGameObject.name}");
            }
        }

        // Calculate and display the reward
        if (CalculateReward())
        {
            acceptOrNotDisplay.SetActive(true); // Show the accept/reject panel
        }
        else
        {
            acceptOrNotDisplay.SetActive(false); // Hide the panel if no valid mix
            StartCoroutine(FlashButtonRed()); // Flash the button red
        }
    }


    private IEnumerator FlashButtonRed()
    {
        CreateButton.color = Color.red; // Set the button to red
        yield return new WaitForSeconds(0.5f); // Wait for the flash duration
        CreateButton.color = Color.white; // Reset the button to its original color
    }




    private bool CalculateReward()
    {
        if (elements.Count > 0)
        {
            // Separate special and basic elements
            var specialElement = elements.FirstOrDefault(e => specialElements.Contains(e));
            var basicElementCounts = elements.Where(e => basicElements.Contains(e))
                                             .GroupBy(e => e)
                                             .ToDictionary(g => g.Key, g => g.Count());

            if (!string.IsNullOrEmpty(specialElement) && basicElementCounts.Count > 0)
            {
                // Find the most common basic element
                var mostCommonBasicElement = basicElementCounts.OrderByDescending(x => x.Value).FirstOrDefault();

                Debug.Log($"Special Element: {specialElement}");
                Debug.Log($"Most Common Basic Element: {mostCommonBasicElement.Key} with {mostCommonBasicElement.Value} occurrences.");

                // Determine and display the reward
                string reward = DetermineReward(specialElement, mostCommonBasicElement.Key);
                if (!string.IsNullOrEmpty(reward))
                {
                    if (IsRewardAlreadyOwned(reward)) // Check directly in the inventory
                    {
                        StartCoroutine(FlashButtonRed()); // Flash red if duplicate
                        return false; // Invalid mix as the item is already owned
                    }

                    Debug.Log($"You have created: {reward}");
                    return true; // Valid mix found
                }
            }
        }

        Debug.Log("No valid combination of elements found.");
        acceptMix.item = null;
        return false; // Invalid mix
    }

    private bool IsRewardAlreadyOwned(string reward)
    {
        var inventory = PlayerInventory.Instance;

        // Check if the reward exists in weapons or clouds
        if (inventory.weapons.Any(w => w.name == reward) || inventory.clouds.Any(c => c.name == reward))
        {
            return true;
        }

        return false;
    }
    private string DetermineReward(string specialElement, string mostCommonBasicElement)
    {
        // Define reward mappings for each combination
        var rewardMappings = new Dictionary<string, Dictionary<string, (string name, Sprite image, GameObject item, Weapon weapon, Cloud cloud)>>
        {
            {
                "Cloud", new Dictionary<string, (string, Sprite, GameObject, Weapon, Cloud)>
                {
                    { "Fire", ("Devil's Cloud", fireCloud.image, fireCloud.cloudGameObject, null, fireCloud) },
                    { "Water", ("Water Cloud", waterCloud.image, waterCloud.cloudGameObject, null, waterCloud) },
                    { "Lightning", ("Lightning Cloud", lightningCloud.image, lightningCloud.cloudGameObject, null, lightningCloud) }
                }
            },
            {
                "Weapon", new Dictionary<string, (string, Sprite, GameObject, Weapon, Cloud)>
                {
                    { "Fire", ("Devil's Spear", fireWeapon.image, fireWeapon.projectile, fireWeapon, null) },
                    { "Water", ("Trident", waterWeapon.image, waterWeapon.projectile, waterWeapon, null) },
                    { "Lightning", ("Lightning Bolt", lightningWeapon.image, lightningWeapon.projectile, lightningWeapon, null) }
                }
            }
        };

        // Check if the special element exists in the mapping
        if (rewardMappings.TryGetValue(specialElement, out var basicElementMappings))
        {
            // Check if the most common basic element exists for the special element
            if (basicElementMappings.TryGetValue(mostCommonBasicElement, out var rewardDetails))
            {
                // Apply the reward details
                acceptMix.ChangeItemInfo(rewardDetails.image, rewardDetails.name);
                acceptMix.item = rewardDetails.item;
                acceptMix.weapon = rewardDetails.weapon;
                acceptMix.cloud = rewardDetails.cloud;
                return rewardDetails.name;
            }
            else
            {
                Debug.LogWarning($"Unhandled basic element type: {mostCommonBasicElement}");
            }
        }
        else
        {
            Debug.LogWarning($"Unhandled special element type: {specialElement}");
        }

        return null; // No valid reward found
    }

}
