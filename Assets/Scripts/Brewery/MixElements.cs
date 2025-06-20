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
        elements.Clear();

        GameObject[] potionGameObjects = GameObject.FindGameObjectsWithTag("Potion");

        foreach (GameObject potionGameObject in potionGameObjects)
        {
            ElementReference elementReference = potionGameObject.GetComponent<ElementReference>();

            if (elementReference != null && elementReference.element != null)
            {
                elements.Add(elementReference.element.potionType.ToString());
            }
            else
            {
                Debug.LogWarning($"No ElementReference component or element found on {potionGameObject.name}");
            }
        }

        if (CalculateReward())
        {
            acceptOrNotDisplay.SetActive(true);
        }
        else
        {
            acceptOrNotDisplay.SetActive(false);
            StartCoroutine(FlashButtonRed());
        }
    }


    private IEnumerator FlashButtonRed()
    {
        CreateButton.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        CreateButton.color = Color.white;
    }




    private bool CalculateReward()
    {
        if (elements.Count > 0)
        {
            var specialElement = elements.FirstOrDefault(e => specialElements.Contains(e));
            var basicElementCounts = elements.Where(e => basicElements.Contains(e))
                                             .GroupBy(e => e)
                                             .ToDictionary(g => g.Key, g => g.Count());

            if (!string.IsNullOrEmpty(specialElement) && basicElementCounts.Count > 0)
            {
                var mostCommonBasicElement = basicElementCounts.OrderByDescending(x => x.Value).FirstOrDefault();

                Debug.Log($"Special Element: {specialElement}");
                Debug.Log($"Most Common Basic Element: {mostCommonBasicElement.Key} with {mostCommonBasicElement.Value} occurrences.");

                string reward = DetermineReward(specialElement, mostCommonBasicElement.Key);
                if (!string.IsNullOrEmpty(reward))
                {
                    if (IsRewardAlreadyOwned(reward))
                    {
                        StartCoroutine(FlashButtonRed());
                        return false;
                    }

                    Debug.Log($"You have created: {reward}");
                    return true;
                }
            }
        }

        Debug.Log("No valid combination of elements found.");
        acceptMix.item = null;
        return false;
    }

    private bool IsRewardAlreadyOwned(string reward)
    {
        var inventory = PlayerInventory.Instance;

        if (inventory.weapons.Any(w => w.name == reward) || inventory.clouds.Any(c => c.name == reward))
        {
            return true;
        }

        return false;
    }
    private string DetermineReward(string specialElement, string mostCommonBasicElement)
    {
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

        if (rewardMappings.TryGetValue(specialElement, out var basicElementMappings))
        {
            if (basicElementMappings.TryGetValue(mostCommonBasicElement, out var rewardDetails))
            {
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

        return null;
    }

}
