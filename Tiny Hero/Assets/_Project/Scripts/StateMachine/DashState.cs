using UnityEngine;

public class DashState : StateBase
{
    public DashState(PlayerController player, Animator animator) : base(player, animator)
    {
    }
    public override void OnEnter()
    {
        Debug.Log("Dash State Entered");
        animator.CrossFade(JumpHash, crossFadeDuration);
    }
    public override void OnFixedUpdate()
    {
        //Call player's dash logic and move logic
        player.HandleMovement();
    }
}