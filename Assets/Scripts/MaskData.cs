using UnityEngine;

[CreateAssetMenu(menuName = "Mask")]
public class MaskData : ScriptableObject
{
    //ComponentReferences
    public float BlockMod => blockModifier;
    [SerializeField] private float blockModifier; 
    public float MovementMod => movementSpeedModifier;
    [SerializeField] private float movementSpeedModifier;
    public float AnimationMod => animationSpeedModifier;
    [SerializeField] private float animationSpeedModifier;
    
    public float HealthMod => healthModifier;
    [SerializeField] private float healthModifier;
    
    public float DamageMod => damageModifier;
    [SerializeField] private float damageModifier;
    public float KnockBackMod => knockBackModifier;
    [SerializeField] private float knockBackModifier;
}