using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuController : PopUpMenu
{
    //ComponentReferences
    [SerializeField] private InputAction pauseAction;
    [SerializeField] private SettingsMenu settings;
    //Params
    //Temps
    private bool gameOver;
    //Public
    
    private void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.performed += OnPause;
        PlayerController.OnGameOver += OnGameOver;
    }

    private void OnDisable()
    {        
        pauseAction.performed -= OnPause;
        pauseAction.Disable();
        PlayerController.OnGameOver -= OnGameOver;
    }

    private void OnGameOver()
    {
        gameOver = true;
        JumpTo(false); 
    }
    
    private void OnPause(InputAction.CallbackContext ctx)
    {
        if (gameOver) return;
        ToggleMenu();
    }

    public void OnSettingsButton()
    {
        JumpTo(false);
        settings.JumpTo(true);
    }
    
    public void ContinueButton() => OnPause(new InputAction.CallbackContext());
    public void OnQuitButton() => Application.Quit();
    public void OnMenuButton() => SceneController.Instance.CleanLoadMainMenu();
}