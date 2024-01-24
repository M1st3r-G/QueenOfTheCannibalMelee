using UnityEngine;
using UnityEngine.UI;

public class MaskUIController : MonoBehaviour
{
    //ComponentReferences
    [SerializeField] private Image[] maskImages;
    [SerializeField] private GameObject border;
    //Params
    private Vector3 nonePosition;
    [SerializeField] private Color unknown;
    //Temps
    //Public
    
    private void Awake()
    {
        nonePosition = border.transform.position;
        foreach (Image mask in maskImages)
        {
            mask.color = unknown;
        }
    }

    public void UnlockMasks(bool[] unlocked)
    {
        for (int i = 0; i < unlocked.Length; i++)
        {
            if (unlocked[i]) maskImages[i].color = Color.white;
        }
    }
    
    public void DisableMask(MaskManager.MaskType mask)
    {
        if (mask == MaskManager.MaskType.None) return;

        border.transform.position = nonePosition;
    }
    
    public void SetMaskActive(MaskManager.MaskType mask)
    {
        if (mask == MaskManager.MaskType.None) return;
        border.transform.position = maskImages[(int)mask].transform.position;
    }
    
    public void SetUnlocked(MaskManager.MaskType mask)
    {
        if (mask == MaskManager.MaskType.None) return;
        maskImages[(int) mask].color = Color.white;
    }
}