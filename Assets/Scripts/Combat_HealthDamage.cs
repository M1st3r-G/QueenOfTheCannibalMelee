using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(15);
        }
    }

    private void TakeDamage(int damage)
    {
        // Reduce health by the specified damage amount
        currentHealth -= damage;

        // Ensure health doesn't go below 0
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log("Player took " + damage + " damage. Remaining HP: " + currentHealth);

        // You can add additional logic here, such as checking for player death
    }
}
