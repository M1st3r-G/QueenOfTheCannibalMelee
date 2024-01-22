using UnityEngine;

public class StatsController : MonoBehaviour
{
    //ComponentReferences
    [SerializeField] private MaskData currentMask;
    [SerializeField] private MaskData[] allMasks;
    
    //ReadParams
    public float DamageBlock => baseDamageBlock * (currentMask != null ? currentMask.BlockMod : 1);
    [SerializeField] private float baseDamageBlock;
    public int Damage =>(int)(baseDamage * (currentMask != null ? currentMask.DamageMod : 1));
    [SerializeField] private int baseDamage;
    public float MovementSpeed => baseMovementSpeed * (currentMask != null ? currentMask.MovementMod : 1);
    [SerializeField] private float baseMovementSpeed;
    public float KnockBackSpeed => baseKnockBackSpeed * (currentMask != null ? currentMask.KnockBackMod : 1);
    [SerializeField] private float baseKnockBackSpeed;
    public float KnockBackDistance => baseKnockBackDistance * (currentMask != null ? currentMask.KnockBackMod : 1);
    [SerializeField] private float baseKnockBackDistance;
    //ActiveParams
    public int MaxHealth => (int)(baseMaxHealth * (currentMask != null ? currentMask.HealthMod : 1));
    [SerializeField] private int baseMaxHealth;
    public float AnimSpeed => currentMask != null ? currentMask.AnimationMod : 1;
    
    //Temps
    //Publics
    public delegate void HealthChange(int oldMax, bool higher);
    public static HealthChange OnHealthChange;
    public delegate void AnimSpeedChange();
    public static AnimSpeedChange OnAnimSpeedChange;
    
    public void ChangeMask(int index)
    {
        if (index < 0 || index >= allMasks.Length) return;
        
        MaskData newMask = allMasks[index];
        if (newMask == currentMask) return;
        
        int oldMax = MaxHealth; 
        currentMask = newMask;
        
        OnHealthChange?.Invoke(oldMax, MaxHealth>oldMax);
        OnAnimSpeedChange?.Invoke();
    }
}