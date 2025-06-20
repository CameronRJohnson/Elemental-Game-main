using UnityEngine;
using UnityEngine.UI;

public class ProjectileShooter : MonoBehaviour
{
    [Header("Projectile Settings")]
    public Weapon projectilePrefab;
    public Transform shootPoint;
    public Transform player;
    public float shootRadius = 10f;
    public float fireRate = 1f;
    public float spawnOffset = 1.5f;
    private float nextFireTime = 0f;

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

