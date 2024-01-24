using UnityEngine;
using UnityEngine.SceneManagement;

public class MaskManager : MonoBehaviour
{
    //ComponentReferences
    private MaskUIController maskUI;
    [SerializeField] private MaskData[] allMasks;
    //Params
    [SerializeField] private float baseDropRate;
    [SerializeField] private int maxMasksPerLevel;
    //Temps
    private int masksFound;
    //Public
    public bool[] Unlocked { get; private set; }
    public MaskType CurrentMaskType { get; private set; }
    public static MaskManager Instance { get; private set; }
    public float MaskDropRate => masksFound == maxMasksPerLevel ? 0 : baseDropRate;

    public enum MaskType {
        Damage, Speed, Block, Health, None
    }
    
    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
        
        CurrentMaskType = MaskType.None;
        Unlocked = new[] { false, false, false, false };
    }


    private void AwakeInScene(Scene s, LoadSceneMode m)
    {
        if (SceneController.IsInLoading) return;
        masksFound = 0;
        maskUI = GameObject.FindGameObjectWithTag("MaskUI").GetComponent<MaskUIController>();
        maskUI.UnlockMasks(Unlocked);
        maskUI.SetMaskActive(CurrentMaskType);
    }

    public void Dequip()
    {
        maskUI.DisableMask(CurrentMaskType);
        CurrentMaskType = MaskType.None;
    }

    public void UnlockMask(MaskType type)
    {
        maskUI.SetUnlocked(type);
    }
    
    public void Equip(MaskType mask)
    {
        if (CurrentMaskType == MaskType.None) maskUI.DisableMask(CurrentMaskType);

        if (!Unlocked[(int)mask]) return;
        CurrentMaskType = mask;
        maskUI.SetMaskActive(CurrentMaskType);
    }
    
    private void OnDestroy()
    {
        if(Instance == this) Instance = null;
    }
    
    public void IncreaseMaskFound() => masksFound++;
    public MaskData GetMask(MaskType type) => type == MaskType.None ? null : allMasks[(int)type];
    private void OnEnable() => SceneManager.sceneLoaded += AwakeInScene;
    private void OnDisable() => SceneManager.sceneLoaded -= AwakeInScene;
    public Sprite GetMaskSprite(MaskType mask) => mask == MaskType.None ? null : allMasks[(int)mask].MaskSprite;
}