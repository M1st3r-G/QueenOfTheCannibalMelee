using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : Character
{
    
    //ComponentReferences
    private PlayerController target;
    private EnemyHealthbar healthBar;
    //Params
    [SerializeField] private float changeDistance;
    [SerializeField] [Range(0f,1f)] private float baseBlockChance;
    [SerializeField] private float blockChanceIncrease;
    [SerializeField] private float spamCooldown;
    private float attackDistance;
    //Temps
    private float blockChance;
    private bool wantsToBlock;
    private float timeSinceLastAttackCounter;
    //Publics
    
    private new void Awake()
    {
        base.Awake();
        AnimationPath = "Enemy";
        
        CapsuleCollider2D c = fistReference.GetComponent<CapsuleCollider2D>();

        attackDistance = Mathf.Abs(fistReference.transform.localPosition.x) +
                         (c.direction == CapsuleDirection2D.Vertical ? c.size.y : c.size.x) / 2 *
                         Mathf.Sin(fistReference.transform.rotation.z) * 1.1f;
        
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        transform.position = LineManager.Instance.SetToLine(gameObject, Random.Range(0, LineManager.Instance.NumberOfLines));
        
        healthBar = GetComponent<EnemyHealthbar>();
        healthBar.SetMaxAndMin(Stats.MaxHealth, 0);

        blockChance = baseBlockChance;
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerAttack += OnPlayerAttack;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerAttack -= OnPlayerAttack;
    }

    private void OnPlayerAttack()
    {
        print("Enemy is Warned");
        if (Random.Range(0f, 1f) > blockChance)wantsToBlock = true;
        else blockChance += blockChanceIncrease;

        timeSinceLastAttackCounter = 0;
    }


    /// <summary>
    /// Controls the AI of the Enemy
    /// </summary>
    private void Update()
    {
        if (timeSinceLastAttackCounter > spamCooldown)
        {
            blockChance = baseBlockChance;
            print("Reset BlockChance");
        }
        else if (timeSinceLastAttackCounter <= spamCooldown) timeSinceLastAttackCounter += Time.deltaTime;
        
        if (ActionActive) return;

        int playerLine = LayerMask.LayerToName(target.gameObject.layer)[^1] - '0' - 1;
        int enemyLine = LayerMask.LayerToName(gameObject.layer)[^1] - '0' - 1;

        if (wantsToBlock) 
        {
            wantsToBlock = false;
            if (!(Mathf.Abs(target.transform.position.x - transform.position.x) < attackDistance)) return;
            print("Enemy Blocks");
            StartCoroutine(BlockRoutine());
        }
        else if (Mathf.Abs(target.transform.position.x - transform.position.x) < changeDistance && playerLine != enemyLine)
        {
            StartCoroutine(LineChangeRoutine((int) Mathf.Sign(playerLine - enemyLine)));
        }
        else if (Mathf.Abs(target.transform.position.x - transform.position.x) < attackDistance)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    protected override void FixedUpdate()
    {
        Direction = !ActionActive ? Mathf.Sign(target.transform.position.x - transform.position.x) : 0;
        Anim.SetFloat(AnimatorDirection, Direction);
        Rb.velocity = Vector2.right * (!KnockedBack ? Direction * Stats.MovementSpeed : CurrentKnockBackSpeed);
    }
    
    protected override void TakeDamage(int amount, float kSpeed, float kDistance)
    {
        CurrentHealth -= amount;
        healthBar.SetValue(CurrentHealth);
        print($"{gameObject.name} Took Damage and is now at {CurrentHealth} health");
        
        //Cancel Attack
        StopAllCoroutines();
        StartCoroutine(HitRoutine());
        StartCoroutine(KnockBack(kSpeed, kDistance));
        
        if (CurrentHealth > 0) return;
        AudioManager.Instance.PlayAudioEffect(AudioManager.Enemy1Death);
        Destroy(gameObject);
    }

    
    
    protected override void PlayPunchSound() { }
}
