using UnityEngine;

public interface ISkill
{
    string Name { get; }
    int AnimationHash { get; }
    bool hasDuration { get; } // Optional: Duration of the skill effect
    int ManaCost { get; }
    float CooldownTime { get; } // Optional: Cooldown time for the skill
    GameObject SpawnedEffect { get; }
    bool CanCast(PlayerController player);
    void Activate(PlayerController player);
    void Deactivate();
}