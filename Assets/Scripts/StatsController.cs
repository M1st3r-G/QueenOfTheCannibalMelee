using UnityEngine;
using UnityEngine.InputSystem;

public class StatsController : MonoBehaviour
{
    //ComponentReferences
    [SerializeField] private MaskData currentMask;
    [SerializeField] private MaskData[] allMasks;
    
    //ReadParams
    public int Damage =>(int)(baseDamage * (currentMask?.DamageMod ?? 1));
    [SerializeField] private int baseDamage;
    public float MovementSpeed => baseMovementSpeed * (currentMask?.MovementMod ?? 1);
    [SerializeField] private float baseMovementSpeed;
    public float KnockBackSpeed => baseKnockBackSpeed * (currentMask?.KnockBackMod ?? 1);
    [SerializeField] private float baseKnockBackSpeed;
    public float KnockBackDistance => baseKnockBackDistance * (currentMask?.KnockBackMod ?? 1);
    [SerializeField] private float baseKnockBackDistance;
    //ActiveParams
    public int MaxHealth => (int)(baseMaxHealth * (currentMask?.HealthMod ?? 1));
    [SerializeField] private int baseMaxHealth;
    public float AnimSpeed => currentMask?.AnimationMod ?? 1;
    
    //Temps
    //Publics
    public delegate void HealthChange(int newMax);
    public delegate void AnimSpeedChange(float newValue);
    public static AnimSpeedChange OnAnimSpeedChange;
    public static HealthChange OnHealthChange;
    
    public void ChangeMask(int index)
    {
        if (index < 0 || index >= allMasks.Length) return;
        MaskData newMask = allMasks[index];
        if (newMask == currentMask) return;
        
        currentMask = newMask;
        OnHealthChange?.Invoke((int)(baseMaxHealth * newMask.HealthMod));
        OnAnimSpeedChange?.Invoke(currentMask.AnimationMod);
    }
}