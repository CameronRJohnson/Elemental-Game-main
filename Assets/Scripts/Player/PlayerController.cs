using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Joystick joystick;

    [Header("Health")]
    public Slider healthBar;
    public float maxHealth = 100f;
    private float currentHealth;
    public Animator animator;
    public float damageDistance = 2f;
    public float damageAmount = 5f;

    // Gravity and fall speed
    public float fallMultiplier = 2.5f;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        _rigidbody.useGravity = true;  // Enable gravity for normal 3D movement
    }

    private void Update()
    {
        // Get joystick input
        float horizontalInput = joystick.Horizontal();
        float verticalInput = joystick.Vertical();

        // Calculate movement vector
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
        float joystickMagnitude = new Vector2(horizontalInput, verticalInput).magnitude;

        if (joystickMagnitude > 0.1f)  // Add dead zone to avoid tiny movements
        {
            // Adjust movement for isometric projection
            movement = new Vector3(movement.x , 0f, movement.z).normalized;

            // Move player with speed scaled by joystick magnitude
            MovePlayer(movement * joystickMagnitude);

            // Rotate player to face movement direction
            RotatePlayer(movement);

            // Play running animation
            animator.SetBool("isRunning", true);
        }
        else
        {
            // Stop running animation if not moving
            animator.SetBool("isRunning", false);
        }

        // Adjust gravity for smoother falling behavior
        if (_rigidbody.velocity.y < 0)
        {
            _rigidbody.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        // Check for nearby enemies to take damage
        CheckForEnemyProximity();
    }

    private void MovePlayer(Vector3 movement)
    {
        // Move player using Rigidbody (movement is now adjusted for isometric view)
        Vector3 moveDirection = movement * _moveSpeed;
        _rigidbody.MovePosition(transform.position + moveDirection);
    }

    private void RotatePlayer(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)  // Ignore small inputs
        {
            // Rotate player based on movement direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 100f);
        }
    }

    private void CheckForEnemyProximity()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= damageDistance)
            {
                TakeDamage(damageAmount * Time.deltaTime);
            }
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

        healthBar.value = currentHealth;
    }

    private void Die()
    {
        // Reload the current scene when the player dies
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
