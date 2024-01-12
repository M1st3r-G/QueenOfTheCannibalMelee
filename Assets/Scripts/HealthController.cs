using UnityEngine;

public class HealthController : MonoBehaviour
{
    //ComponentReferences
    [SerializeField] private int maxHealth;
    //Params
    //Temps
    private int currentHealth;
    //Publics

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collision is with the player
        if (collision.gameObject.CompareTag("Damage"))
        {
            TakeDamage(25);
        }
    }

    private void TakeDamage(int damage)
    {
        // Reduce health by the specified damage amount
        currentHealth -= damage;
        
        print($"{gameObject.name} took {damage} damage. Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
            Destroy(gameObject);
    }
}