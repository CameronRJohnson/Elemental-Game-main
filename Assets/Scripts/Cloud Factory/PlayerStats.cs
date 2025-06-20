using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public GameObject powerBoxPrefab;

    [Header("Player Content Boxes")]
    public GameObject defenseContentBox;
    public GameObject strengthContentBox;
    public GameObject speedContentBox;
    public GameObject damageContentBox;
    public GameObject luckContentBox;
    public GameObject homingContentBox;

    [Header("Player Selected Images")]
    public Image cloudImage;
    public Image weaponImage;



    void Start() {
        InitalizeBoxes();
    }

    public void InitalizeBoxes() {
        InitalizeBox(defenseContentBox, PlayerInventory.Instance.selectedCloud.defence / 10);
        InitalizeBox(strengthContentBox, PlayerInventory.Instance.selectedCloud.strength / 10);
        InitalizeBox(speedContentBox, PlayerInventory.Instance.selectedCloud.speed / 10);
        InitalizeBox(damageContentBox, PlayerInventory.Instance.selectedWeapon.damage / 10);
        InitalizeBox(luckContentBox, PlayerInventory.Instance.selectedWeapon.luck / 10);
        InitalizeBox(homingContentBox, PlayerInventory.Instance.selectedWeapon.homing / 10);
        InitalizeImage(cloudImage, PlayerInventory.Instance.selectedCloud.image);
        InitalizeImage(weaponImage, PlayerInventory.Instance.selectedWeapon.image);
    }

    private void InitalizeBox(GameObject content, float playerInventoryReference) 
    {
        // Clear existing inventory slots if needed
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }

        // Loop through the playerInventoryReference to instantiate power boxes
        for (int i = 0; i < Mathf.FloorToInt(playerInventoryReference); i++) 
        {
            GameObject powerBox = Instantiate(powerBoxPrefab, content.transform);
            powerBox.name = $"PowerBox_{i + 1}";
        }
    }

    public void InitalizeImage(Image statsImage, Sprite selectedImage) {
        statsImage.sprite = selectedImage;
    }


}
