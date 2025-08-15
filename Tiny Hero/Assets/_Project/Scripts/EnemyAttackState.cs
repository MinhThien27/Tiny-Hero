using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : EnemyBaseState
{
    private NavMeshAgent agent;
    private Transform player;
    public EnemyAttackState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) 
        : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
    }
    public override void OnEnter()
    {
        animator.CrossFade(AttackHash, crossFadeDuration);
    }
    public override void OnUpdate()
    {
        agent.SetDestination(player.position);
        enemy.Attack();
    }
    public override void OnFixedUpdate()
    {
        // Handle movement logic here if necessary
    }
    public override void OnExit()
    {
        // Cleanup if necessary
    }
}
