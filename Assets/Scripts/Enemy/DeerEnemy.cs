using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DeerEnemy : Enemy  
{
    public List<GameObject> dropPrefabs;
    public float dropChance = 0.3f;
    public float patrolDelay;
    public float sightRange = 10f;
    public ParticleSystem explosionEffect; // Assign the particle system prefab here

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
        SpawnHealthBar();
        yield return HandleSpawnAnimation();
        FaceCenter();
        SetRandomPatrolTarget();
    }

    private void SpawnHealthBar()
    {
        GameObject healthBarsParent = GameObject.Find("Safe Zone/Health Bars");
        GameObject spawnedHealthBar = Instantiate(healthBar.gameObject, healthBarsParent.transform, false);
        healthBar = spawnedHealthBar.GetComponent<Slider>();
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 4f, 0));
        healthBar.transform.position = screenPosition;
    }

    private IEnumerator HandleSpawnAnimation()
    {
        yield return new WaitForSeconds(.5f);
        animator?.SetBool("isSpawning", false);
    }

    private void FaceCenter()
    {
        Vector3 worldCenter = Vector3.zero;
        Vector3 directionToCenter = (worldCenter - transform.position).normalized;
        directionToCenter.y = 0f;
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
            isIdle = Random.value < 0.9f;
            idleDuration = Random.Range(2f, 5f);

            if (isIdle)
            {
                idleTimer = 0f;
                return;
            }
        }

        if (!isIdle)
        {
            animator?.SetBool("isWalking", true);
            MoveToPosition(randomPatrolTarget);

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
        Vector3 worldCenter = Vector3.zero;
        float maxDistanceFromCenter = 16f;

        while (attempt < maxRetries)
        {
            attempt++;
            float angle = Random.Range(0f, Mathf.PI * 2);
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * 10f;
            Vector3 potentialTarget = transform.position + direction;

            if (Physics.Raycast(potentialTarget + Vector3.up * 10f, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                if (Vector3.Distance(worldCenter, hit.point) <= maxDistanceFromCenter)
                {
                    randomPatrolTarget = hit.point;
                    return;
                }
            }
        }

        randomPatrolTarget = transform.position;
    }

    private void MoveToPosition(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
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
