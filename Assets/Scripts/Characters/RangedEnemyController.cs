using UnityEngine;

public class RangedEnemyController : EnemyController
{
    //ComponentReferences
    //Params
    [SerializeField] private GameObject projectile;
    [SerializeField] private float targetPosition;
    [SerializeField] private float attackDistance;
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
        }
        else if (Mathf.Abs(Target.transform.position.x - transform.position.x) < attackDistance)
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
        Debug.LogWarning("Ranged Enemy Shot");
        Instantiate(projectile, transform.position + Vector3.left * 0.1f, Quaternion.identity);
    }
    
    protected new void FixedUpdate()
    {
        if (!ActionActive) Direction = 0;
        Anim.SetFloat(AnimatorDirection, Direction);
        Rb.velocity = Vector2.right * (!KnockedBack ? Direction * Stats.MovementSpeed : CurrentKnockBackSpeed);
    }
}