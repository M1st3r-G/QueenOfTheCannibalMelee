using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //ComponentReferences
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private LevelData level;
    private GameObject enemyPrefab;
    private Transform cam;
    //Params
    [SerializeField] private int spawnCap;
    [SerializeField] private float spawnTime;
    //Temps
    private bool isInLoading;
    private int numberOfEnemies;
    private float counter;
    //Publics
    public static GameManager Instance { get; private set; }
    public int NextLevelIndex { get; private set; }
    public AudioClip LevelMusic => level.Music;
    
    
    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
        
        isInLoading = true;
        NextLevelIndex = SceneManager.GetActiveScene().buildIndex;
        
        enemyPrefab = level.EnemyInLevel;
        
        Instantiate(level.Transition);
        Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        Instantiate(level.LevelObject, Vector3.zero, Quaternion.identity);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += RefreshReference;
        EnemyController.OnEnemyDeath += OnEnemyDeath;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= RefreshReference;
        EnemyController.OnEnemyDeath -= OnEnemyDeath;
    }

    private void OnEnemyDeath() => numberOfEnemies--;
    
    /// <summary>
    /// This Method is Triggered when Loading into a new Scene, it resets Refrences and counts up the Levels
    /// </summary>
    /// <param name="s">irrelevant</param>
    /// <param name="m">irrelevant</param>
    private void RefreshReference(Scene s, LoadSceneMode m)
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        NextLevelIndex++;
        isInLoading = !isInLoading; 
    }
    
    private void Update()
    {
        if (isInLoading) return;
        if (numberOfEnemies >= spawnCap) return;
        if (counter > spawnTime)
        {
            counter = 0;
            SpawnEnemy();
            numberOfEnemies++;
        }

        counter += Time.deltaTime;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
    
    /// <summary>
    /// Spawns an Enemy on the right side of the Camera
    /// </summary>
    private void SpawnEnemy()
    {
        print("Spawned Enemy");
        Instantiate(enemyPrefab, Vector3.right * (cam.position.x + 5), Quaternion.identity);
    }
    
    /// <summary>
    /// Loads the Next Scene in BuildSettings, if the Current Scene was the Last Scene, it Resets to Index 0
    /// </summary>
    public static void LoadNextScene()
    {   
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings) nextSceneIndex = 0;
        print($"LoadNextScene: {nextSceneIndex}");
        SceneManager.LoadScene(nextSceneIndex);
    }
    
    public void LoadMainMenu()
    {
        Destroy(GameManager.Instance.gameObject);
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
}