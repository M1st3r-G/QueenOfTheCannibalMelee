using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class BossController : MonoBehaviour
{
    private static readonly int AnimatorDirection = Animator.StringToHash("Direction");

    //ComponentReferences
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D hitBox;
    [SerializeField] private GameObject hitBoxReference;
    //Params
    [SerializeField] private int maxHealth;
    [SerializeField] private int damage;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float knockBackSpeed;
    [SerializeField] private float knockBackDistance;
    [SerializeField] private float cooldownTime;
    
    private float attackCooldown;
    private float hitCooldown;
    //Temps
    private bool IsFlipped => Mathf.Sign(transform.localScale.x) == -1f;
    private int currentHealth;
    private bool actionActive;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        hitBox = GetComponent<CapsuleCollider2D>();
        currentHealth = maxHealth;
        actionActive = false;
        UpdateCooldown();
    }
    
    private void UpdateCooldown()
    {
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name.EndsWith("Attack")) attackCooldown = clip.length;
            else if (clip.name.EndsWith("Hit")) hitCooldown = clip.length;
        }
    }
    
    private void Update()
    {
        if (actionActive) return;

        switch (Random.Range(0, 3))
        {
            case 0:
                StartCoroutine(DashAttackRoutine());
                break;
            case 1:
                StartCoroutine(AttackRoutine());
                break;
            case 2:
                StartCoroutine(Wait());
                break;
        }
    }
    
    /// <summary>
    /// Used By an Animation to Determine which Player gets hit
    /// </summary>
    private void Attack()
    {
        print($"Boss used Attack");

        var fistPosition = hitBoxReference.transform.localPosition;
        if (IsFlipped) fistPosition.x *= -1;
        fistPosition += transform.position;
        var coll = hitBoxReference.GetComponent<CapsuleCollider2D>();

        Collider2D[] attackTargets =
            Physics2D.OverlapCapsuleAll(fistPosition, coll.size, coll.direction,
                (IsFlipped ? -1 : 1) * hitBoxReference.transform.rotation.z);
        
        foreach (Collider2D attackTarget in attackTargets)
        {
            if (attackTarget.gameObject == gameObject) continue;
            attackTarget.GetComponent<PlayerController>()?.TakeDamage(damage,knockBackSpeed, knockBackDistance);
        }
    }

    private IEnumerator HitRoutine()
    {
        actionActive = true;
        anim.Play("BossHit");

        float counter = 0;
        while (counter < hitCooldown)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        actionActive = false;
    }

    private IEnumerator Wait()
    {
        actionActive = true;
        yield return new WaitForSeconds(cooldownTime);
    }
    
    private IEnumerator DashAttackRoutine()
    {
        hitBox.isTrigger = true;
        anim.Play("BossDash");
        bool currentlyFlipped = IsFlipped;
        while (currentlyFlipped ? transform.position.x < 6f : transform.position.x > -6f)
        {
            rb.velocity = dashSpeed * (currentlyFlipped ? Vector2.right : Vector2.left);
            yield return null;
        }
        anim.Play("EmptyIdle");
        rb.velocity = Vector2.zero;
        hitBox.isTrigger = false;
        Flip();
    }

    private IEnumerator AttackRoutine()
    {
        actionActive = true;
        anim.Play("BossAttack");

        float counter = 0;
        while (counter < attackCooldown)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        actionActive = false;
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
    private void SetHealthBar()
    {
        print($"{currentHealth} / {maxHealth}");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        other.gameObject.GetComponent<PlayerController>()?.TakeDamage(damage, 10, 0);
    }

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x = -localScale.x;
        transform.localScale = localScale;
    }
}