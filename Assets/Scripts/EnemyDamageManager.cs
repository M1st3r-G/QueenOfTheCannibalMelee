using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            TakeDamage(25);
        }
    }

    private void TakeDamage(int damage)
    {
        // Reduce health by the specified damage amount
        currentHealth -= damage;

        // Ensure health doesn't go below 0
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth == 0)
            Destroy(gameObject);

        Debug.Log("enemy took " + damage + " damage. Remaining HP: " + currentHealth);
        //print (currentHealth == 0 + "Enemy Died");

        
    }
}
