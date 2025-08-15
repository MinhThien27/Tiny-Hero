using UnityEngine;

public abstract class StateBase : IState
{
    protected readonly PlayerController player;
    protected readonly Animator animator;

    public static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    public static readonly int JumpHash = Animator.StringToHash("Jump");
    public static readonly int DashHash = Animator.StringToHash("Dash");

    //Attack
    public static readonly int AttackHash = Animator.StringToHash("AttackNormal");

    protected const float crossFadeDuration = 0.1f;

    protected StateBase(PlayerController player, Animator animator)
    {
        this.player = player;
        this.animator = animator;
    }
    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnExit()
    {
        Debug.Log("Exit state");
    }
}
