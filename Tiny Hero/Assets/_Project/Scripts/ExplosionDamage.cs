using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    [Header("Explosion Settings")]
    public LayerMask enemyLayer;
    public SkillSO skillData;
    public float explosionRadius = 1f;

    private void Start()
    {
        StartCoroutine(DoSlashAttack());
    }

    public IEnumerator DoSlashAttack()
    {
        yield return new WaitForSeconds(1f); //delay before apply damage, wait for animation to play

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Health health))
            {
                health.TakeDamage(skillData.damage);
            }
        }
    }

    // Debug hiển thị phạm vi slash trong Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, explosionRadius); 
    }

}
