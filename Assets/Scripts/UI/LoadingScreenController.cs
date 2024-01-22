using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace UI
{
    public class LoadingScreenController : MonoBehaviour
    {
        [SerializeField] private VideoClip[] loadingScreens;
        private VideoPlayer vp;
        private double waitTime;
        
        private void Awake()
        {
            vp = GetComponent<VideoPlayer>();
            VideoClip clip = loadingScreens[Random.Range(0, loadingScreens.Length)];
            vp.clip = clip;
            vp.Play();
            waitTime = 0.1f + clip.length ; 
            StartCoroutine(AfterVideoEnds());
        }

        /// <summary>
        /// Loads the Next Scene when the WaitTime (Length of the Video + 0.1) is over. The Next Scene is decided by the GameManager NextLevelIndex Property
        /// </summary>
        /// <returns>irrelevant, as this is a Coroutine</returns>
        private IEnumerator AfterVideoEnds()
        {
            yield return new WaitForSeconds((float) waitTime);
            
            if(SceneController.Instance is null) SceneController.LoadFirstLevel();
            else SceneController.Instance.LoadNextScene();
        }
    }
}