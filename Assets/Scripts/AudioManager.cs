using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    //ComponentReferences
    [SerializeField] private AudioClip[] clips;
    private AudioSource effectAudioSource;
    private AudioSource musicAudioSource;
    //Params
    private float musicVolume;
    private float effectVolume;
    private float generalVolume;
    //Temps
    //Publics
    public const int PlayerPunch = 0;
    public const int PlayerHit = 1;
    public const int PlayerDeath = 2;
    public const int PlayerDeathScream = 3;
    public const int Enemy1Death = 4;
    public const int SettingsSound = 5;
    public const int ItemCollect = 6;
    public const int PlayerBlock = 7;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);

        musicVolume = PlayerPrefs.GetFloat(SettingsMenu.MusicVolumeKey, 0.75f);
        effectVolume = PlayerPrefs.GetFloat(SettingsMenu.EffectVolumeKey, 0.75f);
        generalVolume = PlayerPrefs.GetFloat(SettingsMenu.GeneralVolumeKey, 0.75f);
        
        effectAudioSource = GetComponent<AudioSource>();
        effectAudioSource.volume = effectVolume * generalVolume;
    }

    private void OnDestroy()
    {
        if(Instance == this) Instance = null;
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        if (SceneController.IsInLoading) return;
        musicAudioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
        musicAudioSource.clip = GameManager.LevelMusic;
        musicAudioSource.volume = musicVolume * generalVolume;
        musicAudioSource.Play();
    }
    
    public void PlayAudioEffect(int index)
    {
        print($"Played AudioEffect {index}");
        effectAudioSource.PlayOneShot(clips[index]);
    }
    
    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
}