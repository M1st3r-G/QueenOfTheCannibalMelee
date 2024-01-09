using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //ComponentReferences
    [SerializeField] private GameObject playerPrefab;
    private GameObject player;
    //Params
    //Temps
    //Publics
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    
    private void Awake()
    {
        if (_instance is not null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this);

        player = Instantiate(playerPrefab, new Vector3(LineManager.Instance.LineHeights[0], -3, 0f), Quaternion.identity);
    }
    
    private void OnDestroy()
    {
        _instance = null;
    }

    public static void LoadNextScene()
    {   
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings) nextSceneIndex = 0;
        print($"LoadNextScene: {nextSceneIndex}");
        SceneManager.LoadScene(nextSceneIndex);
    }
}