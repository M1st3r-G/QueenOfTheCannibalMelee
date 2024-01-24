using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private const int MainMenuIndex = 1;
    private const int DefaultLevelIndex = 2;
    private const int LoadingScreenIndex = 3;
    private const int BossLevelIndex = 4;

    //ComponentReferences
    private PlayerController player;
    //Params
    [SerializeField] private LevelData[] levels;
    //Temps
    public static bool IsInMainMenu => SceneManager.GetActiveScene().buildIndex == MainMenuIndex;
    public static bool IsInLoading => SceneManager.GetActiveScene().buildIndex == LoadingScreenIndex;
    public static bool IsInBossArena => SceneManager.GetActiveScene().buildIndex == BossLevelIndex;
    [SerializeField] private int currentLevel;

    private bool[] unlocked;
    //Public
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

        unlocked = new[] { false, false, false, false };
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += RefreshMasks;
    }

    private void RefreshMasks(Scene s, LoadSceneMode m)
    {
        if (IsInLoading) return;
        MaskUIController.Instance.SetUnlocked(unlocked);
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= RefreshMasks;
    }
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    
    //private void OnDestroy() => Instance = null;

    /// <summary>
    /// Loads the Next Scene in BuildSettings, if the Current Scene was the Last Scene, it Resets to Index 0
    /// </summary>
    public void LoadNextScene()
    {
        player.gameObject.SetActive(IsInLoading);
        if (!IsInLoading)
        {
            unlocked = MaskUIController.Instance.Unlocked;
            SceneManager.LoadScene(LoadingScreenIndex);
        }
        else
        {
            currentLevel++;
            if (currentLevel == levels.Length - 1)
            {
                player.FullHeal();
                SceneManager.LoadScene(BossLevelIndex);
            }
            else
            {
                SceneManager.LoadScene(DefaultLevelIndex);
            }
        }
    }
    
    public void CleanLoadMainMenu()
    {
        Destroy(gameObject);
        Destroy(player.gameObject);
        Time.timeScale = 1f;
        SceneManager.LoadScene(MainMenuIndex);
    }
    
    public static void LoadFirstLevel() => SceneManager.LoadScene(DefaultLevelIndex);
    public static void LoadFirstLoadingScreen() => SceneManager.LoadScene(LoadingScreenIndex);
    public LevelData GetCurrentLevel() => levels[currentLevel];
}
