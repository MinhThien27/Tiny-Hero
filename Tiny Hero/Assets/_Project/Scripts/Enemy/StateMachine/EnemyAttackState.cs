using UnityEngine;
using UnityEngine.AI;
public class EnemyAttackState : EnemyBaseState
{
    private NavMeshAgent agent;
    private Transform player;
    private EnemyType enemyType;
    public EnemyAttackState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player, EnemyType type) 
        : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
        this.enemyType = type;
    }
    public override void OnEnter()
    {
        switch(enemyType)
        {
            case EnemyType.Melee:
                animator.CrossFade(AttackHash, crossFadeDuration);
                break;
            case EnemyType.Ranged:
                animator.CrossFade(AttackRangeHash, crossFadeDuration);
                break;
        }
    }
    public override void OnUpdate()
    {
        switch (enemyType)
        {
            case EnemyType.Melee:
                agent.SetDestination(player.position);
                break;

            case EnemyType.Ranged:
                float distanceToPlayer = Vector3.Distance(enemy.transform.position, player.position);

                if (distanceToPlayer > enemy.attackRange)
                {
                    agent.SetDestination(player.position);
                }
                else 
                {
                    agent.SetDestination(enemy.transform.position); 
                    enemy.transform.LookAt(player);
                }
                break;
        }

        //enemy.Attack();
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
