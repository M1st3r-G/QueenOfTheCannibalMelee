using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    private static readonly int AnimatorDirection = Animator.StringToHash("Direction");
    private static readonly int AnimatorChangeTrigger = Animator.StringToHash("Change");
    private static readonly int AnimatorAttackTrigger = Animator.StringToHash("Hit");
    
    //ComponentReferences
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerController target;
    [SerializeField] private GameObject fistReference;
    //Params
    public int Damage => baseDamage;
    [SerializeField] private int baseDamage;
    [SerializeField] private int maxHealth;
    [SerializeField] private float movementSpeed;
    
    [SerializeField] private float attackDistance;
    [SerializeField] private float changeDistance;
    
    private float attackCooldown;
    private float lineCooldown;
    //Temps
    private float direction;
    private bool actionActive;

    private int currentHealth;
    //Publics
    
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        anim = GetComponent<Animator>();

        currentHealth = maxHealth;
        
        UpdateCooldowns();

        direction = -1;
    }

    private void UpdateCooldowns()
    {
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            switch (clip.name)
            {
                case "EnemyPunch":
                    attackCooldown = clip.length;
                    break;
                case "EnemyLineChange":
                    lineCooldown = clip.length;
                    break;
                default:
                    print($"Did not find {clip.name}");
                    break;
            }
        }
    }
    
    /// <summary>
    /// Controls the AI of the Enemy
    /// </summary>
    private void Update()
    {
        direction = !actionActive ? Mathf.Sign(target.transform.position.x - transform.position.x) : 0;
        anim.SetFloat(AnimatorDirection, direction);
        if (actionActive) return;

        int playerLine = LayerMask.LayerToName(target.gameObject.layer)[^1] - '0' - 1;
        int enemyLine = LayerMask.LayerToName(gameObject.layer)[^1] - '0' - 1;
        
        if (Mathf.Abs(target.transform.position.x - transform.position.x) < changeDistance && playerLine != enemyLine)
        {
            StartCoroutine(LineChangeRoutine((int) Mathf.Sign(playerLine - enemyLine)));
        }
        else if (Mathf.Abs(target.transform.position.x - transform.position.x) < attackDistance)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector2.right * (direction * movementSpeed);
    }
    
    private IEnumerator LineChangeRoutine(int dir)
    {
        actionActive = true;
        anim.SetTrigger(AnimatorChangeTrigger);
        print("Change Line");

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
        actionActive = false;
    }
    
    private IEnumerator AttackRoutine()
    {
        actionActive = true;
        anim.SetTrigger(AnimatorAttackTrigger);
        
        float counter = 0;
        while (counter < attackCooldown)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        actionActive = false;
    }
    
    private void TakeDamage(int amount)
    {
        currentHealth -= amount;
        print($"{gameObject.name} Took Damage and is now at {currentHealth} health");
        if (currentHealth > 0) return;
        
        Destroy(gameObject);
    }

    private void Attack()
    {
        print($"Noticed An Attack by {gameObject.name}");

        var fistPosition = fistReference.transform.localPosition + transform.position;
        var coll = fistReference.GetComponent<CapsuleCollider2D>();
        
        Collider2D[] targets =
            Physics2D.OverlapCapsuleAll(fistPosition, coll.size, coll.direction, fistReference.transform.rotation.z); 
        
        foreach (Collider2D target in targets)
        {
            if (target.gameObject == gameObject) continue;
            print($"---{target.gameObject.name}");
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    { 
        if(other.gameObject.CompareTag("Transition")) GameManager.LoadNextScene();
    }
}
