using UnityEngine;
public class EnemyHitState : EnemyBaseState
{
    float hitDuration = 0.5f;
    float timer;

    public EnemyHitState(Enemy enemy, Animator animator) : base(enemy, animator) { }

    public override void OnEnter()
    {
        animator.CrossFade(HitHash, crossFadeDuration);
        timer = hitDuration;
    }

    public override void OnUpdate()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            enemy.stateMachine.RevertToPreviousState();
        }
    }
}
