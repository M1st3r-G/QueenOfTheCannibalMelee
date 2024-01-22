using UnityEngine;
using UnityEngine.UI;

public class MaskUIController : MonoBehaviour
{
    //ComponentReferences
    public Sprite GetMaskSprite(int index) => maskImages[index].sprite;
    [SerializeField] private Image[] maskImages;
    [SerializeField] private MaskData[] allMasks;
    //Params
    [SerializeField] private Color unknown;
    [SerializeField] private Color disabled;
    [SerializeField] private float baseDropRate;
    [SerializeField] private int maxMasksPerLevel;
    private readonly Color active = Color.white;
    //Temps
    public bool[] Unlocked { get; private set; }
    private int currentlyActive;
    //Publics
    public static MaskUIController Instance { get; private set; }
    public float MaskDropRate => masksFound == maxMasksPerLevel ? 0 : baseDropRate;
    [HideInInspector] public int masksFound;
    
    private void Awake()
    {
        Instance = this;
        currentlyActive = -1;
        masksFound = 0;

        Unlocked = new []{false, false, false, false};
        
        foreach (Image mask in maskImages)
        {
            mask.color = unknown;
        }
    }

    public void UnlockMask(int index)
    {
        Unlocked[index] = true;
        maskImages[index].color = disabled;
    }
    
    private void OnDestroy()
    {
        Instance = null;
    }
    
    public bool SetMaskActive(int index)
    {
        if (!Unlocked[index]) return false;
        if (currentlyActive != -1) maskImages[currentlyActive].color = disabled;
        maskImages[index].color = active;
        currentlyActive = index;
        return true;
    }

    public void SetUnlocked(bool[] newLock)
    {
        Unlocked = newLock;
        for (int i = 0; i < Unlocked.Length; i++)
        {
            if(Unlocked[i]) UnlockMask(i);
        }
    }
}