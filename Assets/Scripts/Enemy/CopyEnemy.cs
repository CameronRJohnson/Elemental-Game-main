using UnityEngine;
using UnityEngine.UI;

public class CopyEnemy : MonoBehaviour
{
    public GameObject target;
    private float followRange = 6f;
    private float moveSpeed = 1.5f;
    private float rotationSpeed = 8f;
    private float stopDistance = 2f;

    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthBarPrefab;
    private Slider healthBarInstance;

    public float damageOverTime = 50f;
    private float damageRange = 3f;

    void Start()
    {
        currentHealth = maxHealth;

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

        if (distanceToTarget <= damageRange)
        {
            TakeDamage(damageOverTime * Time.deltaTime);
        }

        if (healthBarInstance != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 3f, 0));
            healthBarInstance.transform.position = screenPos;
            healthBarInstance.value = currentHealth;
        }
    }

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
