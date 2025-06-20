using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Weapon weaponData;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (weaponData != null && rb != null)
        {
            rb.velocity = transform.forward * weaponData.speed;
            StartCoroutine(DestroyAfterDelay());
        }
        else
        {
            Debug.LogWarning("Projectile: Missing weaponData or Rigidbody.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (weaponData == null)
            return;

        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(weaponData.damage);
            }
        }

        Destroy(gameObject);
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(weaponData.destroyDelay);
        Destroy(gameObject);
    }
}
