using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    //ComponentReferences
    private CanvasGroup cg;
    //Params
    [SerializeField] private float fadeInDuration;
    //Temps
    //Publics
     
    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        PlayerController.OnGameOver += FadeIn;
    }

    private void OnDisable()
    {
        PlayerController.OnGameOver -= FadeIn;
    }

    private void FadeIn()
    {
        print("GameOver");
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        cg.interactable = true;
        cg.blocksRaycasts = true;
        float counter = 0f;
        while (counter <  fadeInDuration)
        {
            counter += Time.unscaledDeltaTime;
            cg.alpha = counter / fadeInDuration;
            yield return null;
        }
    }

public void ToMainMenu() => SceneController.Instance.CleanLoadMainMenu();
}