using KBCore.Refs;
using UnityEngine;

public class PlayerHealth : Health
{
    [SerializeField] PlayerController player;
    public override bool CanTakeDamage => base.CanTakeDamage && !player.isDefending;
    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerController>();
    }
}
