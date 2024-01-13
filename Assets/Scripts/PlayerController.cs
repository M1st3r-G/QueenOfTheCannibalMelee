using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private static readonly int AnimatorDirection = Animator.StringToHash("Direction");
    private static readonly int AnimatorChangeTrigger = Animator.StringToHash("Change");
    private static readonly int AnimatorAttackTrigger = Animator.StringToHash("Hit");
    
    //ComponentReferences
    private Rigidbody2D rb;
    private Animator anim;
    private InputAction moveAction;
    private CapsuleCollider2D fist;
    //Params
    public int Damage => baseDamage;
    [SerializeField] private int baseDamage;
    
    [SerializeField] private int maxHealth;
    [SerializeField] private float speed;
    
    private float attackCooldown;
    private float lineCooldown;
    //Temps
    private float direction;
    private bool actionActive;
    private int currentHealth;


    // Publics

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        fist = transform.GetChild(0).GetComponent<CapsuleCollider2D>();

        currentHealth = maxHealth;
        
        UpdateCooldowns();
        
        transform.position =  LineManager.Instance.SetToLine(gameObject, 0);
        moveAction = GetComponent<PlayerInput>().actions.FindAction("Move");
    }

    private void UpdateCooldowns()
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
        print($"Reset x-Position to {newPos}");
    }
    
    /// <summary>
    /// Is Called by the PlayerInput Component when the Attack Input is given, to Attack
    /// </summary>
    /// <param name="ctx">The Context of the Triggered Action</param>
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        print("Received AttackInput");

        if (!actionActive) StartCoroutine(AttackRoutine());
    }
    
    /// <summary>
    /// Is Called by the PlayerInput Component when the LineChangeUp Input is given, to Change the Line
    /// </summary>
    /// <param name="ctx">The Context of the Triggered Action</param>
    public void OnChangeLineUp(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        print("Received ChangeLineUp Input");

        if (!actionActive) StartCoroutine(LineChangeRoutine(1));
    }
    
    /// <summary>
    /// Is Called by the PlayerInput Component when the LineChangeDown Input is given, to Change the Line
    /// </summary>
    /// <param name="ctx">The Context of the Triggered Action</param>
    public void OnChangeLineDown(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        print("Received ChangeLineDown Input");

        if (!actionActive) StartCoroutine(LineChangeRoutine(-1));
    }

    private void FixedUpdate()
    {
        direction = !actionActive ? moveAction.ReadValue<float>() : 0;
        anim.SetFloat(AnimatorDirection, direction);
        rb.velocity = Vector2.right * (direction * speed);
    }
    
    private IEnumerator LineChangeRoutine(int dir)
    {
        actionActive = true;
        anim.SetTrigger(AnimatorChangeTrigger);
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
        anim.SetTrigger(AnimatorAttackTrigger);
        float counter = 0;
        while (counter < attackCooldown)
        {
            counter += Time.deltaTime;
            yield return null;
        }
        actionActive = false;
    }
    
    private void EnableFist(int active)
    {
        fist.enabled = active == 1;
    }

    private void TakeDamage(int amount)
    {
        currentHealth -= amount;
        print($"Player Took Damage and is now at {currentHealth} health");
        if (currentHealth > 0) return;
        
        print("GameOver");
        Time.timeScale = 0;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    { 
        if(other.gameObject.CompareTag("Transition")) GameManager.LoadNextScene();
        else if (other.gameObject.CompareTag("Enemy")) TakeDamage(other.gameObject.GetComponent<EnemyController>().Damage); 
    }
}