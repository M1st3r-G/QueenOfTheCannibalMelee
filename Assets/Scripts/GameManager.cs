using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    //ComponentReferences
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    private Transform cam;
    //Params
    [SerializeField] private float spawnTime;
    //Temps
    private bool isInLoading;
    private float counter;
    //Publics
    public static GameManager Instance { get; private set; }

    public int NextLevelIndex { get; private set; }

    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);

        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        isInLoading = true;

        Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NextLevelIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += RefreshReference;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= RefreshReference;
    }

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
        if (counter > spawnTime)
        {
            counter = 0;
            SpawnEnemy();
        }

        counter += Time.deltaTime;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
    
    /// <summary>
    /// Spawns an Enemy on the right side of the Camera in a Random Line
    /// </summary>
    private void SpawnEnemy()
    {
        print("Spawned Enemy");
        GameObject enemy = Instantiate(enemyPrefab, Vector3.right * (cam.position.x + 5), Quaternion.identity);
        enemy.transform.position = LineManager.Instance.SetToLine(enemy, Random.Range(0, LineManager.Instance.NumberOfLines));
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
}