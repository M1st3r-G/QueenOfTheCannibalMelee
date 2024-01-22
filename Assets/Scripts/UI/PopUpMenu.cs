using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class PopUpMenu: MonoBehaviour
    {
        //ComponentReferences
        private CanvasGroup group;
        [SerializeField] private EventSystem controllerUI;
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
        }

        public void ToggleMenu()
        {
            if (currentTransfer is not null) StopCoroutine(currentTransfer);
            currentTransfer = StartCoroutine(FadeToRoutine(!isActive));
        }
    
        private IEnumerator FadeToRoutine(bool active)
        {
            controllerUI.SetSelectedGameObject(firstSelectionInMenu);
        
            group.interactable = group.blocksRaycasts = isActive = active;
            Time.timeScale = active ? 0f : 1f;
        
            if (active) playerActions.Disable();
            else playerActions.Enable();

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
        
            group.alpha = active ? 1f : 0f;
            group.interactable = group.blocksRaycasts = isActive = active;
            Time.timeScale = active ? 0f : 1f;

            if (active) playerActions.Disable();
            else playerActions.Enable();
        }
    
        protected void FadeIn() => StartCoroutine(FadeToRoutine(true));
    }
}
