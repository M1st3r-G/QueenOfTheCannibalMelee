using UnityEngine;

public class EnemyController : Character
{
    
    //ComponentReferences
    private PlayerController target;
    //Params
    [SerializeField] private float attackDistance;
    [SerializeField] private float changeDistance;
    //Temps
    //Publics
    
    private new void Awake()
    {
        base.Awake();

        target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        transform.position = LineManager.Instance.SetToLine(gameObject, Random.Range(0, LineManager.Instance.NumberOfLines));
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

    private new void FixedUpdate()
    {
        Direction = !ActionActive ? Mathf.Sign(target.transform.position.x - transform.position.x) : 0;
        base.FixedUpdate();
    }
    
    protected override void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        print($"{gameObject.name} Took Damage and is now at {CurrentHealth} health");
        
        //Cancel Attack
        StopAllCoroutines();
        ActionActive = false;
        anim.Play("EmptyIdle"); // Hit
        
        if (CurrentHealth > 0) return;
        Destroy(gameObject);
    }
}
