using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileController : MonoBehaviour
{
    //ComponentReferences
    private Rigidbody2D rb;
    //Params
    [SerializeField] private bool destroyAfterHit;
    [SerializeField] private float movementSpeed;
    //Temps
    private int damage;
    private float kSpeed;
    private float kDistance;
    //Public
     
    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.left * movementSpeed;
        Destroy(gameObject, 20f / movementSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        other.gameObject.GetComponent<PlayerController>().TakeDamage(damage, kSpeed, kDistance);
        if (destroyAfterHit) Destroy(gameObject);
    }

    public void SetParams(int pDamage, float pKSpeed, float pKDistance)
    {
        damage = pDamage;
        kSpeed = pKSpeed;
        kDistance = pKDistance;
    }
}