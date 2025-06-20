using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable Object/Potion")]
public class Element : ScriptableObject
{
    [Header("Potion Details")]
    public Sprite image;
    public string description;
    public PotionType potionType;

    [Header("Potion Attributes")]
    [Min(0)] 
    public float numberCollected;

    [Header("Star Attributes")]
    public bool isWorldStar;
    public bool isCloudStar;
    public bool isWeaponStar;

    [Header("Game Object Reference")]
    public GameObject potionGameObject;
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