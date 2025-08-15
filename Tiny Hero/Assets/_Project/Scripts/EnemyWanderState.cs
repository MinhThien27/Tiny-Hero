using UnityEngine;
using UnityEngine.AI;

public class EnemyWanderState : EnemyBaseState
{
    private NavMeshAgent agent;
    private Vector3 startPosition;
    private float wanderRadius;

    public EnemyWanderState(Enemy enemy, Animator animator, NavMeshAgent agent ,float wanderRadius) : base(enemy, animator) 
    { 
        this.agent = agent;
        this.startPosition = enemy.transform.position;
        this.wanderRadius = wanderRadius;
    }
    public override void OnEnter()
    {
        animator.CrossFade(WalkHash, crossFadeDuration);
    }
    public override void OnUpdate()
    {
        if (HasReachedDestination())
        {
            //Find new destination within the wander radius
            Vector3 randomDestination = startPosition + Random.insideUnitSphere * wanderRadius;
            randomDestination.y = startPosition.y; // Keep the y position constant
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDestination, out hit, wanderRadius, 1);
            var finalDestination = hit.position;
            
            agent.SetDestination(finalDestination);
        }
    }

    private bool HasReachedDestination()
    {
        return !agent.pathPending 
               && agent.remainingDistance <= agent.stoppingDistance 
               && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
    }

    public override void OnFixedUpdate()
    {
        // Handle movement logic here
    }
    public override void OnExit()
    {
        // Cleanup if necessary
    }
}