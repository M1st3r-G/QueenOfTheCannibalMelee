using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class BossController : MonoBehaviour
{
    //ComponentReferences
    private Rigidbody2D rb;
    private SpriteRenderer mainSprite;
    private BoxCollider2D hitBox;
    [SerializeField] private GameObject hitBoxReference;
    //Params
    [SerializeField] private int maxHealth;
    [SerializeField] private int damage;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float knockBackSpeed;
    [SerializeField] private float knockBackDistance;
    //Temps
    private bool isLookingRight;
    private int currentHealth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainSprite = GetComponent<SpriteRenderer>();
        hitBox = GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
        StartCoroutine(DashAttack());
    }
    
    /// <summary>
    /// Used By an Animation to Determine which Player gets hit
    /// </summary>
    private void Attack()
    {
        print($"Boss used Attack");

        var fistPosition = hitBoxReference.transform.localPosition + transform.position;
        var coll = hitBoxReference.GetComponent<CapsuleCollider2D>();
        
        Collider2D[] attackTargets =
            Physics2D.OverlapCapsuleAll(fistPosition, coll.size, coll.direction, hitBoxReference.transform.rotation.z); 
        
        foreach (Collider2D attackTarget in attackTargets)
        {
            if (attackTarget.gameObject == gameObject) continue;
            attackTarget.GetComponent<PlayerController>()?.TakeDamage(damage,knockBackSpeed, knockBackDistance);
        }
    }

    private IEnumerator HitRoutine()
    {
        mainSprite.color = Color.white;

        float counter = 0;
        while (counter < 0.5f)
        {
            counter += Time.deltaTime;
            yield return null;
        }
        mainSprite.color = Color.red;
    }

    /// <summary>
    /// Used By the Characters to TakeDamage
    /// </summary>
    /// <param name="amount">The amount of Damage</param>
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        SetHealthBar();
        print($"Boss Took {amount} Damage and is now at {currentHealth} health");

        StartCoroutine(HitRoutine());
        
        if (currentHealth > 0) return;
        Destroy(gameObject);
        print("Player Won");
    }

    private IEnumerator DashAttack()
    {
        hitBox.isTrigger = true;
        while (!isLookingRight ? transform.position.x > -9 : transform.position.x < 9)
        {
            rb.velocity = Vector2.right * (dashSpeed * (isLookingRight ? 1 : -1));
            yield return null;
        }

        rb.velocity = Vector2.zero;
        hitBox.isTrigger = false;
    }
    
    private void SetHealthBar()
    {
        print($"{currentHealth} / {maxHealth}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.gameObject.GetComponent<PlayerController>()?.TakeDamage(damage, 10, 0);
    }
}