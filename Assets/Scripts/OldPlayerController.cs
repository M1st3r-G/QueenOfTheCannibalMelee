using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class OldPlayerController : MonoBehaviour
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

    private static readonly int Change = Animator.StringToHash("Change");

    private static readonly int Direction = Animator.StringToHash("Direction");
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

    /// <summary>
    /// Used to Update the Cooldown times (the length of the Animations)
    /// </summary>
    /// <exception cref="IndexOutOfRangeException">Raised, when the Name of a animation is not found</exception>
    private void UpdateCooldownTime()
    {
        
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            switch (clip.name)
            {
                case "PlayerPunch":
                    attackCooldown = clip.length;
                    break;
                case "PlayerWalk":
                    break;
                case "PlayerLineChange":
                    break;
                default:
                    throw new IndexOutOfRangeException($"{clip.name}");
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

    /// <summary>
    /// Resets the Player X position to -4 when a new Scene is Loaded  
    /// </summary>
    /// <param name="s">irrelevant</param>
    /// <param name="m">irrelevant</param>
    private void ResetPosition(Scene s, LoadSceneMode m)
    {
        Vector3 newPos = new Vector3(-4, transform.position.y, transform.position.z);
        transform.position = newPos;
        print($"Reset Position to {newPos}");
    }
    
    /// <summary>
    /// Is Called by the PlayerInput Component when the Attack Input is given, to Attack
    /// </summary>
    /// <param name="ctx">The Context of the Triggered Action</param>
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        print("Received AttackInput");
        
        if (!canAttack) // Forbids Multiple Attacks at once
        {
            print("Still Attacking");
            return;
        }
        
        anim.SetTrigger(Hit);
        canAttack = false;
        fist.enabled = true;
        
        StartCoroutine(SetAttack());
    }

    /// <summary>
    /// Is Called by the PlayerInput Component when the LineChangeUp Input is given, to Change the Line
    /// </summary>
    /// <param name="ctx">The Context of the Triggered Action</param>
    public void OnChangeLineUp(InputAction.CallbackContext ctx)
    {
        if (!canAttack) return;
        if (!ctx.performed) return;
        print("Received ChangeLineUp Input");
        ChangeLine(1);
    }
    
    /// <summary>
    /// Is Called by the PlayerInput Component when the LineChangeUp Input is given, to Change the Line
    /// </summary>
    /// <param name="ctx">The Context of the Triggered Action</param>
    public void OnChangeLineDown(InputAction.CallbackContext ctx)
    {
        if (!canAttack) return;
        if (!ctx.performed) return;
        print("Received ChangeLineDown Input");
        anim.SetTrigger(Change);
        ChangeLine(-1);
    }

    /// <summary>
    /// Used internally to Chang the Line
    /// </summary>
    /// <param name="dir">Either 1 or -1 to go up or down</param>
    private void ChangeLine(int dir) => 
        LineManager.Instance.ChangeLine(gameObject, Mathf.Clamp(
            LineManager.GetLine(gameObject) + dir, 
            0, 
            LineManager.Instance.NumberOfLines - 1));
    
    
    private void FixedUpdate()
    {
        direction = canAttack ? moveAction.ReadValue<float>() : 0; // GetInput
        anim.SetFloat(Direction, direction);
        rb.velocity = Vector2.right * (direction * speed);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    { 
        if(other.gameObject.CompareTag("Transition")) GameManager.LoadNextScene();
    }

    /// <summary>
    /// A Coroutine used to disable Attack Settings after the Attack Cooldown is expired
    /// </summary>
    /// <returns>irrelevant, as this is used as a Coroutine</returns>
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