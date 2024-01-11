using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    //ComponentReferences
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D fist;
    private InputAction moveAction;
    //Param 
    [SerializeField] private float speed;
    private float attackCooldown;
    //Temps
    private float direction;
    private bool canAttack;
    private static readonly int Hit = Animator.StringToHash("Hit");
    //Publics
     
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        UpdateCooldownTime();
        fist = transform.GetChild(0).GetComponent<CapsuleCollider2D>();
        fist.enabled = false;
        
        LineManager.Instance.SetToLine(gameObject, 0);
        moveAction = GetComponent<PlayerInput>().actions.FindAction("Move");
        canAttack = true; 
    }

    private void UpdateCooldownTime()
    {
        
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            switch (clip.name)
            {
                case "Punch":
                    attackCooldown = clip.length;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += ResetPosition;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= ResetPosition;
    }

    private void ResetPosition(Scene s, LoadSceneMode m)
    {
        Vector3 newPos = new Vector3(-3, transform.position.y, transform.position.z);
        transform.position = newPos;
        print($"Reset Position to {newPos}");
    }
    
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        print("Received AttackInput");
        if (!canAttack)
        {
            print("Still Attacking");
            return;
        }
        anim.SetTrigger(Hit);
        canAttack = false;
        fist.enabled = true;
        
        StartCoroutine(SetAttack());
    }

    public void OnChangeLineUp(InputAction.CallbackContext ctx)
    {
        if (!canAttack) return;
        if (!ctx.performed) return;
        print("Received ChangeLineUp Input");
        ChangeLine(1);
    }
    
    public void OnChangeLineDown(InputAction.CallbackContext ctx)
    {
        if (!canAttack) return;
        if (!ctx.performed) return;
        print("Received ChangeLineDown Input");
        ChangeLine(-1);
    }

    private void ChangeLine(int dir) => 
        LineManager.Instance.ChangeLine(gameObject, Mathf.Clamp(
            LineManager.Instance.GetLine(gameObject) + dir, 
            0, 
            LineManager.Instance.NumberOfLines - 1));
    
    
    private void FixedUpdate()
    {
        direction = canAttack ? moveAction.ReadValue<float>() : 0;
        rb.velocity = Vector2.right * (direction * speed);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    { 
        if(other.gameObject.CompareTag("Transition")) GameManager.LoadNextScene();
    }

    private IEnumerator SetAttack()
    {
        float counter = 0;
        while (counter < attackCooldown)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        fist.enabled = false;
        canAttack = true; 
    }
}