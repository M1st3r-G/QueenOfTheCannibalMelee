using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    //ComponentReferences
    private CanvasGroup group;

    [SerializeField] private InputActionAsset actions;
    [SerializeField] private Toggle moveToggle;
    [SerializeField] private Toggle attackToggle;
    [SerializeField] private Toggle changeToggle;
    [SerializeField] private Toggle blockToggle;
    //Params
    [SerializeField] private float fadeOutTime;
    [SerializeField] private float waitTime;
    //Temps
    private Coroutine current;
    private bool moved;
    private bool blocked;
    private bool attacked;
    private bool changed;
    //Public

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        if (PlayerPrefs.GetInt(SettingsMenu.MovementTutorialKey, 1) == 0) Destroy(gameObject);
        else group.alpha = 1f;
    }

    private void Start() => GameManager.Instance.SetCanSpawn(false);

    private void OnEnable()
    {
        actions.FindAction("Move").performed += OnMove;
        actions.FindAction("Attack").performed += OnAttack;
        actions.FindAction("ChangeLineUp").performed += OnLineChange;
        actions.FindAction("ChangeLineDown").performed += OnLineChange;
        actions.FindAction("Block").performed += OnBlock;
    }

    private void OnDisable()
    {
        actions.FindAction("Move").performed -= OnMove;
        actions.FindAction("Attack").performed -= OnAttack;
        actions.FindAction("ChangeLineUp").performed -= OnLineChange;
        actions.FindAction("ChangeLineDown").performed -= OnLineChange;
        actions.FindAction("Block").performed -= OnBlock;
    }
    
    private void OnMove(InputAction.CallbackContext ctx)
    {
        moved = moveToggle.isOn = true;
        if (!moved || !attacked || !changed || !blocked) return;

        if (current is not null) return;
        current = StartCoroutine(FadeOut());
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        attacked = attackToggle.isOn = true;
        if (!moved || !attacked || !changed || !blocked) return;

        if (current is not null) return;
        current = StartCoroutine(FadeOut());
    }

    private void OnLineChange(InputAction.CallbackContext ctx)
    {
        changed = changeToggle.isOn = true;
        if (!moved || !attacked || !changed || !blocked) return;
        if (current is not null) return;
        current = StartCoroutine(FadeOut());
    }

    private void OnBlock(InputAction.CallbackContext ctx)
    {
        blocked = blockToggle.isOn = true;
        if (!moved || !attacked || !changed || !blocked) return;
        if (current is not null) return;
        current = StartCoroutine(FadeOut());
    }
    
    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(waitTime);
        float counter = 0f;
        while (group.alpha > 0)
        {
            group.alpha = Mathf.Lerp(1f, 0f, counter / fadeOutTime);
            counter += Time.deltaTime;
            yield return null;
        }

        GameManager.Instance.SetCanSpawn(true);
        PlayerPrefs.SetInt(SettingsMenu.MovementTutorialKey, 0);
    }
}