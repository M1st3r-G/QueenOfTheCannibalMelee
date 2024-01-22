using UnityEngine;

[CreateAssetMenu(menuName = "LevelData")]
public class LevelData : ScriptableObject
{
    public GameObject LevelObject => levelLayout;
    [SerializeField] private GameObject levelLayout;

    public GameObject Transition => transition;
    [SerializeField] private GameObject transition;

    public GameObject RangedEnemyInLevel => rangedEnemy;
    [SerializeField] private GameObject rangedEnemy;
    
    public GameObject MeleeEnemyInLevel => meleeEnemy;
    [SerializeField] private GameObject meleeEnemy;

    public AudioClip Music => levelMusic;
    [SerializeField] private AudioClip levelMusic;
}
