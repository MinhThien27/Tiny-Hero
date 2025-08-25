using Unity.VisualScripting;
using UnityEngine;

public class AttackState : StateBase
{
    public AttackState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        animator.CrossFade(AttackHash, 0f); //Set crossFadeDuaration = 0 to play fullfill attack animation
        player.Attack();
    }

    public override void OnFixedUpdate()
    {
        //player.HandleMovement();
    }

    public override void OnExit()
    {

    }
}
