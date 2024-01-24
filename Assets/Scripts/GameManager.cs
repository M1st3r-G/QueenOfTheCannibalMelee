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

    private void RefreshReference(Scene s, LoadSceneMode m)
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if (SceneController.IsInBossArena || SceneController.IsInLoading) return;
        Instantiate(CurrentLevel.Transition);
        Instantiate(CurrentLevel.LevelObject, Vector3.zero, Quaternion.identity, cam);
        numberOfEnemies = 0;
    }
    
    private void OnEnemyDeath() => numberOfEnemies--;
    
    private void Update()
    {
        if (SceneController.IsInLoading || SceneController.IsInBossArena) return;
        
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
        if(Instance == this) Instance = null;
    }

    /// <summary>
    /// Spawns an Enemy on the right side of the Camera
    /// </summary>
    private void SpawnEnemy()
    {
        print("Spawned Enemy");
        float rnd = Random.Range(0f, 1f);
        Instantiate(rnd < meleeChance ? CurrentLevel.MeleeEnemyInLevel : CurrentLevel.RangedEnemyInLevel,
            Vector3.right * (cam.position.x + 5), Quaternion.identity);
    }
}