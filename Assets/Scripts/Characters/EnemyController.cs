using System.Collections;
using UnityEngine;

public class EnemyController : Character
{
    
    //ComponentReferences
    private PlayerController target;
    private EnemyHealthbar healthbar;
    //Params
    [SerializeField] private float changeDistance;
    private float attackDistance;
    //Temps
    //Publics
    
    private new void Awake()
    {
        base.Awake();

        CapsuleCollider2D c = fistReference.GetComponent<CapsuleCollider2D>();

        attackDistance = Mathf.Abs(fistReference.transform.localPosition.x) +
                         (c.direction == CapsuleDirection2D.Vertical ? c.size.y : c.size.x) / 2 *
                         Mathf.Sin(fistReference.transform.rotation.z) * 1.1f;
        
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        transform.position = LineManager.Instance.SetToLine(gameObject, Random.Range(0, LineManager.Instance.NumberOfLines));
        healthbar = GetComponent<EnemyHealthbar>();
        healthbar.SetMaxAndMin(maxHealth, 0);
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
        healthbar.SetValue(CurrentHealth);
        print($"{gameObject.name} Took Damage and is now at {CurrentHealth} health");
        
        //Cancel Attack
        StopAllCoroutines();
        StartCoroutine(Hit());
        
        
        if (CurrentHealth > 0) return;
        Destroy(gameObject);
    }

    private IEnumerator Hit()
    {
        ActionActive = true;
        anim.Play("EnemyHit");

        float counter = 0;
        while (counter < HitCooldown)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        ActionActive = false;
    }
}
