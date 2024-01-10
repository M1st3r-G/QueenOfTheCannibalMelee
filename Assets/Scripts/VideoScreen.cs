using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoScreen : MonoBehaviour
{
    [SerializeField] private InputAction skip;
    [SerializeField] private VideoClip video;
    private float waitTime;
    private void OnEnable()
    {
        skip.Enable();
        skip.performed += Skip;
    }

    private void OnDisable()
    {
        skip.performed -= Skip;
        skip.Disable();
    }

    private void Awake()
    {
        waitTime = (float) video.length + 0.1f; 
        StartCoroutine(AfterVideoEnds());
    }

    private IEnumerator AfterVideoEnds()
    {
        float counter = 0;
        while (counter < waitTime)
        {
            counter += Time.deltaTime;
            yield return null;
        }
        Skip(new InputAction.CallbackContext());
    }
    
    private void Skip(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(1);
    }

    
}
