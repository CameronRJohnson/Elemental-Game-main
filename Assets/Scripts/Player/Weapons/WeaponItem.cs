using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponItem : MonoBehaviour
{
    private Weapon weapon;
    public Image image;

    public void InitializeWeapon(Weapon newWeapon)
    {
        weapon = newWeapon;
        image.sprite = weapon.image;
    }

    public void ChangeSelectedWeapon()
    {
        var playerInventory = PlayerInventory.Instance;

        if (playerInventory != null && playerInventory.weapons.Contains(weapon))
        {
            playerInventory.selectedWeapon = weapon;
            playerInventory.UpdateAllImages();
            playerInventory.SaveInventory();
        }
        else
        {
            Debug.LogWarning("WeaponItem: Weapon not found in player inventory.");
        }

    }
}
