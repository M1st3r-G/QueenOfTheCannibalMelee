using System.Collections;
using UnityEngine;

public class RangedEnemyController : EnemyController
{
    //ComponentReferences
    private GameObject cam;
    //Params
    [SerializeField] private GameObject projectile;
    [SerializeField] private float targetPositionOffset;
    [SerializeField] private float rangeAttackDistance;
    [SerializeField] private float runCooldown;
    //Temps
    private float targetPosition;
    private bool runningToSpot;
    private bool canRun = true;
    //Public

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
    }

    protected new void Update()
    {
        targetPosition = cam.transform.position.x + targetPositionOffset;
        if (ActionActive) return;

        int playerLine = LineManager.GetLine(Target.gameObject);
        int enemyLine = LineManager.GetLine(gameObject);
        
        if (Mathf.Abs(Target.transform.position.x - transform.position.x) < changeDistance && playerLine != enemyLine)
        {
            StartCoroutine(LineChangeRoutine((int) Mathf.Sign(playerLine - enemyLine)));
        }
        else if (Mathf.Abs(transform.position.x - targetPosition) > 0.5f  && canRun)
        {
            StartCoroutine(RunToSpot());
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

    private IEnumerator RunToSpot()
    {
        canRun = false;
        ActionActive = runningToSpot = true;
        while (Mathf.Abs(transform.position.x - targetPosition) > 0.5f)
        {
            Direction = Mathf.Sign(targetPosition - transform.position.x);
            yield return null;
        }
        ActionActive = runningToSpot = false;

        StartCoroutine(ResetRun());
    }

    private IEnumerator ResetRun()
    {
        yield return new WaitForSeconds(runCooldown);
        canRun = true;
    }
    
    protected new void FixedUpdate()
    {
        if (ActionActive && !runningToSpot) Direction = 0;
        Anim.SetFloat(AnimatorDirection, Direction);
        Rb.velocity = Vector2.right * (!KnockedBack ? Direction * Stats.MovementSpeed : CurrentKnockBackSpeed);
    }
}