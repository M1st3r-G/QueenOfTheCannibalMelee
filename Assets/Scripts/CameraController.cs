using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CameraController : MonoBehaviour
{
    //ComponentReferences
    private GameObject target;
    private Rigidbody2D rb;
    //Param 
    [SerializeField] private float speedFactor;
    [SerializeField] private float xTarget;
    //Temps
    //Publics

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player");
    }

    /// <summary>
    /// Follows the Player but keeps X Coordinate in the Bounds
    /// </summary>
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(Mathf.Pow(Mathf.Max(target.transform.position.x - (xTarget + transform.position.x), 0), 2),0) * speedFactor;
    }
}
