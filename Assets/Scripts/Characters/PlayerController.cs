using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : Character
{
    private static readonly int AnimatorMaskParameter = Animator.StringToHash("MaskInt");
    
    //ComponentReferences
    private InputAction moveAction;
    private GameObject healthBar;
    private Animator maskController;
    [SerializeField] private Gradient healthGradient;
    //Params
    [SerializeField] [Range(0f, 1f)] private float relativeEarlyEscape;
    //Temps
    // Publics
    public delegate void GameOverDelegate();
    public static GameOverDelegate OnGameOver;

    private new void Awake()
    {
        base.Awake();
        AnimationPath = "Player";
        
        healthBar = GameObject.FindGameObjectWithTag("HealthUI");
        SetHealthBar(CurrentHealth);
        transform.position =  LineManager.Instance.SetToLine(gameObject, 0);
        moveAction = GetComponent<PlayerInput>().actions.FindAction("Move");
        maskController = transform.GetChild(1).GetChild(0).GetComponent<Animator>();
        DontDestroyOnLoad(gameObject);

        LineCooldown *= relativeEarlyEscape;
        HitCooldown *= relativeEarlyEscape;
        AttackCooldown *= relativeEarlyEscape;
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
            if (CurrentHealth == oldMax) CurrentHealth += Stats.MaxHealth - oldMax;
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
        if (!ctx.performed) return;
        Vector2 dir = ctx.ReadValue<Vector2>();
        int mask = dir.x switch
        {
            -1 => 0,
            1 => 2,
            _ => dir.y switch
            {
                1 => 1,
                -1 => 3,
                _ => -1
            }
        };
        if (mask == -1) throw new Exception("Got Wrong Mask input");
        MaskUIController.Instance.SetMaskActive(mask);
        Stats.ChangeMask(mask);
        maskController.SetInteger(AnimatorMaskParameter, mask);
    }
    
    protected override void FixedUpdate()
    {
        Direction = moveAction.ReadValue<float>() * (!ActionActive ? 1f : 0.5f);
        Anim.SetFloat(AnimatorDirection, Direction);
        Rb.velocity = Vector2.right * (!KnockedBack ? Direction * Stats.MovementSpeed : -CurrentKnockBackSpeed);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    { 
        if(other.gameObject.CompareTag("Transition")) GameManager.LoadNextScene();
    }

    protected override void SetHealthBar(int amount)
    {
        print($"New Health: {amount} / {Stats.MaxHealth}");
        Vector3 newScale = Vector3.one;
        newScale.x = (float) amount / Stats.MaxHealth;
        healthBar.transform.localScale = newScale;
        healthBar.GetComponent<Image>().color = healthGradient.Evaluate(newScale.x);
    }

    protected override void OnNoHealth()
    {
        AudioManager.Instance.PlayAudioEffect(AudioManager.PlayerDeath);
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
}