using UnityEngine;

public class locomotionWeaponState : StateBase
{
    public locomotionWeaponState(PlayerController player, Animator animator) : base(player, animator)
    {
    }
    public override void OnEnter()
    {
        Debug.Log("Locomotion with Weapon State Entered");
        animator.CrossFade(LocomotionWeaponHash, crossFadeDuration);
    }
    public override void OnFixedUpdate()
    {
        player.HandleMovement();
    }
}
public class LocomotionState : StateBase
{
    public LocomotionState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Locomotion State Entered");
        animator.CrossFade(LocomotionHash, crossFadeDuration);
    }

    public override void OnFixedUpdate()
    {
        player.HandleMovement();
    }
}