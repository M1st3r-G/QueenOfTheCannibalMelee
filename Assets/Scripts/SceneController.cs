using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public const int MainMenuIndex = 1;
    public const int SettingsIndex = 2;
    public const int DefaultLevelIndex = 3;
    private const int LoadingScreenIndex = 4;
    private const int BossLevelIndex = 5;
    
    //ComponentReferences
    private GameObject player;
    //Params
    [SerializeField] private LevelData[] levels;
    //Temps
    public static bool IsInLoading => SceneManager.GetActiveScene().buildIndex == LoadingScreenIndex;
    public static bool IsInBossArena => SceneManager.GetActiveScene().buildIndex == BossLevelIndex;
    [SerializeField] private int currentLevel;
    //Publics
    public static SceneController Instance { get; private set; }

    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    
    private void OnDestroy() => Instance = null;

    /// <summary>
    /// Loads the Next Scene in BuildSettings, if the Current Scene was the Last Scene, it Resets to Index 0
    /// </summary>
    public void LoadNextScene()
    {
        if (!IsInLoading) SceneManager.LoadScene(LoadingScreenIndex);
        else
        {
            currentLevel++;
            SceneManager.LoadScene(currentLevel == levels.Length - 1 ? BossLevelIndex : DefaultLevelIndex);
        }
    }
    
    public void CleanLoadMainMenu()
    {
        Destroy(gameObject);
        Destroy(player);
        Time.timeScale = 1f;
        SceneManager.LoadScene(MainMenuIndex);
    }
    
    public static void LoadScene(int i)
    {
        switch (i)
        {
            case MainMenuIndex:
                SceneManager.LoadScene(MainMenuIndex);
                break;
            case SettingsIndex:
                SceneManager.LoadScene(SettingsIndex);
                break;
            default:
                throw new Exception("Unexpected Scene Index");
        }
    }
    
    public static void LoadFirstLevel() => SceneManager.LoadScene(DefaultLevelIndex);
    public static void LoadFirstLoadingScreen() => SceneManager.LoadScene(LoadingScreenIndex);
    public LevelData GetCurrentLevel() => levels[currentLevel];
}
