using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PlayerDetector))]
public class Enemy : Entity 
{
    [SerializeField, Self] NavMeshAgent navMeshAgent;
    [SerializeField, Child] Animator animator;
    [SerializeField, Self] PlayerDetector playerDetector;

    [SerializeField] float wanderRadius = 10f;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] int attackDamage = 1;

    StateMachine stateMachine;

    CountdownTimer attackTimer;

    private void OnValidate() => this.ValidateRefs();

    private void Start()
    {
        attackTimer = new CountdownTimer(attackCooldown);
        stateMachine = new StateMachine();

        var wanderState = new EnemyWanderState(this, animator, navMeshAgent, wanderRadius);
        var chaseState = new EnemyChaseState(this, animator, navMeshAgent, playerDetector.Player);
        var attackState = new EnemyAttackState(this, animator, navMeshAgent, playerDetector.Player);

        At(wanderState, chaseState, new FunctionPredicate(() => playerDetector.CanDetectPlayer()));
        At(chaseState, wanderState, new FunctionPredicate(() => !playerDetector.CanDetectPlayer()));

        At(chaseState, attackState, new FunctionPredicate(() => playerDetector.CanAttackPlayer()));
        At(attackState, chaseState, new FunctionPredicate(() => !playerDetector.CanAttackPlayer()));

        stateMachine.SetState(wanderState);
    }

    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    private void Update()
    {
        stateMachine?.Update();
        attackTimer.Tick(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        stateMachine?.FixedUpdate();
    }

    public void Attack()
    {
        if (attackTimer.IsRunning) return;

        attackTimer.Start();
        playerDetector.PlayerHealth.TakeDamage(attackDamage);
    }
}
