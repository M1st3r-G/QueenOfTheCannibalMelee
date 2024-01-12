using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    //ComponentReferences
    private Rigidbody2D rb;
    private PlayerController target;
    private CapsuleCollider2D fist;
    //Params
    [SerializeField] private float movementSpeed;
    
    [SerializeField] private float attackDistance;
    [SerializeField] private float changeDistance;
    
    [SerializeField] private float attackCooldown;
    [SerializeField] private float lineCooldown;
    //Temps
    private float direction;
    private bool actionActive;
    //Publics

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        
        fist = transform.GetChild(0).GetComponent<CapsuleCollider2D>();
        fist.enabled = actionActive = false;

        direction = -1;
    }

    /// <summary>
    /// Controls the AI of the Enemy
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void Update()
    {
        direction = !actionActive ? Mathf.Sign(target.transform.position.x - transform.position.x) : 0;
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
        fist.enabled = true;
        
        float counter = 0;
        while (counter < attackCooldown)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        fist.enabled = false;
        actionActive = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Damage")) return;
        print($"{name} got Hit!");
        Destroy(gameObject);
    }
}
