using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    //ComponentReferences
    private Rigidbody2D rb;
    private PlayerController target;
    //Params
    [SerializeField] private float movementSpeed;
    //Temps
    //Publics

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rb.velocity = Vector2.left * movementSpeed;
    }
}
