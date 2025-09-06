using UnityEngine;
using CustomInspector;

[CreateAssetMenu(menuName = "Game/Enemy Data")]
public class EnemySO : ScriptableObject
{
    public string id;
    public GameObject prefab;
    public EnemyType enemyType;

    private bool IsRanged() => enemyType == EnemyType.Ranged;
    [ShowIf("IsRanged")]
    public GameObject bulletPrefab;

    public GameObject deadEffect;

    [Header("Stats")]
    public int maxHP = 10;
    public int attackDamage = 1;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public float wanderRadius = 10f;
}