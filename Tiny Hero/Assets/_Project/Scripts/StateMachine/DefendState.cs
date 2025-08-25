using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class DefendState : StateBase
{
    GameObject defendEffect;

    GameObject effect;
    public DefendState(PlayerController player, Animator animator, GameObject defendEffect) : base(player, animator) 
    { 
        this.defendEffect = defendEffect;
    }

    public override void OnEnter()
    {
        animator.CrossFade(DefendHash, crossFadeDuration);
        if (defendEffect != null)
        {
            effect = Object.Instantiate(defendEffect, player.transform.position, Quaternion.identity);
        }

        Debug.Log("Defend State Entered");
    }

    public override void OnUpdate()
    {
        if(effect != null)
        {
            effect.transform.position = player.transform.position;
        }
    }

    public override void OnExit()
    {
        if (effect != null)
        {
            Object.Destroy(effect);
        }
        Debug.Log("Exit Defend State");
    }
}
