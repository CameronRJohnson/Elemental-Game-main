
using UnityEngine;
using UnityEngine.UI;

public class ProjectileShooter : MonoBehaviour
{
    [Header("Projectile Settings")]
    public Weapon projectilePrefab; // Reference to the Weapon ScriptableObject
    public Transform shootPoint; // Point from where the projectile is fired
    public Transform player; // Player object to determine facing direction
    public float shootRadius = 10f; // Radius to detect enemies
    public float fireRate = 1f; // Time between shots
    public float spawnOffset = 1.5f; // Distance to offset the projectile from the player
    private float nextFireTime = 0f; // Tracks when the player can shoot again

    void Start() {
        projectilePrefab = PlayerInventory.Instance.selectedWeapon;
    }
    
    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            ShootAtNearestEnemy();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void ShootAtNearestEnemy()
    {
        if (projectilePrefab == null || shootPoint == null || player == null || projectilePrefab.projectile == null)
        {
            return;
        }

        Collider[] hits = Physics.OverlapSphere(player.position, shootRadius);
        Transform nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(player.position, hit.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = hit.transform;
                }
            }
        }

        if (nearestEnemy != null)
        {
            Vector3 direction = (nearestEnemy.position - shootPoint.position).normalized;
            Vector3 spawnPosition = shootPoint.position + direction * spawnOffset;
            Quaternion projectileRotation = Quaternion.LookRotation(direction);

            GameObject newProjectile = Instantiate(projectilePrefab.projectile, spawnPosition, projectileRotation);

            Projectile projectileComponent = newProjectile.GetComponent<Projectile>();
            if (projectileComponent != null)
            {
                projectileComponent.weaponData = projectilePrefab;
            }
            else
            {
                Debug.LogWarning("ProjectileShooter: The instantiated projectile is missing a Projectile script.");
            }
        }
    }
}
