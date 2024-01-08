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
    }
    
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        print("Attack");
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

    private void FixedUpdate()
    {
        rb.velocity = Vector2.right * (direction * speed);
    }
}