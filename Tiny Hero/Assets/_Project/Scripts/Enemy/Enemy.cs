using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyType
{
    Melee,
    Ranged
}
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PlayerDetector))]
public class Enemy : Entity 
{
    [SerializeField, Self] NavMeshAgent navMeshAgent;
    [SerializeField, Child] Animator animator;
    [SerializeField, Self] PlayerDetector playerDetector;
    [SerializeField, Self] Health health;

    [Header("Attack Settings")]
    [SerializeField] EnemyType enemyType = EnemyType.Melee;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float wanderRadius = 10f;
    public float attackRange = 1f;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] int attackDamage = 1;

    [Header("Dead Settings")]
    [SerializeField] GameObject deadEffect;

    [HideInInspector] public StateMachine stateMachine;

    public CountdownTimer attackTimer;

    public bool isExplosion = false;

    //public bool IsAlreadyAttacking { get; private set; }

    private void OnValidate() => this.ValidateRefs();

    private void Start()
    {
        attackTimer = new CountdownTimer(attackCooldown);
        stateMachine = new StateMachine();

        var wanderState = new EnemyWanderState(this, animator, navMeshAgent, wanderRadius);
        var chaseState = new EnemyChaseState(this, animator, navMeshAgent, playerDetector.Player);
        var attackState = new EnemyAttackState(this, animator, navMeshAgent, playerDetector.Player, enemyType);
        var hitState = new EnemyHitState(this, animator);
        var dieState = new EnemyDieState(this, animator);

        At(wanderState, chaseState, new FunctionPredicate(() => playerDetector.CanDetectPlayer()));
        At(chaseState, wanderState, new FunctionPredicate(() => !playerDetector.CanDetectPlayer()));

        At(chaseState, attackState, new FunctionPredicate(() => playerDetector.CanAttackPlayer()));
        At(attackState, chaseState, new FunctionPredicate(() => !playerDetector.CanAttackPlayer()));

        Any(hitState, new FunctionPredicate(() => health.IsTakeDamaged));
        

        Any(dieState, new FunctionPredicate(() => health.IsDead || isExplosion));

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

        switch (enemyType)
        {
            case EnemyType.Melee:
                MeleeAttack();
                break;
            case EnemyType.Ranged:
                RangeAttack();
                break;
            default:
                break;
        }
    }

    void MeleeAttack()
    {
        playerDetector.PlayerHealth.TakeDamage(attackDamage);

        //IsAlreadyAttacking = true;
    }

    void RangeAttack()
    {
        if (bulletPrefab == null || playerDetector.Player == null) return;

        GameObject bulletGO = Instantiate(bulletPrefab, transform.position + Vector3.up , Quaternion.identity, transform);
        EnemyBullet bullet = bulletGO.GetComponent<EnemyBullet>();
        bullet.SetTarget(playerDetector.Player);

        //IsAlreadyAttacking = true;
    }

    public void OnEnemyDeath()
    {
        Quaternion rotation = transform.rotation * Quaternion.Euler(-90f, 0f, 0f);
        if (deadEffect != null)
            Instantiate(deadEffect, transform.position, rotation, transform);
        
        Destroy(gameObject, 2f);
    }
}
