using UnityEngine;
using UnityEngine.UI;

public class MaskUIController : MonoBehaviour
{
    //ComponentReferences
    [SerializeField] private Image[] masks;
    //Params
    //Temps
    //Publics
    public static MaskUIController Instance { get; set; }

    
    
    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }
    
    private void OnDestroy()
    {
        Instance = null;
    }

    public void SetMaskUnlocked(int index)
    {
        masks[index].color = Color.white;
    }
}