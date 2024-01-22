using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ParallaxBackgroundController : MonoBehaviour
    {
        private CameraController parallaxCamera;
        private readonly List<ParallaxLayerController> parallaxLayers = new();

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
            
                layer.FixOrder(-5 + i);
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
    
        private void OnEnable() => parallaxCamera.OnCameraTranslate += Move;
        private void OnDisable() => parallaxCamera.OnCameraTranslate -= Move;
    }
}