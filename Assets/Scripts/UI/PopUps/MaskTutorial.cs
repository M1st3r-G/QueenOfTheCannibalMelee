using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MaskTutorial : PopUpMenu
{
    //ComponentReferences
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private InputAction closeAction;
    //Params
    //Temps
    //Public
    public static MaskTutorial Instance;
    
    private new void Awake()
    {
        Instance = this;
    }
    
    private void Close(InputAction.CallbackContext obj)
    {
        ToggleMenu();
    }

    public void Activate(MaskManager.MaskType mask)
    {
        infoText.text = mask switch
        {
            MaskManager.MaskType.Damage => "The Mask seems to be pulsating with Rage.\n\nYou feel how your punches grow stronger.\n\nYou can equip it with (1)",
            MaskManager.MaskType.Speed => "The Mask seems to be vibrating with electric energy.\n\nYou fell how your body moves faster.\n\nYou can equip it with (2)",
            MaskManager.MaskType.Block => "The Mask feels heavy and emits a feeling of safety.\n\nYou notice how it is easier for you to block punches now.\n\nYou can equip it with (3).",
            MaskManager.MaskType.Health => "The Mask feels light and refreshing.\n\nYou feel healthy and your Wounds start to heal\n\nYou can equip it with (4)",
            _ => throw new ArgumentOutOfRangeException(nameof(mask), mask, null)
        };
        FadeIn();
    }
    
    private void OnEnable() => closeAction.performed += Close;
    private void OnDisable() => closeAction.performed -= Close;
}