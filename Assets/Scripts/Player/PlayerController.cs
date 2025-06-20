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

    public float fallMultiplier = 2.5f;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        _rigidbody.useGravity = true;
    }

    private void Update()
    {
        float horizontalInput = joystick.Horizontal();
        float verticalInput = joystick.Vertical();

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
        float joystickMagnitude = new Vector2(horizontalInput, verticalInput).magnitude;

        if (joystickMagnitude > 0.1f)
        {
            movement = new Vector3(movement.x , 0f, movement.z).normalized;

            MovePlayer(movement * joystickMagnitude);

            RotatePlayer(movement);

            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (_rigidbody.velocity.y < 0)
        {
            _rigidbody.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        CheckForEnemyProximity();
    }

    private void MovePlayer(Vector3 movement)
    {
        Vector3 moveDirection = movement * _moveSpeed;
        _rigidbody.MovePosition(transform.position + moveDirection);
    }

    private void RotatePlayer(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
