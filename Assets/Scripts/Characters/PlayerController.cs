using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : Character
{
    private static readonly int AnimatorMaskParameter = Animator.StringToHash("MaskValue");
    
    //ComponentReferences
    private InputAction moveAction;
    private GameObject healthBar;
    [SerializeField] private Animator maskController;
    [SerializeField] private Gradient healthGradient;
    //Params
    [SerializeField] [Range(0f, 1f)] private float relativeEarlyEscape;
    //Temps
    //Public
    public delegate void GameOverDelegate();
    public static GameOverDelegate OnGameOver;
    public delegate void ControlChangeDelegate(ControlType type);
    public static ControlChangeDelegate OnControlTypeChange;
    public static PlayerController Instance { get; private set; }
    public ControlType CurrentControls { get; private set; }

    public enum ControlType
    {
        Keyboard, Controller
    }
    
    
    private new void Awake()
    {
        base.Awake();
        
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
        
        AnimationPath = "Player";
        
        healthBar = GameObject.FindGameObjectWithTag("HealthUI");
        SetHealthBar(CurrentHealth);
        transform.position =  LineManager.Instance.SetToLine(gameObject, 0);
        moveAction = GetComponent<PlayerInput>().actions.FindAction("Move");
        DontDestroyOnLoad(gameObject);

        CurrentControls = ControlType.Controller;
        
        LineCooldown *= relativeEarlyEscape;
        HitCooldown *= relativeEarlyEscape;
        AttackCooldown *= relativeEarlyEscape;
    }

    public void OnMoveAction(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        ControlType newControlType =
            ctx.control.path.Contains("Keyboard") ? ControlType.Keyboard : ControlType.Controller;
        
        if (CurrentControls == newControlType) return;
        CurrentControls = newControlType;
        OnControlTypeChange?.Invoke(CurrentControls);
    }
    
    private void OnDestroy()
    {
        if(Instance == this) Instance = null;
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += ResetPosition;
        StatsController.OnHealthChange += OnHealthChange;
        StatsController.OnAnimSpeedChange += OnAnimSpeedChange;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= ResetPosition;
        StatsController.OnHealthChange -= OnHealthChange;
        StatsController.OnAnimSpeedChange -= OnAnimSpeedChange;
    }
    
    private void OnAnimSpeedChange()
    {
        Anim.speed = Stats.AnimSpeed;
    }

    private void OnHealthChange(int oldMax, bool higher)
    {
        if (!higher)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, Stats.MaxHealth);
            SetHealthBar(CurrentHealth);
        }
        else
        {
            CurrentHealth = (int)((float) CurrentHealth / oldMax * Stats.MaxHealth);
            SetHealthBar(CurrentHealth);
        }
    }
    
    /// <summary>
    /// Resets the Player X position to -4 when a new Scene is Loaded  
    /// </summary>
    /// <param name="s">irrelevant</param>
    /// <param name="m">irrelevant</param>
    private void ResetPosition(Scene s, LoadSceneMode m)
    {
        if (!SceneController.IsInLoading)
        {
            healthBar = GameObject.FindGameObjectWithTag("HealthUI");
            SetHealthBar(CurrentHealth);
        }
 
        Vector3 newPos = transform.position;
        newPos.x = -4;
        transform.position = newPos;
        print($"Reset x-Position to {newPos}");
    }
    
    /// <summary>
    /// Is Called by the PlayerInput Component when the Attack Input is given, to Attack
    /// </summary>
    /// <param name="ctx">The Context of the Triggered Action</param>
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        print("Received AttackInput");

        if (!ActionActive) StartCoroutine(AttackRoutine());
    }
    
    /// <summary>
    /// Is Called by the PlayerInput Component when the LineChangeUp Input is given, to Change the Line
    /// </summary>
    /// <param name="ctx">The Context of the Triggered Action</param>
    public void OnChangeLineUp(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        print("Received ChangeLineUp Input");

        if (!ActionActive) StartCoroutine(LineChangeRoutine(1));
    }
    
    /// <summary>
    /// Is Called by the PlayerInput Component when the LineChangeDown Input is given, to Change the Line
    /// </summary>
    /// <param name="ctx">The Context of the Triggered Action</param>
    public void OnChangeLineDown(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        print("Received ChangeLineDown Input");

        if (!ActionActive) StartCoroutine(LineChangeRoutine(-1));
    }

    public void OnBlock(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !ActionActive) StartBlock();
        if (ctx.canceled) BreakBlock();
    }

    public void OnMaskButton(InputAction.CallbackContext ctx)
    {
        if (SceneController.IsInLoading) return;
        if (!ctx.performed) return;
        Vector2 dir = ctx.ReadValue<Vector2>();
        MaskManager.MaskType mask = dir.x switch
        {
            -1 => MaskManager.MaskType.Damage,
            1 => MaskManager.MaskType.Block,
            _ => dir.y switch
            {
                1 => MaskManager.MaskType.Speed,
                -1 => MaskManager.MaskType.Health,
                _ => MaskManager.MaskType.None
            }
        };
        if (mask == MaskManager.MaskType.None) Debug.LogError("Mask None is not allowed in this Context");
        
        if (mask == MaskManager.Instance.CurrentMaskType)
        {
            MaskManager.Instance.Dequip();
            Stats.ChangeMask(MaskManager.MaskType.None);
            maskController.SetFloat(AnimatorMaskParameter, -1);
        }
        else
        {
            if (!MaskManager.Instance.Equip(mask)) return;
            Stats.ChangeMask(mask);
            maskController.SetFloat(AnimatorMaskParameter, (int) mask);
        }
    }
    
    protected override void FixedUpdate()
    {
        Direction = moveAction.ReadValue<float>() * (!ActionActive ? 1f : 0.5f);
        Anim.SetFloat(AnimatorDirection, Direction);
        Rb.velocity = Vector2.right * (!KnockedBack ? Direction * Stats.MovementSpeed : -CurrentKnockBackSpeed);
        CheckFlip();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    { 
        if(other.gameObject.CompareTag("Transition")) SceneController.Instance.LoadNextScene();
    }

    protected override void SetHealthBar(int amount)
    {
        print($"New Health: {amount} / {Stats.MaxHealth}");
        Vector3 newScale = Vector3.one;
        newScale.x = (float) amount / Stats.MaxHealth;
        healthBar.transform.localScale = newScale;
        healthBar.GetComponent<Image>().color = healthGradient.Evaluate(newScale.x);
    }

    protected override IEnumerator HitRoutine()
    {
        ActionActive = true;
        Anim.Play(AnimationPath + "Hit");

        float counter = 0;
        while (counter < HitCooldown)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        ActionActive = false;
    }
    
    protected override void OnNoHealth()
    {
        AudioManager.Instance.PlayAudioEffect(AudioManager.PlayerDeath);
        AudioManager.Instance.PlayAudioEffect(AudioManager.PlayerDeathScream);
        Time.timeScale = 0;
        OnGameOver?.Invoke();
    }

    protected override void PlayHitSound(bool blocked)
    {
        AudioManager.Instance.PlayAudioEffect(!blocked ? AudioManager.PlayerHit : AudioManager.PlayerBlock);
    }

    protected override void PlayPunchSound()
    {
        AudioManager.Instance.PlayAudioEffect(AudioManager.PlayerPunch);
    }

    public void FullHeal()
    {
        CurrentHealth = Stats.MaxHealth;
    }
}