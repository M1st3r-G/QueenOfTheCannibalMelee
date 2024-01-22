using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace UI
{
    public class VideoScreen : MonoBehaviour
    {
        [SerializeField] private InputAction skip;
        [SerializeField] private VideoClip video;
        private float waitTime;
    
        private void Awake() => StartCoroutine(AfterVideoEnds());
    
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
    
        private IEnumerator AfterVideoEnds()
        {
            yield return new WaitForSeconds((float) video.length + 0.1f);
            Skip(new InputAction.CallbackContext());
        }
    
        private static void Skip(InputAction.CallbackContext ctx) => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
