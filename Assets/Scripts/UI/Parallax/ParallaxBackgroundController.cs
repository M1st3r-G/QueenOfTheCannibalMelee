using System.Collections.Generic;
using UI.Parallax;
using UnityEngine;

public class ParallaxBackgroundController : MonoBehaviour
{
    private CameraController parallaxCamera;
    private List<IParallaxLayer> parallaxLayers = new();

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
            IParallaxLayer layer = transform.GetChild(i).GetComponent<IParallaxLayer>();
            
            layer.FixOrder(-(transform.childCount + 2) + i);
            parallaxLayers.Add(layer);
        }
    }

    private void Move(float delta)
    {
        foreach (IParallaxLayer layer in parallaxLayers)
        {
            layer.Move(delta);
        }
    }
}