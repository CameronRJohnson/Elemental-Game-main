using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PotionSpawner : MonoBehaviour
{
    [Header("Potion Configuration")]
    public GameObject potionObject;
    public Transform potionContainer;
    public Element element;

    [Header("UI Elements")]
    public Text elementName;
    public Text elementDescription;
    public Image elementImage;
    public ContentManager contentManager;

    [Header("Launch Configuration")]
    public float baseLaunchVelocityY = 230f;
    public float baseLaunchVelocityZ = 130f;
    public Vector2 randomRangeVelocityY = new Vector2(10, 50);
    public Vector2 randomRangeVelocityZ = new Vector2(5, 25);
    public Vector2 randomRangeVelocityX = new Vector2(-10, 10);

    void Update()
    {
        if (potionObject == null || element == null) return;

        if ((Input.GetButtonDown("Fire1") || IsTouchInput()) && !IsPointerOverUI())
        {
            TrySpawnPotion();
        }
    }

    private void TrySpawnPotion()
    {
        if (PlayerInventory.Instance == null)
        {
            Debug.LogError("PlayerInventory instance is null!");
            return;
        }

        Element playerPotion = PlayerInventory.Instance.collectedPotions.Find(p => p.name == element.name);

        if (playerPotion == null)
        {
            ResetFields();
            contentManager?.UpdateInventoryUI();
            return;
        }

        if (playerPotion.numberCollected <= 0)
        {
            PlayerInventory.Instance.collectedPotions.Remove(playerPotion);
            ResetFields();
            contentManager?.UpdateInventoryUI();
            return;
        }

        if (IsStarPotion(playerPotion) && IsStarAlreadySpawned())
        {
            return;
        }

        playerPotion.numberCollected--;
        SpawnPotion();

        if (playerPotion.numberCollected == 0)
        {
            PlayerInventory.Instance.collectedPotions.Remove(playerPotion);
            ResetFields();
            contentManager?.SetTextToZero();
        }

        contentManager?.UpdateInventoryUI();
    }

    private void SpawnPotion()
    {
        float launchVelocityY = baseLaunchVelocityY + Random.Range(randomRangeVelocityY.x, randomRangeVelocityY.y);
        float launchVelocityZ = baseLaunchVelocityZ + Random.Range(randomRangeVelocityZ.x, randomRangeVelocityZ.y);
        float launchVelocityX = Random.Range(randomRangeVelocityX.x, randomRangeVelocityX.y);

        GameObject spawnedPotion = Instantiate(potionObject, transform.position, transform.rotation, potionContainer);
        Rigidbody rb = spawnedPotion.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddRelativeForce(new Vector3(launchVelocityX, launchVelocityY, launchVelocityZ));
        }
    }

    public void ResetFields()
    {
        potionObject = null;
        element = null;

        if (elementName != null) elementName.text = "";
        if (elementDescription != null) elementDescription.text = "";
        if (elementImage != null)
        {
            elementImage.enabled = false;
            elementImage.sprite = null;
        }
    }

    public void UpdateUI()
    {
        if (element == null) return;

        if (elementName != null) elementName.text = element.name;
        if (elementDescription != null) elementDescription.text = element.description;
        if (elementImage != null)
        {
            elementImage.sprite = element.image;
            elementImage.enabled = true;
        }
    }

    private bool IsTouchInput()
    {
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return true;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return true;
        }

        return false;
    }

    private bool IsStarPotion(Element potionToCheck)
    {
        return potionToCheck.isWorldStar || potionToCheck.isCloudStar || potionToCheck.isWeaponStar;
    }

    private bool IsStarAlreadySpawned()
    {
        foreach (Transform child in potionContainer)
        {
            ElementReference spawnedPotion = child.GetComponent<ElementReference>();
            if (spawnedPotion != null && IsStarPotion(spawnedPotion.element))
            {
                return true;
            }
        }
        return false;
    }
}
