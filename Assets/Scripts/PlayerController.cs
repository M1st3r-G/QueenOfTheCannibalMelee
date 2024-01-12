using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //ComponentReferences
    private Rigidbody2D rb;
    private InputAction moveAction;
    [SerializeField] private CapsuleCollider2D fist;
    //Params
    [SerializeField] private float speed;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float lineCooldown;
    //Temps
    private float direction;
    private bool actionActive;
    // Publics

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        rb = GetComponent<Rigidbody2D>();
        fist.enabled = false;

        transform.position =  LineManager.Instance.SetToLine(gameObject, 0);
        moveAction = GetComponent<PlayerInput>().actions.FindAction("Move");
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
        rb.velocity = Vector2.right * (direction * speed);
    }
    
    private IEnumerator LineChangeRoutine(int dir)
    {
        actionActive = true;
        int newLine = Mathf.Clamp(
            LayerMask.LayerToName(gameObject.layer)[^1] - '0' + dir - 1,
            0,
            LineManager.Instance.NumberOfLines - 1);
        print($"currently in line{LayerMask.LayerToName(gameObject.layer)}({LayerMask.LayerToName(gameObject.layer)[^1] - '0' - 1}) with dir={dir} so new line is {newLine}");
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
        if(other.gameObject.CompareTag("Transition")) GameManager.LoadNextScene();
    }
}