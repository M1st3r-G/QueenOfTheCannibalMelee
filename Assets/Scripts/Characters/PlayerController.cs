using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : Character
{
    //ComponentReferences
    private InputAction moveAction;
    private GameObject healthbar;
    [SerializeField] private Gradient healthGradient;
    //Params
    [SerializeField] [Range(0f, 1f)] private float blockDamageModifier;
    [SerializeField] [Range(0f, 1f)] private float relativeEarlyEscape;
    //Temps
    private bool isBlocking;
    // Publics
    public delegate void GameOverDelegate();
    public static GameOverDelegate OnGameOver;
    
    private new void Awake()
    {
        base.Awake();
        AnimationPath = "Player";
        
        healthbar = GameObject.FindGameObjectWithTag("HealthUI");
        SetHealthbar(CurrentHealth);
        transform.position =  LineManager.Instance.SetToLine(gameObject, 0);
        moveAction = GetComponent<PlayerInput>().actions.FindAction("Move");
        DontDestroyOnLoad(gameObject);

        LineCooldown *= relativeEarlyEscape;
        HitCooldown *= relativeEarlyEscape;
        AttackCooldown *= relativeEarlyEscape;
        BlockCooldown *= relativeEarlyEscape;
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += ResetPosition;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= ResetPosition;
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
        if (!ctx.performed) return;
        print("Blocked");

        if (!ActionActive) StartCoroutine(BlockRoutine());
    }

    protected override void FixedUpdate()
    {
        Direction = !ActionActive ? moveAction.ReadValue<float>() : 0;
        Anim.SetFloat(AnimatorDirection, Direction);
        Rb.velocity = Vector2.right * (Direction * movementSpeed);
    }
    
    protected override void TakeDamage(int amount)
    {
        if (isBlocking) amount = (int)(blockDamageModifier * amount);
        CurrentHealth -= amount;
        SetHealthbar(CurrentHealth);
        AudioManager.Instance.PlayAudioEffect(!isBlocking ? AudioManager.PlayerHit : AudioManager.PlayerBlock);
        print($"Player Took Damage and is now at {CurrentHealth} health");

        if (!isBlocking)
        {
            StopAllCoroutines();
            StartCoroutine(HitRoutine());
        }
        
        if (CurrentHealth > 0) return;
        AudioManager.Instance.PlayAudioEffect(AudioManager.PlayerDeath);
        Time.timeScale = 0;
        OnGameOver?.Invoke();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    { 
        if(other.gameObject.CompareTag("Transition")) GameManager.LoadNextScene();
    }


    private IEnumerator BlockRoutine()
    {
        ActionActive = true;
        Anim.Play(AnimationPath + "Block");
        isBlocking = true;
        Rb.bodyType = RigidbodyType2D.Static;
        
        float counter = 0;
        while (counter < BlockCooldown)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        Rb.bodyType = RigidbodyType2D.Dynamic;
        isBlocking = false;
        ActionActive = false;
    }
    
    private void SetHealthbar(int amount)
    {
        Vector3 newScale = Vector3.one;
        newScale.x = (float) amount / maxHealth;
        healthbar.transform.localScale = newScale;
        healthbar.GetComponent<Image>().color = healthGradient.Evaluate(newScale.x);
    }

    protected override void PlayPunchSound()
    {
        AudioManager.Instance.PlayAudioEffect(AudioManager.PlayerPunch);
    }
}