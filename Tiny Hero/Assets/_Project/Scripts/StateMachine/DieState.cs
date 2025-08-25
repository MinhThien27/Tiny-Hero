using UnityEngine;

public class DieState : StateBase
{
    public DieState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Die State Entered");
        animator.CrossFade(dieHash, crossFadeDuration);
    }
}