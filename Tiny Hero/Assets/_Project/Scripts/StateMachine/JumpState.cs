using UnityEngine;

public class JumpState : StateBase
{
    public JumpState(PlayerController player, Animator animator) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        Debug.Log("Jump State Entered");
        animator.CrossFade(JumpHash, crossFadeDuration);
        //player.jumpCount++;
    }

    public override void OnFixedUpdate()
    {
        //Call player's jump logic and move logic
        player.HandleJump();
        player.HandleMovement();
    }
    public override void OnExit()
    {
        Debug.Log("Exit Jump State");
        // Optionally reset any dash-related parameters or states here
        //player.ResetJumpCountIfGrounded();
    }
}
