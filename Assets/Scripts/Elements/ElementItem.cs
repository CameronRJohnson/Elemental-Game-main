using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementReference : MonoBehaviour
{
    [SerializeField]
    public Element element;

    private bool isPotionCollected = false;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPotionCollected)
        {
            isPotionCollected = true;
            Destroy(gameObject);
            {
                PlayerInventory.Instance.AddPotionToInventory(element);
            }
        }
    }
}

