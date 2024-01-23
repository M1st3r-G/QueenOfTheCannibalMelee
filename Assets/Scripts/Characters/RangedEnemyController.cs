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

        int playerLine = LineManager.GetLine(Target.gameObject);
        int enemyLine = LineManager.GetLine(gameObject);

        
        
        if (Mathf.Abs(Target.transform.position.x - transform.position.x) < changeDistance && playerLine != enemyLine)
        {
            StartCoroutine(LineChangeRoutine((int) Mathf.Sign(playerLine - enemyLine)));
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
        LineManager.Instance.SetToLine(tmp.gameObject, LineManager.GetLine(gameObject));
    }
    
    protected new void FixedUpdate()
    {
        if (!ActionActive) Direction = 0;
        Anim.SetFloat(AnimatorDirection, Direction);
        Rb.velocity = Vector2.right * (!KnockedBack ? Direction * Stats.MovementSpeed : CurrentKnockBackSpeed);
    }
}