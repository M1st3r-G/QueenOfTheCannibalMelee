using UnityEngine;

public class GameManager : MonoBehaviour
{
    //ComponentReferences
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
    }
    
    private void OnDestroy()
    {
        _instance = null;
    }
}