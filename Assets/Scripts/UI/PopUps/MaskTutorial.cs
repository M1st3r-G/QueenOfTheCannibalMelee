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
        base.Awake();
        Instance = this;
    }
    
    private void Close(InputAction.CallbackContext obj)
    {
        ToggleMenu();
    }
    
    public void Activate(MaskManager.MaskType mask)
    {
        (string Key, string TextValue) maskValues = mask switch
        {
            MaskManager.MaskType.Damage => (SettingsMenu.MaskTutorialKeyStrength,
                "The Mask seems to be pulsating with Rage.\n\nYou feel how your punches grow stronger.\n\nYou can equip it with 1"),
            MaskManager.MaskType.Speed => (SettingsMenu.MaskTutorialKeySpeed,
                "The Mask seems to be vibrating with electric energy.\n\nYou feel how your body moves faster.\n\nYou can equip it with 2"),
            MaskManager.MaskType.Block => (SettingsMenu.MaskTutorialKeyBlock,
                "The Mask feels heavy and emits a feeling of safety.\n\nYou notice how feel pained by your blocked punches.\n\nYou can equip it with 3"),
            MaskManager.MaskType.Health => (SettingsMenu.MaskTutorialKeyHealth,
                "The Mask feels light and refreshing.\n\nYou feel healthy and your Wounds start to heal\n\nYou can equip it with 4"),
            _ => throw new ArgumentOutOfRangeException(nameof(mask), mask, null)
        };

        if (PlayerPrefs.GetInt(maskValues.Key, 1) == 0) return;
        infoText.text = maskValues.TextValue;
        PlayerPrefs.SetInt(maskValues.Key, 0);

        FadeIn();
    }
    
    private void OnEnable()
    {
        closeAction.Enable();
        closeAction.performed += Close;
    }

    private void OnDisable()
    {
        closeAction.performed -= Close;
        closeAction.Disable();
    }
}