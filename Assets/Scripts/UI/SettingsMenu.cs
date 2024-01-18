using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SettingsMenu : MonoBehaviour
{
    public const string GeneralVolumeKey = "GeneralVolume";
    public const string EffectVolumeKey = "EffectVolume";
    public const string MusicVolumeKey = "MusicVolume";

    //ComponentReferences
    [SerializeField] private Slider generalVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider effectVolumeSlider;

    private AudioSource effectTest;
    //Params
    [SerializeField] private float soundCooldown;
    //Temps
    private float counter;
    private float musicVolume;
    private float effectVolume;
    private float generalVolume;
    //Publics
     
    private void Awake()
    {
        effectTest = GetComponent<AudioSource>();

        counter = 0;
        
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.75f);
        effectVolume = PlayerPrefs.GetFloat(EffectVolumeKey, 0.75f);
        generalVolume = PlayerPrefs.GetFloat(GeneralVolumeKey, 0.75f);
        
        musicVolumeSlider.value = musicVolume;
        effectVolumeSlider.value = effectVolume;
        generalVolumeSlider.value = generalVolume;

    }

    public void ChangeToMainMenu()
    {
        // Save in Player Prefs
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        PlayerPrefs.SetFloat(EffectVolumeKey, effectVolume);
        PlayerPrefs.SetFloat(GeneralVolumeKey, generalVolume);
        SceneController.LoadScene(SceneController.MainMenuIndex);
    }
    
    public void OnMusicVolumeChange()
    {
        musicVolume = musicVolumeSlider.value;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().volume = musicVolume * generalVolume;
    }

    public void OnGeneralVolumeChange()
    {
        generalVolume = generalVolumeSlider.value;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().volume = musicVolume * generalVolume;
        effectTest.volume = effectVolume * generalVolume;
    }
    
    public void OnEffectVolumeChange()
    {
        effectVolume = effectVolumeSlider.value;
        effectTest.volume = effectVolume * generalVolume;
        if (counter > 0) return;
        effectTest.Play();
        counter = soundCooldown;
    }

    private void Update()
    {
        if(counter > 0) counter -= Time.deltaTime;
    }

    public void WipeSaveData()
    {
        print("WipedSaveData");
        PlayerPrefs.DeleteAll();
    }
    
    
}