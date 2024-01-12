using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    //ComponentReferences
    private Rigidbody2D rb;
    private PlayerController target;
    private CapsuleCollider2D fist;
    //Params
    public int Damage => baseDamage;
    [SerializeField] private int baseDamage;

    [SerializeField] private int maxHealth;
    
    [SerializeField] private float movementSpeed;
    [SerializeField] private float attackDistance;
    
    [SerializeField] private float changeDistance;
    
    private float attackCooldown = 1;
    private float lineCooldown = 1;
    //Temps
    private float direction;
    private bool actionActive;

    private int currentHealth;
    //Publics

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        
        fist = transform.GetChild(0).GetComponent<CapsuleCollider2D>();
        fist.enabled = actionActive = false;
        
        //UpdateCooldowns();

        direction = -1;
    }

    /*private void UpdateCooldowns()
    {
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            switch (clip.name)
            {
                case "PlayerPunch":
                    attackCooldown = clip.length;
                    break;
                case "PlayerLineChange":
                    lineCooldown = clip.length;
                    break;
                default:
                    print($"Did not find {clip.name}");
                    break;
            }
        }
    }*/
    
    /// <summary>
    /// Controls the AI of the Enemy
    /// </summary>
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

        print("Change Line");

        int newLine = Mathf.Clamp(
            LayerMask.LayerToName(gameObject.layer)[^1] - '0' + dir - 1,
            0,
            LineManager.Instance.NumberOfLines - 1);
        
        Vector3 newPos = LineManager.Instance.ChangeLine(gameObject, newLine);
        Vector3 oldPos = transform.position;
        
        // Set Position Smoothly
        float counter = 0;
        print("Hi");
        if (newPos == oldPos) counter = lineCooldown + 1; // break if no change
        print("Setting Positino");
        while (counter < lineCooldown)
        {
            counter += Time.deltaTime;
            print($"{oldPos}|{newPos}");
            transform.position = Vector3.Lerp(oldPos, newPos, counter / lineCooldown);
            yield return null;
        }
        actionActive = false;
    }
    
    private IEnumerator AttackRoutine()
    {
        actionActive = true;
        fist.enabled = true;

        print("Attack!");

        float counter = 0;
        while (counter < attackCooldown)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        fist.enabled = false;
        actionActive = false;
    }
    
    private void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth > 0) return;
        
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    { 
        if(other.gameObject.CompareTag("Transition")) GameManager.LoadNextScene();
        else if (other.gameObject.CompareTag("Enemy")) TakeDamage(other.gameObject.GetComponent<EnemyController>().Damage); 
    }
}
