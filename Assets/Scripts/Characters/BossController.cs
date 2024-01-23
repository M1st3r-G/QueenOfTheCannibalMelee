using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class BossController : MonoBehaviour
{
    private static readonly int AnimatorDirection = Animator.StringToHash("Direction");

    //ComponentReferences
    private SpriteRenderer face;
    private SpriteRenderer body;
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D hitBox;
    private PlayerController player;
    [SerializeField] private GameObject hitBoxReference;
    //Params
    [SerializeField] private Color hitColor;
    
    [SerializeField] private int maxHealth;
    [SerializeField] private int damage;
    [SerializeField] private float moveSpeed;
    
    [SerializeField] private float knockBackSpeed;
    [SerializeField] private float knockBackDistance;
    
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashTriggerDistance;
    [SerializeField] private float waitAfterDash;
    
    [SerializeField] private float customAttackDistance;
    [SerializeField] private float leftCorner;
    [SerializeField] private float hitCooldown;
    
    private float attackCooldown;
    private float lineCooldown;
    //Temps
    private bool IsFlipped => Mathf.Sign(transform.localScale.x) == -1f;
    private int currentHealth;
    private bool actionActive;
    private bool canDash;
    private bool isDashing;
    private float direction;
    private float currentKnockBackSpeed;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        hitBox = GetComponent<CapsuleCollider2D>();
        body = GetComponent<SpriteRenderer>();
        face = transform.GetChild(1).GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
        canDash = true;
        UpdateCooldown();
    }

    private void Start()
    {
        transform.position = LineManager.Instance.SetToLine(gameObject, 0);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void UpdateCooldown()
    {
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name.EndsWith("Attack")) attackCooldown = clip.length;
            else if (clip.name.EndsWith("Hit")) hitCooldown = clip.length;
            else if (clip.name.EndsWith("LineChange")) lineCooldown = clip.length;
        }
    }
    
    private void Update()
    {
        if (actionActive) return;
        
        int playerLine = LineManager.GetLine(player.gameObject);
        int enemyLine = LineManager.GetLine(gameObject);

        if (transform.position.x < leftCorner)
        {
            StartCoroutine(DashBackAndRange());
        }
        else if (playerLine != enemyLine)
        {
            print("LineChange");
            StartCoroutine(LineChangeRoutine((int)Mathf.Sign(playerLine - enemyLine)));
        }
        else if (Mathf.Abs(player.transform.position.x - transform.position.x) > dashTriggerDistance && canDash)
        {
            print("Dash");
            StartCoroutine(DashAttackRoutine());
        }
        else if (Mathf.Abs(player.transform.position.x - transform.position.x) < customAttackDistance)
        {            
            print("Attack");
            StartCoroutine(AttackRoutine());
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;
        direction = !actionActive ? Mathf.Sign(player.transform.position.x - transform.position.x) : 0;
        anim.SetFloat(AnimatorDirection, direction);
        rb.velocity = Vector2.right * (currentKnockBackSpeed > 0 ? currentKnockBackSpeed : direction * moveSpeed);
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
        face.color = body.color = hitColor;
        yield return new WaitForSeconds(hitCooldown);
        face.color = body.color = Color.white;
    }

    private IEnumerator LineChangeRoutine(int dir)
    {
        actionActive = true;
        
        anim.Play("BossLineChange");
        int newLine = Mathf.Clamp(
            LayerMask.LayerToName(gameObject.layer)[^1] - '1' + dir,
            0,
            LineManager.Instance.NumberOfLines - 1);
        Vector3 newPos = LineManager.Instance.ChangeLine(gameObject, newLine);
        Vector3 oldPos = transform.position;
        
        // Set Position Smoothly
        float counter = 0;
        if (newPos == oldPos) counter = lineCooldown + 1; // break if no change
        while (counter < lineCooldown)
        {
            counter += Time.deltaTime;
            transform.position = Vector3.Lerp(oldPos, newPos, counter / lineCooldown);
            yield return null;
        }
        
        actionActive = false;
    }
    /// <summary>
    /// Also Flips
    /// </summary>
    /// <returns></returns>
    private IEnumerator DashAttackRoutine()
    {
        actionActive = true;
        isDashing = true;
        canDash = false;
        hitBox.isTrigger = true;
        anim.Play("BossDash");
        bool currentlyFlipped = IsFlipped;
        float border = currentlyFlipped
            ? Mathf.Min(6f, transform.position.x + dashDistance)
            : Mathf.Max(-6f, transform.position.x - dashDistance);
        
        while (currentlyFlipped ? transform.position.x < border : transform.position.x > border)
        {
            rb.velocity = dashSpeed * (currentlyFlipped ? Vector2.right : Vector2.left);
            yield return null;
        }
        anim.Play("EmptyIdle");
        rb.velocity = Vector2.zero;
        hitBox.isTrigger = false;
        isDashing = false;
        StartCoroutine(RegenerateDash());
        if(player.transform.position.x < transform.position.x && IsFlipped)
            StartCoroutine(WaitAndFlip());
        else if (player.transform.position.x > transform.position.x && !IsFlipped)
            StartCoroutine(WaitAndFlip());
        else actionActive = false;
    }

    private IEnumerator DashBackAndRange()
    {
        print("Dashes Back");
        actionActive = true;
        isDashing = true;
        hitBox.isTrigger = true;
        anim.Play("BossDash");
        
        bool currentlyFlipped = IsFlipped;
        while (currentlyFlipped ? transform.position.x > -6f: transform.position.x < 6f)
        {
            rb.velocity = dashSpeed * (currentlyFlipped ? Vector2.left: Vector2.right);
            yield return null;
        }
       
        anim.Play("EmptyIdle");
        rb.velocity = Vector2.zero;
        hitBox.isTrigger = false;
        isDashing = false;
        //StartCoroutine(RangedAttack());
        actionActive = false;
    }
    
    private IEnumerator RegenerateDash()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    private IEnumerator WaitAndFlip()
    {
        yield return new WaitForSeconds(waitAfterDash);
        Vector3 localScale = transform.localScale;
        localScale.x = -localScale.x;
        transform.localScale = localScale;
        actionActive = false;
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
    /// <param name="kSpeed"></param>
    /// <param name="kDistance"></param>
    public void TakeDamage(int amount, float kSpeed, float kDistance)
    {
        currentHealth -= amount;
        SetHealthBar();
        print($"Boss Took {amount} Damage and is now at {currentHealth} health");

        StartCoroutine(HitRoutine());
        StartCoroutine(KnockBack(kSpeed, kDistance));
        
        if (currentHealth > 0) return;
        Destroy(gameObject);
        print("Player Won");
    }
    
    private IEnumerator KnockBack(float speed, float distance)
    {
        float counter = 0;
        currentKnockBackSpeed = speed;
        while (counter < distance / speed)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        currentKnockBackSpeed = 0;
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