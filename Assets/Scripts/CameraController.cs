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
        target = GameObject.FindGameObjectWithTag("Player");
        oldXPosition = transform.position.x;
    }
    
    private void FixedUpdate()
    {
        float currentXPos = transform.position.x;
        float displacement = Mathf.Max(target.transform.position.x - (xTarget + currentXPos), 0);
        rb.velocity = speedFactor * displacement * displacement * Vector2.right;
        
        if (Math.Abs(oldXPosition - currentXPos) < 0.01f) return;
        OnCameraTranslate?.Invoke(oldXPosition - currentXPos);
        oldXPosition = currentXPos;
    }
}