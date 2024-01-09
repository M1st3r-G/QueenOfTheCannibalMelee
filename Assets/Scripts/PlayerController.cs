using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    //ComponentReferences
    private Rigidbody2D rb;
    //Params
    [SerializeField] private float speed;
    //Temps
    private float direction;
    //Publics
     
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        rb = GetComponent<Rigidbody2D>();
        
        LineManager.Instance.SetToLine(gameObject, 0);
    }
    
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        print("Received AttackInput");
    }
    
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            direction = 0f;
            return;
        }
        direction = ctx.ReadValue<float>();
    }

    public void OnChangeLineUp(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        print("Received ChangeLineUp Input");
        ChangeLine(1);
    }
    
    public void OnChangeLineDown(InputAction.CallbackContext ctx)
    {
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
        rb.velocity = Vector2.right * (direction * speed);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    { 
        if(other.gameObject.CompareTag("Transition")) GameManager.LoadNextScene();
    }
}