using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public abstract class Character : MonoBehaviour
{
    private static readonly int AnimatorDirection = Animator.StringToHash("Direction");
    private static readonly int AnimatorChangeTrigger = Animator.StringToHash("Change");
    private static readonly int AnimatorAttackTrigger = Animator.StringToHash("Hit");

    //ComponentReferences
    private Rigidbody2D rb;
    protected Animator anim;
    [SerializeField] private GameObject fistReference;
    //Params
    public int Damage => baseDamage;
    [SerializeField] protected int baseDamage;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected float movementSpeed;
    //Params
    private float attackCooldown;
    private float lineCooldown;
    //Temps
    protected float Direction;
    protected bool ActionActive;
    protected int CurrentHealth;

    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        CurrentHealth = maxHealth;
        UpdateCooldown();
    }
    
    /// <summary>
    /// Used Internally to Update AnimationLengths
    /// </summary>
    private void UpdateCooldown()
    {
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name.EndsWith("Punch"))attackCooldown = clip.length;
            else if (clip.name.EndsWith("LineChange")) lineCooldown = clip.length;
            else print($"Did not find {clip.name}");
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
        anim.SetTrigger(AnimatorChangeTrigger);
        int newLine = Mathf.Clamp(
            LayerMask.LayerToName(gameObject.layer)[^1] - '0' + dir - 1,
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
        ActionActive = false;
    }
    
    /// <summary>
    /// Used by The Character to Enable the Attack Animation and block other Actions
    /// </summary>
    /// <returns>irrelevant, as this is a Coroutine</returns>
    protected IEnumerator AttackRoutine()
    {
        ActionActive = true;
        anim.SetTrigger(AnimatorAttackTrigger);
        
        float counter = 0;
        while (counter < attackCooldown)
        {
            counter += Time.deltaTime;
            yield return null;
        }
        
        ActionActive = false;
    }

    /// <summary>
    /// Sets the Speed and Animator Speed
    /// </summary>
    protected void FixedUpdate()
    {
        anim.SetFloat(AnimatorDirection, Direction);
        rb.velocity = Vector2.right * (Direction * movementSpeed);
    }
    
    /// <summary>
    /// Used By an Animation to Determin which Player gets hit
    /// </summary>
    protected void Attack()
    {
        print($"Noticed An Attack by {gameObject.name}");

        var fistPosition = fistReference.transform.localPosition + transform.position;
        var coll = fistReference.GetComponent<CapsuleCollider2D>();
        
        Collider2D[] targets =
            Physics2D.OverlapCapsuleAll(fistPosition, coll.size, coll.direction, fistReference.transform.rotation.z); 
        
        foreach (Collider2D target in targets)
        {
            if (target.gameObject == gameObject) continue;
            target.GetComponent<Character>().TakeDamage(Damage);
        }
    }

    /// <summary>
    /// Used By the Characters to TakeDamage
    /// </summary>
    /// <param name="amount">The amount of Damage</param>
    protected abstract void TakeDamage(int amount);
}