using System;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private float groundDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    public event Action OnGrounded;

    public bool IsGrounded { get; private set; }
    private bool wasGrounded;

    private void Update()
    {
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        IsGrounded = Physics.CheckSphere(
            transform.position,
            groundDistance,
            groundLayer
        );

        //If grounded state changed, invoke the event
        if (IsGrounded && !wasGrounded)
        {
            OnGrounded?.Invoke();
        }
        wasGrounded = IsGrounded;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * groundDistance, groundDistance);
    }

}
