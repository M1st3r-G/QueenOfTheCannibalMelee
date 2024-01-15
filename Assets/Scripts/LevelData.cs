using UnityEngine;

[CreateAssetMenu(menuName = "LevelData")]
public class LevelData : ScriptableObject
{
    public GameObject LevelObject => levelLayout;
    [SerializeField] private GameObject levelLayout;
    
    public GameObject EnemyInLevel => enemy;
    [SerializeField] private GameObject enemy;
}