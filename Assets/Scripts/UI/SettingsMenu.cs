using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SettingsMenu : PopUpMenu
{
    public const string GeneralVolumeKey = "GeneralVolume";
    public const string EffectVolumeKey = "EffectVolume";
    public const string MusicVolumeKey = "MusicVolume";

    //ComponentReferences
    [SerializeField] private PauseMenuController pauseMenu;
    [SerializeField] private Slider generalVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider effectVolumeSlider;
    private AudioSource musicAudioSource;
    private AudioSource effectAudioSource;
    //Params
    [SerializeField] private float soundCooldown;
    //Temps
    private bool playable;
    private float musicVolume;
    private float effectVolume;
    private float generalVolume;
    //Public
     
    private new void Awake()
    {
        base.Awake();
        playable = true;
        
        
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.75f);
        effectVolume = PlayerPrefs.GetFloat(EffectVolumeKey, 0.75f);
        generalVolume = PlayerPrefs.GetFloat(GeneralVolumeKey, 0.75f);
        
        musicVolumeSlider.SetValueWithoutNotify(musicVolume);
        effectVolumeSlider.SetValueWithoutNotify(effectVolume);
        generalVolumeSlider.SetValueWithoutNotify(generalVolume); 
    }

    private void Start()
    {
        effectAudioSource = SceneController.IsInMainMenu ? GetComponent<AudioSource>() : AudioManager.Instance.GetComponent<AudioSource>();
        musicAudioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
    }

    public void CloseSettings()
    {
        // Save in PlayerPrefs
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        PlayerPrefs.SetFloat(EffectVolumeKey, effectVolume);
        PlayerPrefs.SetFloat(GeneralVolumeKey, generalVolume);
        
        if (pauseMenu is not null)
        {
            JumpTo(false);
            pauseMenu.JumpTo(true);
        }
        else ToggleMenu();
    }
    
    public void OnMusicVolumeChange()
    {
        musicVolume = musicVolumeSlider.value;
        musicAudioSource.volume = musicVolume * generalVolume;
    }

    public void OnGeneralVolumeChange()
    {
        generalVolume = generalVolumeSlider.value;
        musicAudioSource.volume = musicVolume * generalVolume;
        effectAudioSource.volume = effectVolume * generalVolume;
    }
    
    public void OnEffectVolumeChange()
    {
        effectVolume = effectVolumeSlider.value;
        effectAudioSource.volume = effectVolume * generalVolume;
        
        if (!playable) return;
        if (AudioManager.Instance is not null) AudioManager.Instance.PlayAudioEffect(AudioManager.SettingsSound);
        else effectAudioSource.Play();
        StartCoroutine(RefreshCooldown());
    }

    public void WipeSaveData()
    {
        print("WipedSaveData");
        PlayerPrefs.DeleteAll();
    }

    private IEnumerator RefreshCooldown()
    {
        playable = false;
        float counter = 0;
        while(counter < soundCooldown)
        {
            counter += Time.unscaledDeltaTime;
            yield return null;
        }
        playable = true;
    }
}