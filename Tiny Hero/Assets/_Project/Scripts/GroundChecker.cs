using System;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private float groundDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    public bool IsGrounded { get; private set; }

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
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * groundDistance, groundDistance);
    }

}
