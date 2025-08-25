using System.Collections;
using UnityEngine;
public class SkillQ : ISkill
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

    public SkillQ(SkillSO data)
    {
        skillData = data;
    }

    public void Activate(PlayerController player)
    {
        runner = player; //to use coroutine

        if (skillData.effectPrefab != null)
        {
            Quaternion rotation = player.transform.rotation * Quaternion.Euler(90f,0f,0f);
            Vector3 spawnPos = player.transform.position + Vector3.up * 0.5f;

            SpawnedEffect = GameObject.Instantiate(
                skillData.effectPrefab,
                spawnPos,
                rotation,
                player.transform
            );
        }

        damageRoutine = runner.StartCoroutine(DamageOverTime(player));
    }

    private IEnumerator DamageOverTime(PlayerController player)
    {
        while (true)
        {
            Collider[] hitEnemies = Physics.OverlapSphere(player.transform.position, skillData.range);
            foreach (var enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    enemy.GetComponent<Health>()?.TakeDamage(skillData.damage);
                }
            }

            yield return new WaitForSeconds(skillData.delayDamage);
        }
    }

    public void Deactivate()
    {
        if (damageRoutine != null && runner != null)
        {
            runner.StopCoroutine(damageRoutine);
            damageRoutine = null;
        }

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





//using System.Collections;
//using UnityEngine;
//public class SkillQ : ISkill
//{
//    public string Name => "SkillQ";
//    public int AnimationHash => Animator.StringToHash("SkillQ");
//    public GameObject SpawnedEffect { get; private set; }

//    public bool hasDuration => true;

//    private GameObject effectPrefab;
//    private MonoBehaviour runner;
//    private Coroutine damageRoutine;

//    public SkillQ(GameObject prefab)
//    {
//        effectPrefab = prefab;
//    }

//    public void Activate(PlayerController player)
//    {
//        // PlayerController inherits from MonoBehaviour, so we can use it to start coroutines
//        runner = player; 

//        if (effectPrefab != null)
//        {
//            Quaternion rotation = player.transform.rotation * Quaternion.Euler(0f, 0f, 0f);
//            Vector3 spawnPos = player.transform.position + Vector3.up * 0.5f;
//            SpawnedEffect = GameObject.Instantiate(effectPrefab, spawnPos, rotation, player.transform);
//        }

//        // Start coroutine to apply damage over time
//        damageRoutine = runner.StartCoroutine(DamageOverTime(player));
//    }

//    private IEnumerator DamageOverTime(PlayerController player)
//    {
//        while (true) // loop to Deactivate
//        {
//            Collider[] hitEnemies = Physics.OverlapSphere(player.transform.position, 2f);
//            foreach (var enemy in hitEnemies)
//            {
//                if (enemy.CompareTag("Enemy"))
//                {
//                    enemy.GetComponent<Health>()?.TakeDamage(player.skillQDamage);
//                    Debug.Log($"Hit enemy: {enemy.name} with SkillQ tick damage");
//                }
//            }

//            yield return new WaitForSeconds(player.skillQDelayDamage);
//        }
//    }

//    public void Deactivate()
//    {
//        if (damageRoutine != null && runner != null)
//        {
//            runner.StopCoroutine(damageRoutine);
//            damageRoutine = null;
//        }

//        if (SpawnedEffect != null)
//        {
//            Object.Destroy(SpawnedEffect);
//            SpawnedEffect = null;
//        }
//    }
//}
