using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SlashDamage : MonoBehaviour
{
    [Header("Slash Settings")]
    public Vector3 boxHalfExtents; 
    public LayerMask enemyLayer;
    public SkillSO skillData;

    private void Start()
    {
        boxHalfExtents = new Vector3(1f, 2f, skillData.range);
        StartCoroutine(DoSlashAttack());
    }

    public IEnumerator DoSlashAttack()
    {
        yield return new WaitForSeconds(1f); //delay before apply damage, wait for animation to play
        Vector3 attackPosition = transform.position + transform.forward;

        Collider[] hits = Physics.OverlapBox(attackPosition, boxHalfExtents, transform.rotation, enemyLayer);

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
        Vector3 boxCenter = transform.position + transform.forward * skillData.range;
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2f);
    }

}


