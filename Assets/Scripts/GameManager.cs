using System;
using System.Collections;
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
    private float counter;
    //Publics
    public static GameManager Instance => _instance;
    private static GameManager _instance;
    
    private void Awake()
    {
        if (_instance is not null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this);

        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        
        Instantiate(playerPrefab, new Vector3(LineManager.Instance.LineHeights[0], -3, 0f), Quaternion.identity);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += RefreshReference;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= RefreshReference;
    }

    private void RefreshReference(Scene s, LoadSceneMode m)
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }
    
    private void Update()
    {
        if (counter > spawnTime)
        {
            counter = 0;
            SpawnEnemy();
        }

        counter += Time.deltaTime;
    }

    private void OnDestroy()
    {
        _instance = null;
    }
    
    private void SpawnEnemy()
    {
        Vector3 pos = cam.position + Vector3.right * 5;
        GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
        LineManager.Instance.SetToLine(enemy, Random.Range(0, LineManager.Instance.NumberOfLines));
    }
    
    public static void LoadNextScene()
    {   
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings) nextSceneIndex = 0;
        print($"LoadNextScene: {nextSceneIndex}");
        SceneManager.LoadScene(nextSceneIndex);
    }
}