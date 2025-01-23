using UnityEngine;

public class StatAmplifier : MonoBehaviour
{
    public int endurance = 1; // Level 1-10
    public int combat = 1;    // Level 1-10
    public int speed = 1;     // Level 1-10

    public SpecialistRoleNpc specialistRole;
    private StatManager statManager;
    // Role-based modifiers
    private float roleMaxStaminaModifier = 0f;
    private float roleStaminaConsumeModifier = 0f;
    private float roleStaminaRecoverModifier = 0f;
    private float roleSpeedModifier = 0f;
    private float roleAccuracyModifier = 0f;
    private float roleHandlingModifier = 0f;
    private float roleStabilityModifier = 0f;
    private float roleReloadSpeedModifier = 0f;
    private float roleDamageModifier = 0f;

    // Base multipliers from stats
    private float baseEnduranceMultiplier;
    private float baseCombatMultiplier;
    private float baseSpeedMultiplier;

    void Start()
    {
        InitializeAmplifiers();
        statManager = GetComponent<StatManager>();
        statManager.OnStatAmplifierChanged();
    }
    
    public void InitializeAmplifiers()
    {
        // Calculate base multipliers
        baseEnduranceMultiplier = 1 + GetAmplifierMultiplier(endurance);
        baseCombatMultiplier = 1 + GetAmplifierMultiplier(combat);
        baseSpeedMultiplier = 1 + GetAmplifierMultiplier(speed);

        // Apply role-based modifiers
        ApplyRoleModifiers();
        statManager?.OnStatAmplifierChanged();
    }

    // Rest of the class remains the same...

    // Method to calculate the percentage increase based on the level
    private float GetAmplifierMultiplier(int level)
    {
        if (level >= 2 && level <= 3) return 0.05f * (level - 1);    // 5% per level
        if (level >= 4 && level <= 6) return 0.10f + 0.07f * (level - 3); // 7% per level
        if (level >= 7 && level <= 9) return 0.31f + 0.04f * (level - 6); // 4% per level
        if (level == 10) return 0.46f + 0.10f;                       // 10% for level 10
        return 0f;
    }

    // Apply role-based stat modifiers
    public void ApplyRoleModifiers()
    {
        // Reset role modifiers
        roleMaxStaminaModifier = 0f;
        roleStaminaConsumeModifier = 0f;
        roleStaminaRecoverModifier = 0f;
        roleSpeedModifier = 0f;
        roleAccuracyModifier = 0f;
        roleHandlingModifier = 0f;
        roleStabilityModifier = 0f;
        roleReloadSpeedModifier = 0f;
        roleDamageModifier = 0f;

        switch (specialistRole)
        {
            case SpecialistRoleNpc.Handicraft:
                roleMaxStaminaModifier = 0.05f;         // +5% MaxStamina
                roleStaminaConsumeModifier = -0.05f;    // -5% StaminaConsume
                break;
            case SpecialistRoleNpc.Maintainance:
                roleMaxStaminaModifier = 0.10f;         // +10% MaxStamina
                break;
            case SpecialistRoleNpc.Network:
                roleMaxStaminaModifier = -0.10f;        // -10% MaxStamina
                roleStaminaConsumeModifier = 0.05f;     // +5% StaminaConsume
                roleStaminaRecoverModifier = -0.05f;    // -5% StaminaRecover
                break;
            case SpecialistRoleNpc.Scavenger:
                roleMaxStaminaModifier = 0.20f;         // +20% MaxStamina
                roleSpeedModifier = 0.10f;              // +10% Speed
                break;
            case SpecialistRoleNpc.Military_training:
                roleMaxStaminaModifier = 0.20f;         // +20% MaxStamina
                roleAccuracyModifier = 0.15f;           // +15% Accuracy
                roleHandlingModifier = 0.15f;           // +15% Handling
                roleStabilityModifier = 0.15f;          // +15% Stability
                roleReloadSpeedModifier = -0.10f;       // -10% Reload Speed (reloads 10% faster)
                roleDamageModifier = 0.07f;             // +7% Damage
                break;
            case SpecialistRoleNpc.Chemical:
                roleMaxStaminaModifier = -0.15f;        // -15% MaxStamina
                break;
            case SpecialistRoleNpc.Doctor:
                // No stat changes
                break;
            case SpecialistRoleNpc.Entertainer:
                roleStaminaConsumeModifier = -0.05f;    // -5% StaminaConsume
                break;
        }
    }

    // Methods to get final multipliers
    public float GetMaxStaminaMultiplier()
    {
        return baseEnduranceMultiplier * (1 + roleMaxStaminaModifier);
    }

    public float GetStaminaConsumeMultiplier()
    {
        return (1 + roleStaminaConsumeModifier);
    }

    public float GetStaminaRecoverMultiplier()
    {
        return baseSpeedMultiplier * (1 + roleStaminaRecoverModifier);
    }

    public float GetSpeedMultiplier()
    {
        return baseSpeedMultiplier * (1 + roleSpeedModifier);
    }

    public float GetAccuracyMultiplier()
    {
        return baseCombatMultiplier * (1 + roleAccuracyModifier);
    }

    public float GetHandlingMultiplier()
    {
        return baseCombatMultiplier * (1 + roleHandlingModifier);
    }

    public float GetStabilityMultiplier()
    {
        return baseCombatMultiplier * (1 + roleStabilityModifier);
    }

    public float GetReloadSpeedMultiplier()
    {
        return (1 + roleReloadSpeedModifier); // Reload speed is modified directly
    }

    public float GetDamageMultiplier()
    {
        return 1 + roleDamageModifier; // Damage increases by role modifier
    }

    public float GetEnduranceMultiplier() => 1 + GetAmplifierMultiplier(endurance);
    public float GetCombatMultiplier() => 1 + GetAmplifierMultiplier(combat);
}
