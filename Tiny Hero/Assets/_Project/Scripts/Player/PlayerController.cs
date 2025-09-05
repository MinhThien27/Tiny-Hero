using Cinemachine;
using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
public class PlayerController : ValidatedMonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] Rigidbody rb;
    [SerializeField, Self] GroundChecker groundChecker;
    [SerializeField, Self] Animator animator;
    [SerializeField, Self] public PlayerHealth health;
    [SerializeField, Self] public Mana mana;
    [SerializeField, Anywhere] CinemachineFreeLook freeLookVCam;
    [SerializeField, Anywhere] InputReader input;
    [SerializeField, Anywhere] SkillHUD skillHUD;

    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 15f;
    [SerializeField] float smoothTime = 0.2f;

    [Header("Jump Settings")]
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float jumpCooldown = 0f;
    [SerializeField] float jumpDuration = 0.5f;
    [SerializeField] float gravityMultiplier = 3f;
    [SerializeField] int maxJumpTimes = 2;
    [SerializeField] public int jumpCount = 0;

    [Header("Dash Settings")]
    [SerializeField] ParticleSystem dashParticle;
    [SerializeField] GameObject dashAfterImage;
    [SerializeField] Material dashMaterial;
    [SerializeField] float dashForce = 10f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField] float dashDuration = 0.2f;

    [Header("AttackNormalSetting")]
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] WeaponCollider leftHandWeapon;
    [SerializeField] WeaponCollider rightHandWeapon;
    [SerializeField] WeaponCollider weaponMainAttack;

    AttackState attackState;

    [Header("Skills Settings")]
    public SkillSO skillQData;
    public SkillSO skillEData;
    public SkillSO skillRData;

    private ISkill skillQ;
    private ISkill skillE;
    private ISkill skillR;

    [Header("Defend Settings")]
    [SerializeField] GameObject defendEffect;
    public bool isDefending { get; private set; }
    public bool isCastingSkill => skillQTimer.IsRunning || skillETimer.IsRunning || skillRTimer.IsRunning;
    public bool isAttacking => attackCooldownTimer.IsRunning;

    Transform mainCamera;

    private const float ZeroF = 0f;

    float currentSpeed;
    float velocity;
    float jumpVelocity;
    float dashVelocity = 1f;

    MeshRenderer meshRenderer;
    Material originalMaterial;

    Vector3 movement;

    private SkillState skillQState;
    private SkillState skillEState;
    private SkillState skillRState;

    List<Timer> timers;
    CountdownTimer jumpTimer;
    CountdownTimer jumpCooldownTimer;

    CountdownTimer dashTimer;
    CountdownTimer dashCooldownTimer;

    CountdownTimer attackCooldownTimer;

    CountdownTimer skillQTimer;
    CountdownTimer skillQCooldownTimer;

    CountdownTimer skillETimer;
    CountdownTimer skillECooldownTimer;

    CountdownTimer skillRTimer;
    CountdownTimer skillRCooldownTimer;

    [HideInInspector] public StateMachine stateMachine;
    [SerializeField] public CameraManager cameraManager;

    //Animator parameters
    static readonly int Speed = Animator.StringToHash("Speed");
    //static readonly int AttackStyle = Animator.StringToHash("AttackNormal");

    private void OnEnable()
    {
        input.Jump += OnJump;
        groundChecker.OnGrounded += ResetJumpCountIfGrounded; //Reset jump count when grounded
        input.Dash += OnDash;
        input.Attack += OnAttack;
        input.Defend += OnDefend;
        input.SkillQ += OnSkillQ;
        input.SkillE += OnSkillE;
        input.SkillR += OnSkillR;
    }


    private void OnDisable()
    {
        input.Jump -= OnJump;
        groundChecker.OnGrounded -= ResetJumpCountIfGrounded;
        input.Dash -= OnDash;
        input.Attack -= OnAttack;
        input.Defend -= OnDefend;
        input.SkillQ -= OnSkillQ;
        input.SkillE -= OnSkillE;
        input.SkillR -= OnSkillR;

    }

    private void Awake()
    {
        mainCamera = Camera.main.transform;
        freeLookVCam.Follow = transform;
        freeLookVCam.LookAt = transform;
        freeLookVCam.OnTargetObjectWarped(
            transform,
            transform.position - freeLookVCam.transform.position - Vector3.forward
        );

        rb.freezeRotation = true;

        //weaponCollider = GetComponentInChildren<WeaponCollider>();

        meshRenderer = GetComponentInChildren<MeshRenderer>();
        originalMaterial = meshRenderer != null ? meshRenderer.material : null;

        skillQ = SkillFactory.CreateSkill(skillQData);
        skillE = SkillFactory.CreateSkill(skillEData);
        skillR = SkillFactory.CreateSkill(skillRData);

        SetupTimers();
        SetupStateMachine();

        skillHUD.InitSkillUI();
    }

    private void SetupStateMachine()
    {
        //Statemachine setup
        stateMachine = new StateMachine();

        //Declare states
        attackState = new AttackState(this, animator, weaponMainAttack);
        var locomotionState = new LocomotionState(this, animator);
        var locomotionWeaponState = new locomotionWeaponState(this, animator);
        var jumpState = new JumpState(this, animator);
        var dashState = new DashState(this, animator, dashMaterial);
        var dieState = new DieState(this, animator);
        var hitState = new HitState(this, animator);
        var defendState = new DefendState(this, animator, defendEffect);
        skillQState = new SkillState(this, animator, skillQ);
        skillEState = new SkillState(this, animator, skillE);
        skillRState = new SkillState(this, animator, skillR);

        //-----Define transitions-----//
        //locomotionState -> jumpState
        At(locomotionState, jumpState, new FunctionPredicate(() => jumpTimer.IsRunning));
        At(locomotionWeaponState, jumpState, new FunctionPredicate(() => jumpTimer.IsRunning));

        //locomotionState -> dashState
        At(locomotionState, dashState, new FunctionPredicate(() => dashTimer.IsRunning));
        At(locomotionWeaponState, dashState, new FunctionPredicate(() => dashTimer.IsRunning));

        //locomotionState -> attackState
        At(locomotionState, attackState, new FunctionPredicate(() => attackCooldownTimer.IsRunning));
        At(locomotionWeaponState, attackState, new FunctionPredicate(() => attackCooldownTimer.IsRunning));


        //Any state -> locomotionState if not jumping or dashing
        Func<bool> ReturnLocomotionState = () => !dashTimer.IsRunning
                                                  && !jumpTimer.IsRunning
                                                  && groundChecker.IsGrounded
                                                  && !attackCooldownTimer.IsRunning
                                                  && !health.IsDead
                                                  && !health.IsTakeDamaged
                                                  && !isDefending
                                                  && !skillQTimer.IsRunning
                                                  && !skillETimer.IsRunning
                                                  && !skillRTimer.IsRunning
                                                  && (!leftHandWeapon && !rightHandWeapon);

        Func<bool> ReturnLocomotionWeaponState = () => !dashTimer.IsRunning
                                                  && !jumpTimer.IsRunning
                                                  && groundChecker.IsGrounded
                                                  && !attackCooldownTimer.IsRunning
                                                  && !health.IsDead
                                                  && !health.IsTakeDamaged
                                                  && !isDefending
                                                  && !skillQTimer.IsRunning
                                                  && !skillETimer.IsRunning
                                                  && !skillRTimer.IsRunning
                                                  && (leftHandWeapon || rightHandWeapon);

        Any(locomotionState, new FunctionPredicate(ReturnLocomotionState));
        Any(locomotionWeaponState, new FunctionPredicate(ReturnLocomotionWeaponState));
        Any(hitState, new FunctionPredicate(() => health.IsTakeDamaged && !skillQTimer.IsRunning && !skillETimer.IsRunning && !skillRTimer.IsRunning));
        Any(dieState, new FunctionPredicate(() => health.IsDead));
        Any(defendState, new FunctionPredicate(() => isDefending));

        Any(skillQState, new FunctionPredicate(() => skillQTimer.IsRunning));
        Any(skillEState, new FunctionPredicate(() => skillETimer.IsRunning));
        Any(skillRState, new FunctionPredicate(() => skillRTimer.IsRunning));

        //Set initial state
        stateMachine.SetState(locomotionState);
    }

    private void SetupTimers()
    {
        //Setup Timers
        jumpTimer = new CountdownTimer(jumpDuration);
        jumpCooldownTimer = new CountdownTimer(jumpCooldown);
        jumpTimer.OnTimerStarted += () => jumpVelocity = jumpForce;
        jumpTimer.OnTimerStopped += () => jumpCooldownTimer.Start();

        dashTimer = new CountdownTimer(dashDuration);
        dashCooldownTimer = new CountdownTimer(dashCooldown);
        dashTimer.OnTimerStarted += () => dashVelocity = dashForce;
        dashTimer.OnTimerStopped += () =>
        {
            dashVelocity = 1f;
            dashCooldownTimer.Start();
        };

        skillQTimer = new CountdownTimer(skillQData.duration);
        skillQCooldownTimer = new CountdownTimer(skillQData.cooldown);
        skillQTimer.OnTimerStopped += () => skillQCooldownTimer.Start();
        skillQCooldownTimer.OnTimerStarted += () => StartCoroutine(skillHUD.UpdateCooldownUI(skillHUD.skillQ));

        skillETimer = new CountdownTimer(skillEData.duration);
        skillECooldownTimer = new CountdownTimer(skillEData.cooldown);
        skillETimer.OnTimerStopped += () => skillECooldownTimer.Start();
        skillECooldownTimer.OnTimerStarted += () => StartCoroutine(skillHUD.UpdateCooldownUI(skillHUD.skillE));

        skillRTimer = new CountdownTimer(skillRData.duration);
        skillRCooldownTimer = new CountdownTimer(skillRData.cooldown);
        skillRTimer.OnTimerStopped += () => skillRCooldownTimer.Start();
        //Bug: Unsed skillR but UI still update
        skillRCooldownTimer.OnTimerStarted += () =>
        {
            if (skillR.CanCast(this))
                StartCoroutine(skillHUD.UpdateCooldownUI(skillHUD.skillR));

        };

        attackCooldownTimer = new CountdownTimer(attackCooldown);

        timers = new() { jumpTimer, jumpCooldownTimer, dashTimer, dashCooldownTimer, attackCooldownTimer, skillQTimer, skillQCooldownTimer, skillETimer, skillECooldownTimer, skillRTimer, skillRCooldownTimer};
    }

    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);

    void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    private void Start()
    {
        input.EnablePlayerActions();

        if (skillHUD != null)
        {
            skillHUD.skillQ.skill = skillQ;
            skillHUD.skillQ.timer = skillQCooldownTimer;

            skillHUD.skillE.skill = skillE;
            skillHUD.skillE.timer = skillECooldownTimer;

            skillHUD.skillR.skill = skillR;
            skillHUD.skillR.timer = skillRCooldownTimer;
        }
    }

    private void Update()
    {
        movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
        
        HandleTimer();

        stateMachine.Update();

        UpdateAnimator();

        if (cameraManager.isAimingWithBow )
        {
            AlignToCamera();
        }
    }
    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    private void OnJump(bool performed)
    {
        if (isDefending || isCastingSkill) return;
        Debug.Log("Jump count: " + jumpCount);

        if (performed && jumpCount < maxJumpTimes)
        {
            jumpTimer.Start();
            jumpCount++;
            Debug.Log($"Jump Count: {jumpCount}");
        }
        else if (!performed && jumpTimer.IsRunning)
        {
            jumpTimer.Stop();
        }
    }

    public void ResetJumpCountIfGrounded()
    {
        jumpCount = 0;
        Debug.Log("Reset Jump Count to 0");
    }

    private void OnDash(bool performed)
    {
        if(currentSpeed <= 0.5f) return; 
        if (performed && !dashTimer.IsRunning && !dashCooldownTimer.IsRunning)
        {
            dashTimer.Start();
        }
        else if (!performed && dashTimer.IsRunning)
        {
            dashTimer.Stop();
        }
    }

    public void EquipWeapon(WeaponCollider wpCol)
    {
        switch (wpCol.weaponData.weaponType)
        {
            case WeaponType.Shield:
                leftHandWeapon = wpCol;
                break;

            case WeaponType.Sword:
                rightHandWeapon = wpCol;
                weaponMainAttack = rightHandWeapon;
                break;

            case WeaponType.Bow:
                leftHandWeapon = wpCol;
                rightHandWeapon = null;
                weaponMainAttack = leftHandWeapon;
                break;

            default:
                Debug.LogWarning("Unknown weapon type equipped: " + wpCol.weaponData.weaponType);
                break;
        }

        attackState = new AttackState(this, animator, weaponMainAttack);

        stateMachine.ReplaceTransition(typeof(AttackState), attackState);
    }
    private void OnAttack()
    {
        if (!attackCooldownTimer.IsRunning && weaponMainAttack != null)
        {
            if(weaponMainAttack.weaponData.weaponType == WeaponType.Shield)
            {
                Debug.Log("Can't attack with shield!");
                return;
            }
            attackCooldownTimer.Start();
        }
    }

    public void Attack()
    {
        switch (weaponMainAttack.weaponData.weaponType)
        {
            case WeaponType.Sword:
                Debug.Log("is already hit:" + weaponMainAttack.isAlreadyHit);
                weaponMainAttack.ResetHit();
                break;
            case WeaponType.Bow:
                //TODO: When using bow, use freeVCam to zoom in and shot at the center of the screen
                weaponMainAttack.ResetHit();
                Debug.Log("is using bow");
                weaponMainAttack.Fire();
                break;
        }
    }

    public void StopSkillTimer(ISkill skill)
    {
        if (skill == skillQ) 
        { 
            skillQTimer.Stop(); 
            skillQCooldownTimer.Stop(); //Stop cooldown if skill is interrupted
        }
        else if (skill == skillE)
        {
            skillETimer.Stop();
            skillECooldownTimer.Stop(); //Stop cooldown if skill is interrupted
        }
        else if (skill == skillR)
        {
            skillRTimer.Stop();
            skillRCooldownTimer.Stop(); //Stop cooldown if skill is interrupted
            Debug.Log("There are no enemy in range, skill R interrupted!");
        }
    }

    private void TryCastSkill(ISkill skill, CountdownTimer skillTimer, CountdownTimer cooldownTimer, string keyName)
    {
        if (isCastingSkill)
        {
            Debug.Log($"Can't cast {keyName} while another skill is active!");
            return;
        }

        if (!skillTimer.IsRunning && !cooldownTimer.IsRunning)
        {
            skillTimer.Start();
        }
        else if (!skillTimer.IsRunning && cooldownTimer.IsRunning)
        {
            Debug.Log($"{skill.Name} is cooldown!");
        }
    }

    private void OnSkillQ() => TryCastSkill(skillQ, skillQTimer, skillQCooldownTimer, "Skill Q");

    private void OnSkillE() => TryCastSkill(skillE, skillETimer, skillECooldownTimer, "Skill E");

    private void OnSkillR() => TryCastSkill(skillR, skillRTimer, skillRCooldownTimer, "Skill R");

    public void OnDefend(bool performed)
    {
        if(leftHandWeapon != null && leftHandWeapon.weaponData.weaponType == WeaponType.Shield)
        {
            isDefending = performed;
            return;
        }
        isDefending = false;
    }

    private void HandleTimer()
    {
        foreach (var timer in timers)
        {
            timer.Tick(Time.deltaTime);
        }
    }

    public void HandleJump()
    {
        if (!jumpTimer.IsRunning && groundChecker.IsGrounded)
        {
            jumpVelocity = ZeroF;
            jumpTimer.Stop();
            return;
        }

        if (!jumpTimer.IsRunning) 
        {
            jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
        }

        rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
    }

    private void UpdateAnimator()
    {
        animator.SetFloat(Speed, currentSpeed);
    }

    public void HandleMovement()
    {
        // Rotate movement direction to match camera rotation
        var adjustedDirection = Quaternion.AngleAxis(mainCamera.eulerAngles.y, Vector3.up) * movement;

        if (adjustedDirection.magnitude > ZeroF)
        {
            HandleHorizontalMovement(adjustedDirection);
            HandleRotation(adjustedDirection);
            SmoothSpeed(adjustedDirection.magnitude);
        }
        else
        {
            SmoothSpeed(ZeroF);

            // Reset horizontal velocity for a snappy stop
            rb.velocity = new Vector3(ZeroF, rb.velocity.y, ZeroF);
        }
    }

    void HandleHorizontalMovement(Vector3 adjustedDirection)
    {
        // Move the player
        Vector3 velocity = adjustedDirection * (moveSpeed * dashVelocity * Time.fixedDeltaTime);
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
    }

    public void HandleRotation(Vector3 adjustedDirection)
    {
        // Adjust rotation to match movement direction
        var targetRotation = Quaternion.LookRotation(adjustedDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void AlignToCamera()
    {
        Vector3 forward = mainCamera.forward;
        forward.y = 0;
        if (forward.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.LookRotation(forward),
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void SmoothSpeed(float value)
    {
        currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
    }

    public void OnPlayerDeath()
    {
        StartCoroutine(WaitForPlayerDeathAnimation(1f));
    }

    public IEnumerator WaitForPlayerDeathAnimation(float value)
    {
        yield return new WaitForSeconds(value);
        gameObject.SetActive(false);
    }

    public IEnumerator CreateAfterImage(float duration, int count, float timeBetweenImages)
    {
        if (dashAfterImage == null || dashMaterial == null) yield break;
        for (int i = 0; i < count; i++)
        {
            GameObject afterImage = Instantiate(dashAfterImage, transform.position, transform.rotation);

            Destroy(afterImage, duration);
            yield return new WaitForSeconds(timeBetweenImages);
        }
    }
    public void PlayAfterImage(float duration, int count, float timeBetweenImages)
    {
        StartCoroutine(CreateAfterImage(duration, count, timeBetweenImages));
    }

}
