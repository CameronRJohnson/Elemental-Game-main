using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCloud", menuName = "Cloud System/Cloud")]
public class Cloud : ScriptableObject
{
    [Header("Cloud Details")]
    public Sprite image;

    [Header("Cloud Settings")]
    public float defence = 0f;
    public float strength = 0;
    public float speed = 0f;

    [Header("Game Object Reference")]
    public GameObject cloudGameObject;
    internal object sprite;
}
