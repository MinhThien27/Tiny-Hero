using UnityEngine;

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
        //Call Player's move logic
        player.HandleMovement();
    }
}