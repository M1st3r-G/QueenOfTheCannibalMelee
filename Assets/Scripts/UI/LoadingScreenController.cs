using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LoadingScreenController : MonoBehaviour
{
    [SerializeField] private VideoClip[] loadingScreens;
    private VideoPlayer vp;
    private float waitTime;
    
    
    private void Awake()
    {
        vp = GetComponent<VideoPlayer>();
        VideoClip clip = loadingScreens[Random.Range(0, loadingScreens.Length)];
        vp.clip = clip;
        vp.Play();
        waitTime = (float) clip.length + 0.1f; 
        StartCoroutine(AfterVideoEnds());
    }

    /// <summary>
    /// Loads the Next Scene when the WaitTime (Length of the Video + 0.1) is over. The Next Scene is decided by the GameManager NextLevelIndex Property
    /// </summary>
    /// <returns>irrelevant, as this is a Coroutine</returns>
    private IEnumerator AfterVideoEnds()
    {
        float counter = 0;
        while (counter < waitTime)
        {
            counter += Time.deltaTime;
            yield return null;
        }
        if(SceneController.Instance is null) SceneController.LoadFirstLevel();
        else SceneController.Instance.LoadNextScene();
    }
}