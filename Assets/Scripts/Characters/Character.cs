using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(StatsController))]
public abstract class Character : MonoBehaviour
{
    protected static readonly int AnimatorDirection = Animator.StringToHash("Direction");
    
    //ComponentReferences
    protected Rigidbody2D Rb;
    protected Animator Anim;
    [SerializeField] protected GameObject fistReference;
    //Params
    protected StatsController Stats;
    private bool IsFlipped => Mathf.Abs(Mathf.Sign(transform.localScale.x) - (-1)) < Mathf.Epsilon;

    protected string AnimationPath;
    protected float AttackCooldown;
    protected float LineCooldown;
    protected float HitCooldown;
    //Temps
    protected float CurrentKnockBackSpeed;
    protected float Direction;
    protected int CurrentHealth;
    private bool blocking;
    protected bool ActionActive;
    protected bool KnockedBack;

    protected void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        Stats = GetComponent<StatsController>();
        CurrentHealth = Stats.MaxHealth;
        UpdateCooldown();
    }
    
    /// <summary>
    /// Used Internally to Update AnimationLengths
    /// </summary>
    private void UpdateCooldown()
    {
        foreach (AnimationClip clip in Anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name.EndsWith("Attack"))AttackCooldown = clip.length;
            else if (clip.name.EndsWith("LineChange")) LineCooldown = clip.length;
            else if (clip.name.EndsWith("Hit")) HitCooldown = clip.length;
        }
    }
    
    /// <summary>
    /// Used by the Characters to Change their Line
    /// </summary>
    /// <param name="dir">1: Up, -1: Down</param>
    /// <returns>irrelevant, as this is a Coroutine</returns>
    protected IEnumerator LineChangeRoutine(int dir)
    {
        ActionActive = true;
        Anim.Play(AnimationPath + "LineChange");
        int newLine = Mathf.Clamp(
            LineManager.GetLine(gameObject) + dir,
            0,
            LineManager.Instance.NumberOfLines - 1);
        Vector3 newPos = LineManager.Instance.ChangeLine(gameObject, newLine);
        Vector3 oldPos = transform.position;
        
        // Set Position Smoothly
        float counter = 0;
        if (newPos == oldPos) counter = LineCooldown + 1; // break if no change
        while (counter < LineCooldown)
        {
            counter += Time.deltaTime;
            transform.position = Vector3.Lerp(oldPos, newPos, counter / LineCooldown);
            yield return null;
        }
        ActionActive = false;
    }
    
    /// <summary>
    /// Used by The Character to Enable the Attack Animation and block other Actions
    /// </summary>
    /// <returns>irrelevant, as this is a Coroutine</returns>
    protected IEnumerator AttackRoutine()
    {
        ActionActive = true;
        Anim.Play(AnimationPath + "Attack");
        
        PlayPunchSound();
        
        float counter = 0;
        while (counter < AttackCooldown)
        {
            counter += Time.deltaTime;
            yield return null;
        }
        
        ActionActive = false;
    }
    
    /// <summary>
    /// Used By an Animation to Determine which Player gets hit
    /// </summary>
    protected void Attack()
    {
        print($"Noticed An Attack by {gameObject.name}");

        var fistPosition = fistReference.transform.localPosition;
        if (IsFlipped) fistPosition.x *= -1;
        fistPosition += transform.position;
        var coll = fistReference.GetComponent<CapsuleCollider2D>();

        Collider2D[] attackTargets =
            Physics2D.OverlapCapsuleAll(fistPosition, coll.size, coll.direction,
                (IsFlipped ? -1 : 1) * fistReference.transform.rotation.z);
        
        foreach (Collider2D attackTarget in attackTargets)
        {
            if (attackTarget.gameObject == gameObject) continue;
            attackTarget.GetComponent<Character>()
                ?.TakeDamage(Stats.Damage, Stats.KnockBackSpeed, Stats.KnockBackDistance);
            attackTarget.GetComponent<BossController>()
                ?.TakeDamage(Stats.Damage, Stats.KnockBackSpeed, Stats.KnockBackDistance / 2f);
        }
    }

    private IEnumerator HitRoutine()
    {
        ActionActive = true;
        Anim.Play(AnimationPath + "Hit");

        float counter = 0;
        while (counter < HitCooldown)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        ActionActive = false;
    }

    private IEnumerator KnockBack(float speed, float distance)
    {
        float counter = 0;
        KnockedBack = true;
        CurrentKnockBackSpeed = speed;
        while (counter < distance / speed)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        KnockedBack = false;
    }

    protected void StartBlock()
    {
        ActionActive = true;
        Anim.Play(AnimationPath + "Block");
        blocking = true;
    }
    
    protected void BreakBlock()
    {
        if (!blocking) return;
        Anim.Play("EmptyIdle");
        StopAllCoroutines();
        blocking = false;
        ActionActive = false;
    }
    
    /// <summary>
    /// Used By the Characters to TakeDamage
    /// </summary>
    /// <param name="amount">The amount of Damage</param>
    /// <param name="kSpeed">The KnockBack Speed</param>
    /// <param name="kDistance">The KnockBack Distance</param>
    public void TakeDamage(int amount, float kSpeed, float kDistance)
    {
        if (blocking) amount = (int)((1 - Stats.DamageBlock) * amount);
        CurrentHealth -= amount;
        SetHealthBar(CurrentHealth);
        PlayHitSound(blocking);
        print($"{AnimationPath} Took {amount} Damage and is now at {CurrentHealth} health");

        if (!blocking)
        {
            StopAllCoroutines();
            StartCoroutine(HitRoutine());
            StartCoroutine(KnockBack(kSpeed, kDistance));
        }
        
        if (CurrentHealth > 0) return;
        OnNoHealth();
    }

    protected void CheckFlip()
    {
        if (ActionActive) return;
        if ((!(Direction < 0) || IsFlipped) && (!(Direction > 0) || !IsFlipped)) return;
        Vector3 localScale = transform.localScale;
        localScale.x = -localScale.x;
        transform.localScale = localScale;
    }
    
    protected abstract void SetHealthBar(int amount);
    protected abstract void OnNoHealth();
    protected abstract void PlayHitSound(bool blocked);
    protected abstract void PlayPunchSound();
    protected abstract void FixedUpdate();
}
