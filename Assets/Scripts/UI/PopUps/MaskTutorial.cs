using System;
using TMPro;
using Unity.VisualScripting;
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
        switch (mask)
        {
            case MaskManager.MaskType.Damage:
                if (PlayerPrefs.GetInt(SettingsMenu.MaskTutorialKeyStrength, 1) == 0) return;
                infoText.text =
                    "The Mask seems to be pulsating with Rage.\n\nYou feel how your punches grow stronger.\n\nYou can equip it with 1";
                PlayerPrefs.SetInt(SettingsMenu.MaskTutorialKeyStrength, 0);
                break;
            case MaskManager.MaskType.Speed:
                if (PlayerPrefs.GetInt(SettingsMenu.MaskTutorialKeySpeed, 1) == 0) return;
                infoText.text =
                    "The Mask seems to be vibrating with electric energy.\n\nYou fell how your body moves faster.\n\nYou can equip it with 2";
                PlayerPrefs.SetInt(SettingsMenu.MaskTutorialKeySpeed, 0);
                break;
            case MaskManager.MaskType.Block:
                if (PlayerPrefs.GetInt(SettingsMenu.MaskTutorialKeyBlock, 1) == 0) return;
                infoText.text =
                    "The Mask feels heavy and emits a feeling of safety.\n\nYou notice how it is easier for you to block punches now.\n\nYou can equip it with 3.";
                PlayerPrefs.SetInt(SettingsMenu.MaskTutorialKeyBlock, 0);
                break;
            case MaskManager.MaskType.Health:
                if (PlayerPrefs.GetInt(SettingsMenu.MaskTutorialKeyHealth, 1) == 0) return;
                infoText.text =
                    "The Mask feels light and refreshing.\n\nYou feel healthy and your Wounds start to heal\n\nYou can equip it with 4";
                PlayerPrefs.SetInt(SettingsMenu.MaskTutorialKeyHealth, 0);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mask), mask, null);
        }

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