using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileController : MonoBehaviour
{
    //ComponentReferences
    //Params
    [SerializeField] private float movementSpeed;
    //Temps
    private int damage;
    private float kSpeed;
    private float kDistance;
    //Public
     
    private void Awake() => GetComponent<Rigidbody2D>().velocity = Vector2.left * movementSpeed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.gameObject.GetComponent<PlayerController>().TakeDamage(damage, kSpeed, kDistance);
    }

    public void SetParams(int pDamage, float pKSpeed, float pKDistance)
    {
        damage = pDamage;
        kSpeed = pKSpeed;
        kDistance = pKDistance;
    }
}