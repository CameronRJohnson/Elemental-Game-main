using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon System/Weapon")]
public class Weapon : ScriptableObject
{
    [Header("Weapon Details")]
    public Sprite image; // Icon or visual representation of the potion

    [Header("Projectile Settings")]
    public float damage = 10f;
    public float luck = 0f;
    public float homing = 0f;

    [Header("Hidden Projectile Settings")]
    public float speed = 20f;
    public float timeBetweenShots = 0.5f;
    public float destroyDelay = 5f;

    [Header("Game Object Reference")]
    public GameObject projectile; // Reference to the GameObject prefab for this potion

    private float nextShootTime = 0f;

    public virtual void Fire()
    {
        if (Time.time < nextShootTime)
        {
            Debug.Log($"{this.GetType().Name}: Cooldown not finished.");
            Destroy(projectile);
            return;
        }

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = GetLaunchDirection() * speed;
        }
        else
        {
            Debug.LogWarning($"{this.GetType().Name}: Rigidbody not found on projectile.");
        }

        nextShootTime = Time.time + timeBetweenShots;

        // Start destruction timer (via MonoBehaviour on the projectile)
        MonoBehaviour projectileBehaviour = projectile.GetComponent<MonoBehaviour>();
        if (projectileBehaviour != null)
        {
            projectileBehaviour.StartCoroutine(DestroyAfterDelay());
        }
    }

    protected virtual Vector3 GetLaunchDirection()
    {
        return projectile.transform.forward;
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(projectile);
    }
}
