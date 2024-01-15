using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MainMenuFunction : MonoBehaviour
{
    //ComponentReferences
    private AudioSource effects;
    [SerializeField] private AudioClip[] clips;
    //Params
    //Temps
    //Publics

    private void Awake()
    {
        effects = GetComponent<AudioSource>();
        
        float effectVolume = PlayerPrefs.GetFloat(SettingsMenu.EffectVolumeKey, 0.75f);
        float generalVolume = PlayerPrefs.GetFloat(SettingsMenu.GeneralVolumeKey, 0.75f);
        float musicVolume = PlayerPrefs.GetFloat(SettingsMenu.MusicVolumeKey, 0.75f);

        effects.volume = effectVolume * generalVolume;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().volume = musicVolume * generalVolume;
    }

    /// <summary>
    /// Used by Buttons to Load the Settings
    /// </summary>
    public void OnSettingsClick()
    {
        effects.PlayOneShot(clips[1]);
        StartCoroutine(DelaySceneLoad(clips[1].length, 2));
    }

    /// <summary>
    /// Used by Buttons to start the Game
    /// </summary>
    public void OnPlayClick()
    {
        effects.PlayOneShot(clips[1]);
        StartCoroutine(DelaySceneLoad(clips[1].length, 3));
    }
    
    /// <summary>
    /// Used by the Buttons to Quit the Game
    /// </summary>
    public void OnQuitClick()
    {
        effects.PlayOneShot(clips[1]);
        StartCoroutine(DelayQuit(clips[1].length));
    }

    /// <summary>
    /// Used By an EventTrigger to Add Sounds while Hovering over the Buttons
    /// </summary>
    public void PlayButtonHover()
    {
        print("playedHoverSound");
        effects.PlayOneShot(clips[0]);
    }

    /// <summary>
    /// A Coroutine to Delay the Scene Load of Scene with buildIndex s by delay seconds.
    /// </summary>
    /// <param name="delay">The amount of Seconds to wait</param>
    /// <param name="s">The BuildIndex of the Scene to Load</param>
    /// <returns>irrelevant, as this used as a Corountine</returns>
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
    
    /// <summary>
    /// Delays the Exit of the Game by delay Seconds
    /// </summary>
    /// <param name="delay">The Number of Seconds to delay by</param>
    /// <returns>irrelevant, as this is used as a Coroutine</returns>
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