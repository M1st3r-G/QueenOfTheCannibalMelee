using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : Character
{
    
    //ComponentReferences
    private PlayerController target;
    private EnemyHealthbar healthBar;
    //Params
    [SerializeField] private float changeDistance;
    [SerializeField] private GameObject maskDropPrefab;
    private float attackDistance;
    //Temps
    private Coroutine resetTime;
    //Publics
    public delegate void EnemyDeath();
    public static EnemyDeath OnEnemyDeath;
    
    
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
    }

    /// <summary>
    /// Controls the AI of the Enemy
    /// </summary>
    private void Update()
    {
        if (ActionActive) return;

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

    protected override void FixedUpdate()
    {
        Direction = !ActionActive ? Mathf.Sign(target.transform.position.x - transform.position.x) : 0;
        Anim.SetFloat(AnimatorDirection, Direction);
        Rb.velocity = Vector2.right * (!KnockedBack ? Direction * Stats.MovementSpeed : CurrentKnockBackSpeed);
    }
    
    protected override void OnNoHealth()
    {
        AudioManager.Instance.PlayAudioEffect(AudioManager.Enemy1Death);
        OnEnemyDeath?.Invoke();
        if (Random.Range(0f, 1f) < MaskUIController.Instance.MaskDropRate) Instantiate(maskDropPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    
    protected override void SetHealthBar(int amount) => healthBar.SetValue(CurrentHealth);

    protected override void PlayHitSound(bool blocked) { }
    protected override void PlayPunchSound() { }
}
