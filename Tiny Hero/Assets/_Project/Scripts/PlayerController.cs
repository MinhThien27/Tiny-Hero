using Cinemachine;
using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerController : ValidatedMonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] Rigidbody rb;
    [SerializeField, Self] GroundChecker groundChecker;
    [SerializeField, Self] Animator animator;
    [SerializeField, Anywhere] CinemachineFreeLook freeLookVCam;
    [SerializeField, Anywhere] InputReader input;

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
    [SerializeField] float dashForce = 10f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField] float dashDuration = 0.2f;

    [Header("AttackSetting")]
    [SerializeField] float attackCooldown = 0.5f;
    [SerializeField] float attackDistance = 1f;
    [SerializeField] int attackDamage = 10;

    Transform mainCamera;

    private const float ZeroF = 0f;

    float currentSpeed;
    float velocity;
    float jumpVelocity;
    float dashVelocity = 1f;

    Vector3 movement;

    List<Timer> timers;
    CountdownTimer jumpTimer;
    CountdownTimer jumpCooldownTimer;
    CountdownTimer dashTimer;
    CountdownTimer dashCooldownTimer;
    CountdownTimer attackTimer;

    StateMachine stateMachine;

    //Animator parameters
    static readonly int Speed = Animator.StringToHash("Speed");
    private void OnEnable()
    {
        input.Jump += OnJump;
        input.Dash += OnDash;
        input.Attack += OnAttack;
    }


    private void OnDisable()
    {
        input.Jump -= OnJump;
        input.Dash -= OnDash;
        input.Attack -= OnAttack;
    }


    //private void OnJump(bool performed)
    //{
    //    if(performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && groundChecker.IsGrounded)
    //    {
    //        jumpTimer.Start();
    //    }
    //    else if(!performed && jumpTimer.IsRunning)
    //    {
    //        jumpTimer.Stop();
    //    }
    //}
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

        SetupTimers();
        SetupStateMachine();
    }

    private void SetupStateMachine()
    {
        //Statemachine setup
        stateMachine = new StateMachine();

        //Declare states
        var locomotionState = new LocomotionState(this, animator);
        var jumpState = new JumpState(this, animator);
        var dashState = new DashState(this, animator);
        var attackState = new AttackState(this, animator);

        //-----Define transitions-----//
        //locomotionState -> jumpState
        At(locomotionState, jumpState, new FunctionPredicate(() => jumpTimer.IsRunning));

        //locomotionState -> dashState
        At(locomotionState, dashState, new FunctionPredicate(() => dashTimer.IsRunning));
        //jumpState -> dashState
        At(jumpState, dashState, new FunctionPredicate(() => dashTimer.IsRunning && !jumpTimer.IsRunning));

        //locomotionState -> attackState
        At(locomotionState, attackState, new FunctionPredicate(() => attackTimer.IsRunning));

        //Any state -> locomotionState if not jumping or dashing
        Func<bool> ReturnLocomotionState = () =>  !dashTimer.IsRunning
                                                  && !jumpTimer.IsRunning
                                                  && groundChecker.IsGrounded
                                                  && !attackTimer.IsRunning;
        Any(locomotionState, new FunctionPredicate(ReturnLocomotionState));


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
            //Reset dash velocity to 1 after dash ends
            dashVelocity = 1f;
            dashCooldownTimer.Start();
        };

        attackTimer = new CountdownTimer(attackCooldown);

        timers = new(5) { jumpTimer, jumpCooldownTimer, dashTimer, dashCooldownTimer, attackTimer };
    }

    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);

    void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    private void Start() => input.EnablePlayerActions();

    private void Update()
    {
        movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
        
        HandleTimer();

        stateMachine.Update();

        UpdateAnimator();
    }
    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    private void OnJump(bool performed)
    {
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
        
        if (groundChecker.IsGrounded && jumpCount >= maxJumpTimes)
        {
            Debug.Log($"Grounded: {groundChecker.IsGrounded}");
            jumpCount = 0;
            Debug.Log("Reset Jump Count to 0");
        }
    }

    private void OnDash(bool performed)
    {
        if (performed && !dashTimer.IsRunning && !dashCooldownTimer.IsRunning)
        {
            dashTimer.Start();
        }
        else if (!performed && dashTimer.IsRunning)
        {
            dashTimer.Stop();
        }
    }

    private void OnAttack()
    {
        if (!attackTimer.IsRunning)
        {
            attackTimer.Start();
        }
    }

    public void Attack()
    {
        Vector3 attackPos = transform.position + transform.forward;
        Collider[] hitEnemies = Physics.OverlapSphere(attackPos, attackDistance);

        foreach (var enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<Health>().TakeDamage(attackDamage);
            }
        }
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
        //If not jumping and grounded, keep jump velocity at zero
        if (!jumpTimer.IsRunning && groundChecker.IsGrounded)
        {
            jumpVelocity = ZeroF;
            jumpTimer.Stop();
            return;
        }

        if (!jumpTimer.IsRunning) 
        {
            //Gravity take over
            jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
        }

        //Apply velocity
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


    void HandleRotation(Vector3 adjustedDirection)
    {
        // Adjust rotation to match movement direction
        var targetRotation = Quaternion.LookRotation(adjustedDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void SmoothSpeed(float value)
    {
        currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
    }
}
