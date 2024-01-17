using UnityEngine;
using UnityEngine.UI;

public class MaskUIController : MonoBehaviour
{
    //ComponentReferences
    [SerializeField] private Image[] masks;
    //Params
    [SerializeField] private Color unknown;
    [SerializeField] private Color disabled;
    private readonly Color active = Color.white;
    //Temps
    private int currentlyActive;
    //Publics
    public static MaskUIController Instance { get; private set; }

    
    
    private void Awake()
    {
        Instance = this;
        currentlyActive = -1;

        foreach (Image mask in masks)
        {
            mask.color = unknown;
        }
    }
    
    private void OnDestroy()
    {
        Instance = null;
    }

    public void SetMaskActive(int index)
    {
        if (currentlyActive != -1) masks[currentlyActive].color = disabled;
        masks[index].color = active;
        currentlyActive = index;
    }
}