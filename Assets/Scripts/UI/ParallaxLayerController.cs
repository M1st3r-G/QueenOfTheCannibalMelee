using UnityEngine;

public class ParallaxLayerController : MonoBehaviour
{
    // Param
    [SerializeField] private float parallaxFactor;
    private float loopDistance;
    private float startDistance;
    
    private void Awake()
    {
        Sprite s = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        loopDistance = 2 * s.rect.width / s.pixelsPerUnit;
        startDistance = transform.localPosition.x;
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
        transform.localPosition += (1 - parallaxFactor) * delta * Vector3.left;

        if (startDistance - transform.localPosition.x > loopDistance)
            transform.localPosition += Vector3.right * loopDistance;
    }
}
