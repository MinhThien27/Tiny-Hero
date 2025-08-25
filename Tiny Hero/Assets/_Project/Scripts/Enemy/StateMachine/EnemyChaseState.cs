using UnityEngine;
using UnityEngine.AI;
public class EnemyChaseState : EnemyBaseState
{
    private NavMeshAgent agent;
    private Transform target;
    public EnemyChaseState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform target) : base(enemy, animator)
    {
        this.agent = agent;
        this.target = target;
    }
    public override void OnEnter()
    {
        animator.CrossFade(RunHash, crossFadeDuration);
    }
    public override void OnUpdate()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }
    public override void OnFixedUpdate()
    {
        // Handle chase logic here
    }
    public override void OnExit()
    {
        // Cleanup if necessary
    }
}