using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SkillE : ISkill
{
    private SkillSO skillData;

    public string Name => skillData.skillType.ToString();
    public int AnimationHash => skillData.AnimationHash;
    public GameObject SpawnedEffect { get; private set; }
    public bool hasDuration => true;
    public int ManaCost => skillData.manaCost;
    public float CooldownTime => skillData.cooldown;

    public SkillE(SkillSO data)
    {
        skillData = data;
    }

    public void Activate(PlayerController player)
    {
        if (skillData.effectPrefab != null)
        {
            Vector3 spawnPos = player.transform.position + Vector3.up * 1f;
            Quaternion rotation = player.transform.rotation * Quaternion.Euler(0f, 0f, 90f);
            SpawnedEffect = GameObject.Instantiate(skillData.effectPrefab, spawnPos, rotation, player.transform);
        }
    }

    public void Deactivate()
    {
        if (SpawnedEffect != null)
        {
            Object.Destroy(SpawnedEffect);
            SpawnedEffect = null;
        }
    }

    public bool CanCast(PlayerController player)
    {
        return true;
    }
}
