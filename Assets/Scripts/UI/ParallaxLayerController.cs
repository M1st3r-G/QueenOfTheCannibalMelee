using System;
using UnityEngine;

public class ParallaxLayerController : MonoBehaviour
{
    // Param
    [SerializeField] private float parallaxFactor;
    private float loopDistance;
    private float currentMove;
    
    private void Awake()
    {
        Sprite s = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        loopDistance = 2 * s.rect.width / s.pixelsPerUnit;
        print($"{gameObject.name} loops with {loopDistance} units");
        currentMove = -2 *  transform.localPosition.x;
    }

    public void FixOrder(int layer)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = layer;
        }
    }
    
    public void Move(float delta)
    {
        transform.localPosition -= Vector3.right * (delta * parallaxFactor);
        currentMove -= delta*(1-parallaxFactor);
        
        print($"{gameObject.name}: {currentMove}| > |{loopDistance}");
        if (currentMove < loopDistance) return;
        print($"{gameObject.name} Looped!");
        currentMove -= loopDistance;
        transform.localPosition += Vector3.right * loopDistance;

    }
}
