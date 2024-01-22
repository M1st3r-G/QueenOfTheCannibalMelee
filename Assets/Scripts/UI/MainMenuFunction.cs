using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MainMenuFunction : MonoBehaviour
{
    //ComponentReferences
    private AudioSource effects;
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private SettingsMenu settings;
    //Params
    //Temps
    //Public

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
        settings.ToggleMenu();
    }

    /// <summary>
    /// Used by Buttons to start the Game
    /// </summary>
    public void OnPlayClick()
    {
        effects.PlayOneShot(clips[1]);
        StartCoroutine(DelayStart(clips[1].length));
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

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// A Coroutine to Delay the Scene Load of Scene with buildIndex s by delay seconds.
    /// </summary>
    /// <param name="delay">The amount of Seconds to wait</param>
    /// <returns>irrelevant, as this used as a Coroutine</returns>
    private static IEnumerator DelayStart(float delay)
    {
        float counter = 0;
        while (counter < delay)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        SceneController.LoadFirstLoadingScreen();
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