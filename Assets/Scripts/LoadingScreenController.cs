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

    private IEnumerator AfterVideoEnds()
    {
        float counter = 0;
        while (counter < waitTime)
        {
            counter += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(GameManager.Instance.NextLevelIndex);
    }
}