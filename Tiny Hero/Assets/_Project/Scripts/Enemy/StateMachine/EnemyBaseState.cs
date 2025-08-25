using UnityEngine;

public class EnemyBaseState : IState
{
    protected readonly Enemy enemy;
    protected readonly Animator animator;

    protected const float crossFadeDuration = 0.1f;
    //Get the animator's hash code for the state
    protected static readonly int IdleHash = Animator.StringToHash("IdleNormal");
    protected static readonly int WalkHash = Animator.StringToHash("WalkFWD");
    protected static readonly int RunHash = Animator.StringToHash("RunFWD");
    protected static readonly int AttackHash = Animator.StringToHash("AttackNormal");
    protected static readonly int AttackRangeHash = Animator.StringToHash("AttackRangeNormal");
    protected static readonly int HitHash = Animator.StringToHash("HitNormal");
    protected static readonly int DieHash = Animator.StringToHash("DieNormal");

    protected EnemyBaseState(Enemy enemy, Animator animator)
    {
        this.enemy = enemy;
        this.animator = animator;
    }
    public virtual void OnEnter() 
    {

    }
    public virtual void OnUpdate() 
    {

    }
    public virtual void OnFixedUpdate() 
    {

    }
    public virtual void OnExit() 
    { 
         
    }
}
