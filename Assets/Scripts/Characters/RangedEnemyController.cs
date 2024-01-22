using UnityEngine;

public class RangedEnemyController : EnemyController
{
    //ComponentReferences
    //Params
    [SerializeField] private GameObject projectile;
    [SerializeField] private float targetPosition;
    [SerializeField] private float rangeAttackDistance;
    //Temps
    //Public
    
    protected new void Update()
    {
        if (ActionActive) return;

        int playerLine = LayerMask.LayerToName(Target.gameObject.layer)[^1] - '0' - 1;
        int enemyLine = LayerMask.LayerToName(gameObject.layer)[^1] - '0' - 1;

        
        
        if (Mathf.Abs(Target.transform.position.x - transform.position.x) < changeDistance && playerLine != enemyLine)
        {
            StartCoroutine(LineChangeRoutine((int) Mathf.Sign(playerLine - enemyLine)));
            Debug.LogWarning("LineChange");
        }
        else if (Mathf.Abs(Target.transform.position.x - transform.position.x) < rangeAttackDistance)
        {
            StartCoroutine(AttackRoutine());
        }
        else if (Target.transform.position.x < targetPosition)
        {
            Direction = 1;
        }
    }

    protected new void Attack()
    {
        GameObject tmp =  Instantiate(projectile, fistReference.transform.position, Quaternion.identity);
        tmp.GetComponent<ProjectileController>().SetParams(Stats.Damage, Stats.KnockBackSpeed, Stats.KnockBackDistance);
        tmp.gameObject.layer = gameObject.layer;
    }
    
    protected new void FixedUpdate()
    {
        if (!ActionActive) Direction = 0;
        Anim.SetFloat(AnimatorDirection, Direction);
        Rb.velocity = Vector2.right * (!KnockedBack ? Direction * Stats.MovementSpeed : CurrentKnockBackSpeed);
    }
}