using Cinemachine;
using KBCore.Refs;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : ValidatedMonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] Rigidbody rb;
    [SerializeField, Self] Animator animator;
    [SerializeField, Anywhere] CinemachineFreeLook freeLookVCam;
    [SerializeField, Anywhere] InputReader input;

    [Header("Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 15f;
    [SerializeField] float smoothTime = 0.2f;

    Transform mainCamera;

    private const float ZeroF = 0f;

    float currentSpeed;
    float velocity;
    Vector3 movement;

    //Animator parameters
    static readonly int Speed = Animator.StringToHash("Speed");

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
    }

    private void Start() => input.EnablePlayerActions();

    private void Update()
    {
        movement = new Vector3(input.Direction.x, 0, input.Direction.y).normalized;
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        //HandleJump();
        HandleMovement();
    }

    private void UpdateAnimator()
    {
        animator.SetFloat(Speed, currentSpeed);
    }

    private void HandleMovement()
    {
        //Rotate movement direction to match camera rotation
        var adjustedDirection = Quaternion.AngleAxis(mainCamera.eulerAngles.y, Vector3.up) * movement;
        if(adjustedDirection.magnitude > ZeroF)
        {
            HandleRotation(adjustedDirection);
            HandleHorizontalMovement(adjustedDirection);
            SmoothSpeed(adjustedDirection.magnitude);
        }
        else
        {
            SmoothSpeed(ZeroF);

            //Reset horizontal velocity for a snappy stop
            rb.velocity = new Vector3(ZeroF, rb.velocity.y, ZeroF);
        }
    }

    private void HandleRotation(Vector3 adjustedDirection)
    {
        //Adjust rotate to match ovement direction
        var targetRotation = Quaternion.LookRotation(adjustedDirection);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
        transform.LookAt(transform.position + adjustedDirection);
    }

    private void HandleHorizontalMovement(Vector3 adjustedDirection)
    {
        //Move player
        Vector3 velocity = adjustedDirection * moveSpeed * Time.fixedDeltaTime;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
    }

    private void SmoothSpeed(float value)
    {
        currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
    }
}
