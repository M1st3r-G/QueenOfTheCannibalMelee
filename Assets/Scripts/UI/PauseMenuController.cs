using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    //ComponentReferences
    [SerializeField] private InputActionAsset playerActions;
    [SerializeField] private EventSystem controllerUI;
    //Params
    //Temps
    //Publics
    
    [SerializeField] private InputActionReference pauseAction;
    private CanvasGroup group;
    //Params
    [SerializeField] private float fadeInTime;
    //Temps
    private bool isPaused;
    private bool gameOver;
    private Coroutine currentTransfer;
    
    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        isPaused = false;
    }
    
    private void OnEnable()
    {
        pauseAction.action.Enable();
        pauseAction.action.performed += OnPause;
        PlayerController.OnGameOver += OnGameOver;
    }

    private void OnDisable()
    {        
        pauseAction.action.performed -= OnPause;
        pauseAction.action.Disable();
        PlayerController.OnGameOver += OnGameOver;
    }

    private void OnGameOver()
    {
        gameOver = true;
        if (currentTransfer is not null) StopCoroutine(currentTransfer);

        isPaused = false;
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }
    
    private void OnPause(InputAction.CallbackContext ctx)
    {
        if (gameOver) return;
        if (currentTransfer is not null) StopCoroutine(currentTransfer);
        currentTransfer = StartCoroutine(FadeTo(!isPaused));
    }
    
    private IEnumerator FadeTo(bool active)
    {
        float counter = 0f;
        Time.timeScale = active ? 0f : 1f;


        if (active) playerActions.Disable();
        else playerActions.Enable();

        controllerUI.SetSelectedGameObject(transform.GetChild(0).gameObject);
        isPaused = active;
        group.interactable = active;
        group.blocksRaycasts = active;

        while (active ? group.alpha < 1 : group.alpha > 0)
        {
            group.alpha = Mathf.Lerp(active ? 0f : 1f, active ? 1f : 0f, counter / fadeInTime);
            counter += Time.unscaledDeltaTime;
            yield return null;
        }
    }
    
    public void ContinueButton() => OnPause(new InputAction.CallbackContext());
    public void OnQuitButton() => Application.Quit();
    public void OnMenuButton() => SceneController.Instance.CleanLoadMainMenu();
}