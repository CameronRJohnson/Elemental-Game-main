using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DinoEnemy : Enemy  
{
    // Public fields
    public List<GameObject> dropPrefabs;
    public float dropChance = 0.3f;
    public float patrolDelay;
    public float sightRange = 10f;
    public ParticleSystem explosionEffect; // Assign the particle system prefab here


    // Private fields
    private float stopDistance = 2f;
    private Vector3 randomPatrolTarget;
    private float timeSinceLastPatrol;
    private bool isIdle;
    private float idleDuration;
    private float idleTimer;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(InitializeDeer());
    }

        private IEnumerator InitializeDeer()
    {
        // Spawn and set up health bar directly in Start
        SpawnHealthBar();

        // Handle spawn animation
        yield return HandleSpawnAnimation();

        // Make the deer face the center (or a specific point)
        FaceCenter();

        // Set the patrol target
        SetRandomPatrolTarget();
    }



    private void SpawnHealthBar()
    {
        GameObject healthBarsParent = GameObject.Find("Safe Zone/Health Bars");
        GameObject spawnedHealthBar = Instantiate(healthBar.gameObject, healthBarsParent.transform, false);
        healthBar = spawnedHealthBar.GetComponent<Slider>();
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        // Set initial position
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 4f, 0));
        healthBar.transform.position = screenPosition;
    }



    private IEnumerator HandleSpawnAnimation()
    {

            // Wait for the spawning animation to finish
            yield return new WaitForSeconds(.5f);

            // Allow normal behavior
            animator?.SetBool("isSpawning", false);
    }

    private void FaceCenter()
    {
        Vector3 worldCenter = Vector3.zero; // Replace with the actual center point if needed
        Vector3 directionToCenter = (worldCenter - transform.position).normalized;
        directionToCenter.y = 0f; // Keep rotation on the horizontal plane

        // Rotate towards the center
        Quaternion targetRotation = Quaternion.LookRotation(directionToCenter, Vector3.up);
        transform.rotation = targetRotation;
    }

    void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        bool playerInSightRange = distanceToTarget <= sightRange;

        if (playerInSightRange)
        {
            ChasePlayer(distanceToTarget);
        }
        else
        {
            Patrol();
        }

        if (distanceToTarget <= stopDistance)
        {
            TakeDamage(damageOverTime * Time.deltaTime);
        }

        healthBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 4f, 0));
    }

    private void ChasePlayer(float distanceToTarget)
    {
        if (distanceToTarget > stopDistance)
        {
            animator?.SetBool("isWalking", true);
            MoveToPosition(target.transform.position);
        }
        else
        {
            animator?.SetBool("isWalking", false);
        }
    }

    private void Patrol()
    {
        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDuration)
            {
                isIdle = false;
                SetRandomPatrolTarget();
            }
            animator?.SetBool("isWalking", false);
            return;
        }

        timeSinceLastPatrol += Time.deltaTime;
        if (timeSinceLastPatrol >= patrolDelay)
        {
            timeSinceLastPatrol = 0f;

            // Increase the likelihood of the deer idling
            isIdle = Random.value < 0.9f; // Increase from 0.3 to 0.7
            idleDuration = Random.Range(2f, 5f); // Increase idle duration range

            if (isIdle)
            {
                idleTimer = 0f;
                return; // Immediately return if the deer is idling
            }
        }

        if (!isIdle)
        {
            animator?.SetBool("isWalking", true);
            MoveToPosition(randomPatrolTarget);

            // Stop patrolling if near the target
            if (Vector3.Distance(transform.position, randomPatrolTarget) <= 1f)
            {
                isIdle = true;
                idleTimer = 0f;
            }
        }
    }

    private void SetRandomPatrolTarget()
    {
        int maxRetries = 10;
        int attempt = 0;

        Vector3 worldCenter = Vector3.zero; // Set this to the actual center of your world
        float maxDistanceFromCenter = 16f; // Adjust as needed for your game world size

        while (attempt < maxRetries)
        {
            attempt++;
            float angle = Random.Range(0f, Mathf.PI * 2);
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * 10f;
            Vector3 potentialTarget = transform.position + direction;

            if (Physics.Raycast(potentialTarget + Vector3.up * 10f, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                // Check if the potential target is within the desired distance from the world center
                if (Vector3.Distance(worldCenter, hit.point) <= maxDistanceFromCenter)
                {
                    randomPatrolTarget = hit.point;
                    return;
                }
            }
        }

        // Default to the current position if no valid target was found
        randomPatrolTarget = transform.position;
    }

    private void MoveToPosition(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Move towards the target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Rotate towards the target
        RotateTowards(targetPosition);
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
    }

    protected override void Die()
    {
        DropItem();

        // Play the explosion effect
        if (explosionEffect != null)
        {
            ParticleSystem explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            explosion.Play();
            Destroy(explosion.gameObject, explosion.main.duration); // Clean up the particle system
        }

        Destroy(healthBar.gameObject);
        base.Die();
    }

    private void DropItem()
    {
        if (dropPrefabs == null || dropPrefabs.Count == 0) return;

        if (Random.value <= dropChance)
        {
            int randomIndex = Random.Range(0, dropPrefabs.Count);
            Instantiate(dropPrefabs[randomIndex], transform.position, Quaternion.identity);
        }
    }
}
