using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    protected float currentHealth;
    public float damageOverTime = 25f;
    public GameObject target;
    public Slider healthBar;
    public Animator animator;
    public float followSpeed;




    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        healthBar.value = currentHealth;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // Placeholder for death behavior
        Destroy(gameObject);
    }
}
