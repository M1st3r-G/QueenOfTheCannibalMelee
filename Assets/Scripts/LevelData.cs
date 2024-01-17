using UnityEngine;

[CreateAssetMenu(menuName = "LevelData")]
public class LevelData : ScriptableObject
{
    public GameObject LevelObject => levelLayout;
    [SerializeField] private GameObject levelLayout;

    public GameObject Transition => transition;
    [SerializeField] private GameObject transition;
    
    public GameObject EnemyInLevel => enemy;
    [SerializeField] private GameObject enemy;

    public AudioClip Music => levelMusic;
    [SerializeField] private AudioClip levelMusic;
}
