using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon/WeaponSO", order = 0)]
public class WeaponSO : BaseItemSO
{
    public WeaponType weaponType;
    public int damage = 10;
    public float range = 2f;
    public float attackSpeed = 1f;

    // Hash animation
    public string animationTrigger;
    public int AnimationHash => Animator.StringToHash(animationTrigger);
}
public enum WeaponType
{
    Sword,
    Bow,
    //Staff,
    //Dagger,
    Arrow,
    Shield
}