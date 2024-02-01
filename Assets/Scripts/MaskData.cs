using UnityEngine;

[CreateAssetMenu(menuName = "Mask")]
public class MaskData : ScriptableObject
{
    //ComponentReferences
    public Sprite MaskSprite => sprite;
    [SerializeField] private Sprite sprite;
    
    public float Deflect => deflectModifier;
    [SerializeField] private float deflectModifier; 
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
    //Params
    //Temps
    //Publics
}