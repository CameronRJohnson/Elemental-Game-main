using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(menuName = "Scriptable Object/Potion")]
public class Element : ScriptableObject
{
    [Header("Potion Details")]
    public Sprite image; // Icon or visual representation of the potion
    public string description; // Description of the potion's effects
    public PotionType potionType; // The type of potion

    [Header("Potion Attributes")]
    [Min(0)] 
    public float numberCollected; // Track how many potions of this type are collected (use int if fractional collection isn't needed)

    [Header("Star Attributes")]
    public bool isWorldStar; // Indicates if the potion is a World Star
    public bool isCloudStar; // Indicates if the potion is a Cloud Star
    public bool isWeaponStar; // Indicates if the potion is a Weapon Star

    [Header("Game Object Reference")]
    public GameObject potionGameObject; // Reference to the GameObject prefab for this potion
}


public enum PotionType {
    Water,
    Air,
    Fire,
    Dark,
    Electric,
    Candy,
    Earth,
    World,
    Cloud,
    Weapon
}