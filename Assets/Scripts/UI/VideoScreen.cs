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

    /// <summary>
    /// Loads the Next Scene after the video ends
    /// </summary>
    /// <returns></returns>
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
    
    /// <summary>
    /// Triggered by the Skip Action or as a result of the <see cref="AfterVideoEnds"/> Coroutine has ended, to load the next Scene.
    /// </summary>
    /// <param name="ctx">irrelevant in this Context</param>
    private void Skip(InputAction.CallbackContext ctx)
    {
        SceneController.LoadScene(SceneController.MainMenuIndex);
    }

    
}
