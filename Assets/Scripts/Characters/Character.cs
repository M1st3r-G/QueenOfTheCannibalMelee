using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public abstract class Character : MonoBehaviour
{
    protected static readonly int AnimatorDirection = Animator.StringToHash("Direction");
    
    //ComponentReferences
    protected Rigidbody2D Rb;
    protected Animator Anim;
    [SerializeField] protected GameObject fistReference;
    //Params
    private int Damage => baseDamage;
    [SerializeField] protected int baseDamage;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected float movementSpeed;
    
    protected string AnimationPath;
    //Params
    protected float AttackCooldown;
    protected float LineCooldown;
    protected float HitCooldown;
    protected float BlockCooldown;
    //Temps
    protected float Direction;
    protected bool ActionActive;
    protected int CurrentHealth;

    protected void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        CurrentHealth = maxHealth;
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
            else if (clip.name.EndsWith("AldiBagIdle")) BlockCooldown = clip.length;
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
            LayerMask.LayerToName(gameObject.layer)[^1] - '0' + dir - 1,
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
    /// Used By an Animation to Determin which Player gets hit
    /// </summary>
    protected void Attack()
    {
        print($"Noticed An Attack by {gameObject.name}");

        var fistPosition = fistReference.transform.localPosition + transform.position;
        var coll = fistReference.GetComponent<CapsuleCollider2D>();
        
        Collider2D[] attackTargets =
            Physics2D.OverlapCapsuleAll(fistPosition, coll.size, coll.direction, fistReference.transform.rotation.z); 
        
        foreach (Collider2D attackTarget in attackTargets)
        {
            if (attackTarget.gameObject == gameObject) continue;
            attackTarget.GetComponent<Character>()?.TakeDamage(Damage);
        }
    }
    
    protected IEnumerator HitRoutine()
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

    /// <summary>
    /// Used By the Characters to TakeDamage
    /// </summary>
    /// <param name="amount">The amount of Damage</param>
    protected abstract void TakeDamage(int amount);
    protected abstract void PlayPunchSound();
    protected abstract void FixedUpdate();
}
