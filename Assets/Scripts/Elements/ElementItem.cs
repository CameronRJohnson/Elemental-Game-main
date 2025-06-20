using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementReference : MonoBehaviour
{
    [SerializeField]
    public Element element;

    private bool isPotionCollected = false; // Flag to prevent repeated collection


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPotionCollected)
        {
            isPotionCollected = true; // Set the flag to true when collected
            Destroy(gameObject); // Destroy the object after collection
            {
                PlayerInventory.Instance.AddPotionToInventory(element);
            }
        }
    }
}

