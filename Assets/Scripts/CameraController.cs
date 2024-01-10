using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //ComponentReferences
    private GameObject target;
    //Param 
    [SerializeField] private float leftBound;
    [SerializeField] private float rightBound;
    [SerializeField] private float speedFactor;

    [SerializeField] private float xTarget;
    //Temps
    //Publics

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        float velocity = Mathf.Max(target.transform.position.x - (xTarget + transform.position.x), 0);
        Vector3 nextPos = transform.position + velocity *velocity * speedFactor * Time.deltaTime * Vector3.right;
        if (nextPos.x < leftBound) nextPos.x = leftBound;
        if (nextPos.x > rightBound) nextPos.x = rightBound;
        transform.position = nextPos; 
    }
}
