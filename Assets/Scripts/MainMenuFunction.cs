using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuFunction : MonoBehaviour
{
    //ComponentReferences
    [SerializeField] private AudioSource effects;
    [SerializeField] private AudioClip[] clips;
    //Params
    //Temps
    //Publics

    public void OnSettingsClick()
    {
        effects.PlayOneShot(clips[1]);
        StartCoroutine(DelaySceneLoad(clips[1].length, 2));
    }

    public void OnPlayClick()
    {
        effects.PlayOneShot(clips[1]);
        StartCoroutine(DelaySceneLoad(clips[1].length, 3));
    }
    
    public void OnQuitClick()
    {
        effects.PlayOneShot(clips[1]);
        StartCoroutine(DelayQuit(clips[1].length));
    }

    public void PlayButtonHover()
    {
        print("playedHoverSound");
        effects.PlayOneShot(clips[0]);
    }

    private static IEnumerator DelaySceneLoad(float delay, int s)
    {
        float counter = 0;
        while (counter < delay)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(s);
    }
    
    private static IEnumerator DelayQuit(float delay)
    {
        float counter = 0;
        while (counter < delay)
        {
            counter += Time.deltaTime;
            yield return null;
        }
        
        Application.Quit();
        print("Quit the Game");
    }
}