using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }
    private string saveFilePath;
    [Header("Player Cloud Info")]
    public ChangeCloud changeCloud;

    [Header("Player Inventory")]
    public List<Element> collectedPotions = new List<Element>();
    public List<Weapon> weapons = new List<Weapon>();
    public List<Cloud> clouds = new List<Cloud>();

    [Header("References")]
    public PlayerStats playerStats;

    [Header("Player Images")]
    public Image cloudImage;
    public Image weaponImage;

    [Header("Player Stats")]
    public Cloud selectedCloud;
    public Weapon selectedWeapon;

    [Header("Starting Stuff")]
    public Cloud startingCloud;
    public Weapon startingWeapon;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            saveFilePath = Path.Combine(Application.persistentDataPath, "inventory.json");
            LoadInventory();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        if (weapons.Count == 0)
        {
            AddWeaponToInventory(startingWeapon);
            selectedWeapon = startingWeapon;
        }

        if (clouds.Count == 0)
        {
            AddCloudToInventory(startingCloud);
            selectedCloud = startingCloud;
        }

        SaveInventory();

        UpdateAllImages();
    }

    public void AddPotionToInventory(Element pickedUpPotion)
    {
        if (pickedUpPotion == null)
        {
            Debug.LogWarning("Picked up potion is null!");
            return;
        }

        if (collectedPotions.Contains(pickedUpPotion))
        {
            Element existingPotion = collectedPotions.Find(p => p.name == pickedUpPotion.name);
            existingPotion.numberCollected++;
            return;
        }

        pickedUpPotion.numberCollected = 1;
        collectedPotions.Add(pickedUpPotion);
    }

    public void AddWeaponToInventory(Weapon newWeapon)
    {
        if (newWeapon == null)
        {
            Debug.LogWarning("Weapon is null!");
            return;
        }

        if (!weapons.Contains(newWeapon))
        {
            weapons.Add(newWeapon);

            Debug.Log($"{newWeapon.name} added to inventory");
        }
        else Debug.LogError("This is already made!");
    }

    public void AddCloudToInventory(Cloud newCloud)
    {
        if (newCloud == null)
        {
            Debug.LogWarning("Cloud is null!");
            return;
        }

        if (!clouds.Contains(newCloud))
        {
            clouds.Add(newCloud);
            Debug.Log($"{newCloud.name} added to inventory");
        }
        else Debug.LogError("This is already made!");
    }

    public void ResetInventory()
    {
        collectedPotions.Clear();
        weapons.Clear();
        clouds.Clear();

        SaveInventory();
        SceneManager.LoadScene("Home");
        Debug.Log("Inventory has been reset.");
    }

    public void SaveInventory()
    {
        InventoryWrapper wrapper = new InventoryWrapper(collectedPotions, weapons, clouds, selectedWeapon, selectedCloud);

        string json = JsonUtility.ToJson(wrapper);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Inventory saved.");
    }

    private void LoadInventory()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            InventoryWrapper wrapper = JsonUtility.FromJson<InventoryWrapper>(json);

            collectedPotions.Clear();
            weapons.Clear();
            clouds.Clear();

            if (wrapper != null)
            {
                if (wrapper.potions != null)
                    collectedPotions.AddRange(wrapper.potions);

                if (wrapper.weapons != null)
                    weapons.AddRange(wrapper.weapons);

                if (wrapper.clouds != null)
                    clouds.AddRange(wrapper.clouds);

                selectedWeapon = wrapper.selectedWeapon;
                selectedCloud = wrapper.selectedCloud;

                Debug.Log("Inventory loaded successfully.");
            }
        }
        else
        {
            Debug.Log("No inventory file found.");
        }
    }

    public void UpdateAllImages()
    {
        if (selectedCloud == null || selectedCloud.image == null)
        {
            Debug.LogError("SelectedCloud or its image is null!");
        }
        else
        {
            cloudImage.sprite = selectedCloud.image;
            changeCloud.SetCloudObject(selectedCloud.cloudGameObject.GetComponent<MeshRenderer>(), selectedCloud.cloudGameObject.GetComponent<MeshFilter>());
        }

        if (selectedWeapon == null || selectedWeapon.image == null)
        {
            Debug.LogError("SelectedWeapon or its image is null!");
        }
        else
        {
            weaponImage.sprite = selectedWeapon.image;
        }
        if (playerStats != null) {
            playerStats.InitalizeBoxes();
        }
    }
}

[System.Serializable]
public class InventoryWrapper
{
    public List<Element> potions;
    public List<Weapon> weapons;
    public List<Cloud> clouds;
    public Weapon selectedWeapon;
    public Cloud selectedCloud;

    public InventoryWrapper(List<Element> potions, List<Weapon> weapons, List<Cloud> clouds, Weapon selectedWeapon, Cloud selectedCloud)
    {
        this.potions = potions;
        this.weapons = weapons;
        this.clouds = clouds;
        this.selectedWeapon = selectedWeapon;
        this.selectedCloud = selectedCloud;
    }
}