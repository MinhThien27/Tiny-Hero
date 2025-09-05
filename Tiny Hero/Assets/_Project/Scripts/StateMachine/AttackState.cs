using Unity.VisualScripting;
using UnityEngine;

public class AttackState : StateBase
{
    WeaponSO currentWeapon;
    public AttackState(PlayerController player, Animator animator, WeaponCollider weapon) : base(player, animator)
    {
        if (weapon != null)
        {
            currentWeapon = weapon.weaponData;
        }
    }

    public override void OnEnter()
    {
        if (currentWeapon == null) return;
        if (currentWeapon.AnimationHash == 0) return;

        animator.CrossFade(currentWeapon.AnimationHash, 0f);
        //animator.CrossFade(AttackHash, 0f); //Set crossFadeDuaration = 0 to play fullfill attack animation
        //player.Attack();
    }

    public override void OnFixedUpdate()
    {
        //player.HandleMovement();
    }

    public override void OnExit()
    {

    }
}
