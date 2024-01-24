using System;
using UnityEngine;

public class RangedEnemyController : EnemyController
{
    //ComponentReferences
    private GameObject cam;
    //Params
    [SerializeField] private GameObject projectile;
    [SerializeField] private float targetPositionOffset;
    [SerializeField] private float rangeAttackDistance;
    //Temps
    //Public

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
    }

    protected new void Update()
    {
        if (ActionActive) return;

        int playerLine = LineManager.GetLine(Target.gameObject);
        int enemyLine = LineManager.GetLine(gameObject);
        float targetPosition = cam.transform.position.x + targetPositionOffset;
        
        
        if (Mathf.Abs(Target.transform.position.x - transform.position.x) < changeDistance && playerLine != enemyLine)
        {
            StartCoroutine(LineChangeRoutine((int) Mathf.Sign(playerLine - enemyLine)));
        }
        else if (Math.Abs(Target.transform.position.x - targetPosition) < 0.5f)
        {
            Direction = Mathf.Sign(targetPosition - Target.transform.position.x);
        }
        else if (Mathf.Abs(Target.transform.position.x - transform.position.x) < rangeAttackDistance)
        {
            StartCoroutine(AttackRoutine());
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