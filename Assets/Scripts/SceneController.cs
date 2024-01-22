using UI;
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
    [SerializeField] private int currentLevel;
    private bool[] unlocked;
    
    //Public
    public static bool IsInLoading => SceneManager.GetActiveScene().buildIndex == LoadingScreenIndex;
    public static bool IsInBossArena => SceneManager.GetActiveScene().buildIndex == BossLevelIndex;
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
    
    public void LoadNextScene()
    {
        player.gameObject.SetActive(IsInLoading);
        if (IsInLoading)
        {
            currentLevel++;

            if (currentLevel == levels.Length - 1) player.FullHeal();
            else unlocked = MaskUIController.Instance.Unlocked;

            SceneManager.LoadScene(currentLevel == levels.Length - 1 ? BossLevelIndex : DefaultLevelIndex);
        }
        else SceneManager.LoadScene(LoadingScreenIndex);
    }
    
    private void RefreshMasks(Scene s, LoadSceneMode m)
    {
        if (!IsInLoading) MaskUIController.Instance.SetUnlocked(unlocked);
    }
    
    public void CleanLoadMainMenu()
    {
        Destroy(gameObject);
        Destroy(player.gameObject);
        Time.timeScale = 1f;
        SceneManager.LoadScene(MainMenuIndex);
    }
    
    private void Start() => player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    private void OnEnable() => SceneManager.sceneLoaded += RefreshMasks;
    private void OnDisable() => SceneManager.sceneLoaded -= RefreshMasks;
    private void OnDestroy() => Instance = null;
    public static void LoadFirstLevel() => SceneManager.LoadScene(DefaultLevelIndex);
    public static void LoadFirstLoadingScreen() => SceneManager.LoadScene(LoadingScreenIndex);
    public LevelData GetCurrentLevel() => levels[currentLevel];
}
