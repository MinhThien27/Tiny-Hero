using UnityEngine;
public class HitState : StateBase
{
    float hitDuration = 0.5f;
    float timer;
    public HitState(PlayerController player, Animator animator) : base(player, animator)
    {
    }
    public override void OnEnter()
    {
        Debug.Log("Hit State Entered");
        animator.CrossFade(HitHash, 0f);
        timer = hitDuration;
    }
    public override void OnUpdate()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            player.stateMachine.RevertToPreviousState();
        }
    }
    public override void OnFixedUpdate()
    {
        // Handle any movement or logic while in the hit state if necessary
        player.HandleMovement();
    }
    public override void OnExit()
    {
        Debug.Log("Exit Hit State");
    }
}
