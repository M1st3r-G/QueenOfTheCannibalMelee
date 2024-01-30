using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CanvasGroup))]
public abstract class PopUpMenu: MonoBehaviour
{
    //ComponentReferences
    private CanvasGroup group;
    [SerializeField] protected EventSystem controllerUI;
    [SerializeField] protected InputActionAsset playerActions;
    [SerializeField] private GameObject firstSelectionInMenu;
    //Params
    [SerializeField] private float fadeInTime;
    //Temps
    //Public
    private bool isActive;
    private Coroutine currentTransfer;


    protected void Awake()
    {
        group = GetComponent<CanvasGroup>();
        isActive = false;
    }

    protected void FadeIn()
    {
        StartCoroutine(FadeToRoutine(true));
    }

    public void ToggleMenu()
    {
        if (currentTransfer is not null) StopCoroutine(currentTransfer);
        currentTransfer = StartCoroutine(FadeToRoutine(!isActive));
    }
    
    private IEnumerator FadeToRoutine(bool active)
    {
        group.interactable = active;
        group.blocksRaycasts = active;
        
        controllerUI.SetSelectedGameObject(firstSelectionInMenu);

        Time.timeScale = active ? 0f : 1f;
        if (active) playerActions.Disable();
        else playerActions.Enable();

        isActive = active;
        
        float counter = 0f;
        while (active ? group.alpha < 1 : group.alpha > 0)
        {
            group.alpha = Mathf.Lerp(active ? 0f : 1f, active ? 1f : 0f, counter / fadeInTime);
            counter += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    public void JumpTo(bool active)
    {
        if (currentTransfer is not null) StopCoroutine(currentTransfer);
        
        isActive = active;
        group.alpha = active ? 1f : 0f;
        group.interactable = active;
        group.blocksRaycasts = active;
        if (active) playerActions.Disable();
        else playerActions.Enable();
        Time.timeScale = active ? 0f : 1f;
    }
}
