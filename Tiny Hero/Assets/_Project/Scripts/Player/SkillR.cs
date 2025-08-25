using System.Collections;
using UnityEngine;

public class SkillR : ISkill
{

    private SkillSO skillData;
    private MonoBehaviour runner;
    private Coroutine damageRoutine;
    public string Name => skillData.skillType.ToString();
    public int AnimationHash => skillData.AnimationHash;
    public GameObject SpawnedEffect { get; private set; }
    public bool hasDuration => true;
    public int ManaCost => skillData.manaCost;
    public float CooldownTime => skillData.cooldown;

    public SkillR(SkillSO data)
    {
        skillData = data;
    }

    public void Activate(PlayerController player)
    {
        runner = player; //to use coroutine

        Collider[] hits = Physics.OverlapSphere(player.transform.position, skillData.range);

        if (skillData.effectPrefab != null)
        {
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    SpawnedEffect = GameObject.Instantiate(
                        skillData.effectPrefab,
                        hit.transform.position,
                        Quaternion.identity,
                        hit.transform
                    );
                }
            }
        }
    }

    public void Deactivate()
    {
        //No need to deactivate the effect here, as it is handled by the skill's effect prefab
        //if (SpawnedEffect != null)
        //{
        //    Object.Destroy(SpawnedEffect);
        //    SpawnedEffect = null;
        //}
    }

    public bool CanCast(PlayerController player)
    {
        Collider[] hits = Physics.OverlapSphere(player.transform.position, skillData.range);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
                return true;
        }
        return false;
    }
}
