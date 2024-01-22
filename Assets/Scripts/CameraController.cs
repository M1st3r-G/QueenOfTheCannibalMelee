using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CameraController : MonoBehaviour
{
    //ComponentReferences
    private GameObject target;
    private Rigidbody2D rb;
    //Param 
    [SerializeField] protected float speedFactor;
    [SerializeField] protected float xTarget;
    //Temps
    private float oldXPosition;
    //Publics
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    public ParallaxCameraDelegate OnCameraTranslate;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        oldXPosition = transform.position.x;
        target = GameObject.FindGameObjectWithTag("Player");
    }

    /// <summary>
    /// Follows the Player but keeps X Coordinate in the Bounds
    /// </summary>
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(Mathf.Pow(Mathf.Max(target.transform.position.x - (xTarget + transform.position.x), 0), 2),0) * speedFactor;
        if (Math.Abs(oldXPosition - transform.position.x) < 0.01f) return;
        OnCameraTranslate?.Invoke(oldXPosition-transform.position.x);
        oldXPosition = transform.position.x;
    }
}