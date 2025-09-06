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
        boxHalfExtents = new Vector3(1f, 2f, skillData.range * 0.5f);
        StartCoroutine(DoSlashAttack());
    }

    public IEnumerator DoSlashAttack()
    {
        yield return new WaitForSeconds(1f); // delay animation
        Vector3 boxCenter = transform.position + transform.forward * (skillData.range * 0.5f);

        Collider[] hits = Physics.OverlapBox(boxCenter, boxHalfExtents, transform.rotation, enemyLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Health health))
            {
                health.TakeDamage(skillData.damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (skillData == null) return;

        Gizmos.color = Color.blue;
        Vector3 boxCenter = transform.position + transform.forward * (skillData.range * 0.5f);
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2f);
    }

}


