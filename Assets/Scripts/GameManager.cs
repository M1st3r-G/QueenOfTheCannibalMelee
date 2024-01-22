using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    //ComponentReferences
    [SerializeField] private GameObject playerPrefab;
    private static LevelData CurrentLevel => SceneController.Instance.GetCurrentLevel();
    private Transform cam;
    //Params
    [SerializeField] private int spawnCap;
    [SerializeField] private float spawnTime;
    [SerializeField] private float meleeChance;
    //Temps
    private int numberOfEnemies;
    private float counter;
    //Public
    private static GameManager Instance { get; set; }
    public static AudioClip LevelMusic => CurrentLevel.Music;
    
    
    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
        
        Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
    }
    
    private void Start()
    {
        if (SceneController.IsInBossArena) return;
        Instantiate(CurrentLevel.Transition);
        Instantiate(CurrentLevel.LevelObject, Vector3.zero, Quaternion.identity);
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
    
    private void Update()
    {
        if (SceneController.IsInLoading || SceneController.IsInBossArena || numberOfEnemies >= spawnCap) return;
        
        if (counter > spawnTime)
        {
            counter = 0;
            SpawnEnemy();
        }

        counter += Time.deltaTime;
    }
    
    private void SpawnEnemy()
    {
        print("Spawned Enemy");
        numberOfEnemies++;
        Instantiate(
            Random.Range(0f, 1f) < meleeChance ? CurrentLevel.MeleeEnemyInLevel : CurrentLevel.RangedEnemyInLevel,
            Vector3.right * (cam.position.x + 5), Quaternion.identity);
    }
    
    private void OnDestroy() => Instance = null;
    private void RefreshReference(Scene s, LoadSceneMode m) => cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
    private void OnEnemyDeath() => numberOfEnemies--;
}
