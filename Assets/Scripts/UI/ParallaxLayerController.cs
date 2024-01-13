using UnityEngine;

public class ParallaxLayerController : MonoBehaviour
{
    // Param
    [SerializeField] private float parallaxFactor;

    public void Move(float delta)
    {
        transform.localPosition -= Vector3.right * (delta * parallaxFactor);
    }
}
