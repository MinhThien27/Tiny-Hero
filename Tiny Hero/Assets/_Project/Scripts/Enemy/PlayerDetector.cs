using KBCore.Refs;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] float detectionAngle = 60f; // Angle in degrees for detection (in front of the enemy)
    [SerializeField] float detectionRadius = 10f; // Large circle for close detection
    [SerializeField] float innerDetectionRadius = 5f; // Small circle for close detection
    [SerializeField] float detectionCooldown = 1f;
    [SerializeField] float attackRange = 1f;

    [SerializeField] Enemy Enemy { get; set; } 
    public Transform Player { get; private set; }
    public Health PlayerHealth { get; private set; }

    CountdownTimer detectionTimer;

    IDetectionStrategy detectionStrategy;

    private void Awake()
    {
        Enemy = GetComponent<Enemy>();
        attackRange = Enemy.attackRange; 
        Player = GameObject.FindGameObjectWithTag("Player").transform; 
        PlayerHealth = Player.GetComponent<Health>();
    }

    private void Start()
    {
        detectionTimer = new CountdownTimer(detectionCooldown);
        detectionStrategy = new ConeDetectionStrategy(detectionAngle, detectionRadius, innerDetectionRadius);
    }

    private void Update() => detectionTimer.Tick(Time.deltaTime); //Update the timer

    public bool CanDetectPlayer()
    {
        return detectionTimer.IsRunning || detectionStrategy.Execute(Player, transform, detectionTimer);
    }

    public void SetDetectionStrategy(IDetectionStrategy detectionStrategy)
    {
        this.detectionStrategy = detectionStrategy;
    }

    public bool CanAttackPlayer()
    {
        var directionToPlayer = Player.position - transform.position;
        return directionToPlayer.magnitude <= attackRange;
    }

    void OnDrawGizmos()
    {
        // Draw the detection cone in the Scene view for debugging
        Gizmos.color = Color.red;

        //Draw shpere for detection radius and inner detection radius
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.DrawWireSphere(transform.position, innerDetectionRadius);

        //Calculate our cone directions
        Vector3 leftBoundary = Quaternion.Euler(0, -detectionAngle / 2f, 0) * transform.forward * detectionRadius;
        Vector3 rightBoundary = Quaternion.Euler(0, detectionAngle / 2f, 0) * transform.forward * detectionRadius;

        //Draw the detection cone lines
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}
