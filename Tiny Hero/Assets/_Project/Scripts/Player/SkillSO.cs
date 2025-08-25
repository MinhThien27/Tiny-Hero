using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Data", menuName = "Skill/SkillSO", order = 0)]
public class SkillSO : ScriptableObject
{
    public SkillType skillType;
    public GameObject effectPrefab;
    public int manaCost = 10;
    public float cooldown = 1f;
    public float duration = 2f;
    public float delayDamage = 0.5f;
    public int damage = 10;
    public float range = 2f;

    // Hash animation cho skill này
    public string animationTrigger;
    public int AnimationHash => Animator.StringToHash(animationTrigger);
}

public enum SkillType
{
    Q,
    E,
    R
}