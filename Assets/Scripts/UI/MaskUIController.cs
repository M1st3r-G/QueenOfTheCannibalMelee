using UnityEngine;
using UnityEngine.UI;

public class MaskUIController : MonoBehaviour
{
    //ComponentReferences
    [SerializeField] private Image[] maskImages;
    //Params
    [SerializeField] private Color active = Color.white;
    [SerializeField] private Color unknown;
    [SerializeField] private Color disabled;
    //Temps
    //Public
    
    private void Awake()
    {
        foreach (Image mask in maskImages)
        {
            mask.color = unknown;
        }
    }

    public void UnlockMasks(bool[] unlocked)
    {
        for (int i = 0; i < unlocked.Length; i++)
        {
            if (unlocked[i]) maskImages[i].color = disabled;
        }        
    }
    
    public void DisableMask(MaskManager.MaskType mask)
    {
        if (mask == MaskManager.MaskType.None) return;
        
        maskImages[(int) mask].color = disabled;
    }
    
    public void SetMaskActive(MaskManager.MaskType mask)
    {
        if (mask == MaskManager.MaskType.None) return;
        maskImages[(int) mask].color = active;
    }
    
    public void SetUnlocked(MaskManager.MaskType mask)
    {
        if (mask == MaskManager.MaskType.None) return;
        maskImages[(int) mask].color = disabled;
    }
}