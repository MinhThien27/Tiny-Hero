using UnityEngine;

public class DashState : StateBase
{
    ParticleSystem dashParticle;
    float afterImageDuration = 0.2f;
    int afterImageCount = 5;
    float timeBetweenAfterImages = 0.02f;
    public DashState(PlayerController player, Animator animator, Material dashMaterial) : base(player, animator)
    {

    }
    public override void OnEnter()
    {
        Debug.Log("Dash State Entered");

        //Play dash particle effect
        dashParticle?.Play();
        player.PlayAfterImage(afterImageDuration, afterImageCount, timeBetweenAfterImages);

        animator.CrossFade(JumpHash, crossFadeDuration);
    }
    public override void OnFixedUpdate()
    {
        //Call player's dash logic and move logic
        player.HandleMovement();
    }
    public override void OnExit()
    {
        Debug.Log("Exit Dash State");
    }
}
