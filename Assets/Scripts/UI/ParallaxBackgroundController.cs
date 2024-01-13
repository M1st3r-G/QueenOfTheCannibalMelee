using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackgroundController : MonoBehaviour
{
    private CameraController parallaxCamera;
    List<ParallaxLayerController> parallaxLayers = new();

    private void Awake()
    {
        parallaxCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        parallaxCamera.OnCameraTranslate += Move;
        SetLayers();
    }

    private void SetLayers()
    {
        parallaxLayers.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            ParallaxLayerController layer = transform.GetChild(i).GetComponent<ParallaxLayerController>();
            transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = -5 + i;
            parallaxLayers.Add(layer);
        }
    }

    private void Move(float delta)
    {
        foreach (ParallaxLayerController layer in parallaxLayers)
        {
            layer.Move(delta);
        }
    }
}