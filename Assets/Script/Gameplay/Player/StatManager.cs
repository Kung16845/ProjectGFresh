using UnityEngine;

public class StatManager : MonoBehaviour
{
    // Base stats
    public float baseSpeed = 2f;
    public float baseSprintSpeed = 4f;
    public float baseMaxStamina = 100f;
    public float baseStaminaRecoverSpeed = 10f;
    public float baseStaminaConsumeSpeed = 15f;
    public float baseDamage = 10f;
    public float baseHandling = 50f;
    public float baseAccuracy = 50f;
    public float baseStability = 50f;
    private float movementHandlingPenalty = 1f;
    private float sprintHandlingPenalty = 1f;
    // Final stats after applying all modifiers
    public float speed { get; private set; }
    public float sprintSpeed { get; private set; }
    public float maxStamina { get; private set; }
    public float staminaRecoverSpeed { get; private set; }
    public float staminaConsumeSpeed { get; private set; }
    public float damage { get; private set; }
    public float handling { get; private set; }
    public float accuracy { get; private set; }
    public float stability { get; private set; }
    
    public CaliberType caliberType { get; private set; }

    // Modifiers from items and roles
    [Header("Actualstat")]
    public float speedModifier = 1f;
    public float sprintSpeedModifier = 1f;
    public float maxStaminaModifier = 1f;
    public float staminaRecoverSpeedModifier = 1f;
    public float staminaConsumeSpeedModifier = 1f;
    public float damageModifier = 1f;
    public float handlingModifier = 1f;
    public float accuracyModifier = 1f;
    public float stabilityModifier = 1f;

    // References to modifier sources
    private ArmourEquip armourEquip;
    private BackpackEquip backpackEquip;
    private Weapon weapon;
    private StatAmplifier statAmplifier;

    void Awake()
    {
        // Get references to modifier sources
        armourEquip = GetComponent<ArmourEquip>();
        backpackEquip = GetComponent<BackpackEquip>();
        weapon = GetComponent<Weapon>();
        statAmplifier = GetComponent<StatAmplifier>();
    }

    void Start()
    {
        RecalculateStats();
    }

    public void RecalculateStats()
    {
        // Reset modifiers
        speedModifier = 1f;
        sprintSpeedModifier = 1f;
        maxStaminaModifier = 1f;
        staminaRecoverSpeedModifier = 1f;
        staminaConsumeSpeedModifier = 1f;
        damageModifier = 1f;
        handlingModifier = 1f;
        accuracyModifier = 1f;
        stabilityModifier = 1f;

        // Apply modifiers from ArmourEquip
        if (armourEquip != null)
        {
            speedModifier *= armourEquip.GetSpeedModifier();
            maxStaminaModifier *= armourEquip.GetStaminaModifier();
            damageModifier *= armourEquip.GetDamageModifier();
        }

        // Apply modifiers from BackpackEquip
        if (backpackEquip != null)
        {
            speedModifier *= backpackEquip.GetSpeedModifier();
            staminaRecoverSpeedModifier *= backpackEquip.GetStaminaRecoverSpeedModifier();
        }

        // Apply modifiers from StatAmplifier
        if (statAmplifier != null)
        {
            speedModifier *= statAmplifier.GetSpeedMultiplier();
            maxStaminaModifier *= statAmplifier.GetMaxStaminaMultiplier();
            staminaRecoverSpeedModifier *= statAmplifier.GetStaminaRecoverMultiplier();
            staminaConsumeSpeedModifier *= statAmplifier.GetStaminaConsumeMultiplier();
            damageModifier *= statAmplifier.GetDamageMultiplier();
            handlingModifier *= statAmplifier.GetHandlingMultiplier();
            accuracyModifier *= statAmplifier.GetAccuracyMultiplier();
            stabilityModifier *= statAmplifier.GetStabilityMultiplier();
        }

        // Apply modifiers from Weapon
        if (weapon != null)
        {
            caliberType = weapon.GetCaliberType();
            handlingModifier *= weapon.GetHandlingModifier();
            accuracyModifier *= weapon.GetAccuracyModifier();
            stabilityModifier *= weapon.GetStabilityModifier();
            damageModifier *= weapon.GetDamageModifier();
            movementHandlingPenalty = weapon.GetMovementHandlingPenalty();
            sprintHandlingPenalty = weapon.GetSprintHandlingPenalty();
        }
        else
        {
            movementHandlingPenalty = 1f;
            sprintHandlingPenalty = 1f;
        }
        // Calculate final stats
        speed = (baseSpeed * speedModifier) * movementHandlingPenalty;
        sprintSpeed = (baseSprintSpeed * speedModifier) * movementHandlingPenalty; // Assuming sprint speed uses the same modifier
        maxStamina = baseMaxStamina * maxStaminaModifier;
        staminaRecoverSpeed = baseStaminaRecoverSpeed * staminaRecoverSpeedModifier;
        staminaConsumeSpeed = baseStaminaConsumeSpeed * staminaConsumeSpeedModifier;
        damage = baseDamage * damageModifier;
        handling = baseHandling * handlingModifier;
        accuracy = baseAccuracy * accuracyModifier;
        stability = baseStability * stabilityModifier;

        // Notify other components about stat changes
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.OnStatsChanged();
        }

        Weapon weaponComponent = GetComponent<Weapon>();
        if (weaponComponent != null)
        {
            weaponComponent.OnStatsChanged();
        }
    }

    // Methods to be called by other scripts when their modifiers change
    public void OnArmourStatsChanged()
    {
        RecalculateStats();
    }

    public void OnBackpackStatsChanged()
    {
        RecalculateStats();
    }

    public void OnWeaponStatsChanged()
    {
        RecalculateStats();
    }

    public void OnStatAmplifierChanged()
    {
        RecalculateStats();
    }
}
