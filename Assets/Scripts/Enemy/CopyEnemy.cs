using UnityEngine;
using UnityEngine.UI;  // For UI components

public class CopyEnemy : MonoBehaviour
{
    public GameObject target; // The target for the enemy to follow, usually the player
    private float followRange = 6f; // The range within which the enemy will follow the target
    private float moveSpeed = 1.5f; // Speed at which the enemy will move towards the target
    private float rotationSpeed = 8f; // Speed of rotation to face the target
    private float stopDistance = 2f; // Distance at which the enemy stops moving towards the target

    public float maxHealth = 100f; // Maximum health of the enemy
    public float currentHealth; // Current health of the enemy
    public Slider healthBarPrefab; // UI Slider prefab to display health bar
    private Slider healthBarInstance; // Instance of the health bar for this enemy

    public float damageOverTime = 50f; // Amount of damage to apply per second
    private float damageRange = 3f; // Range at which the enemy starts taking damage from the player

    void Start()
    {
        currentHealth = maxHealth;

        // Instantiate the health bar prefab and set it as a child of this enemy object
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + new Vector3(0, 3f, 0), Quaternion.identity);

            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                healthBarInstance.transform.SetParent(canvas.transform, true);
                healthBarInstance.transform.localPosition = Vector3.zero;
                healthBarInstance.maxValue = maxHealth;
                healthBarInstance.value = currentHealth;
            }
            else
            {
                Debug.LogError("No Canvas found in the scene!");
            }
        }
    }

    void Update()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (distanceToTarget <= followRange)
        {
            if (distanceToTarget > stopDistance)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
            }

            Vector3 lookDirection = (target.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Gradual damage if the enemy is within range of the player
        if (distanceToTarget <= damageRange)
        {
            TakeDamage(damageOverTime * Time.deltaTime);  // Apply damage over time
        }

        // Update the health bar position above the enemy
        if (healthBarInstance != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 3f, 0));
            healthBarInstance.transform.position = screenPos;
            healthBarInstance.value = currentHealth;
        }
    }

    // Call this method to take damage
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        if (healthBarInstance != null)
        {
            healthBarInstance.value = currentHealth;
        }
    }

    private void Die()
    {
        if (healthBarInstance != null)
        {
            if (healthBarInstance.gameObject != null)
            {
                Destroy(healthBarInstance.gameObject);
            }
        }

        Destroy(gameObject);
    }
}
