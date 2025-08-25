using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDieState : EnemyBaseState
{
    public EnemyDieState(Enemy enemy, Animator animator) : base(enemy, animator)
    {

    }
    public override void OnEnter()
    {
        animator.CrossFade(DieHash, crossFadeDuration);
    }
    public override void OnExit()
    {
        
    }

}
