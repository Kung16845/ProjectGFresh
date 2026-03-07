using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewZombieData", menuName = "GreenNight/Zombie Data")]
public class ZombieData : ScriptableObject
{
    [Header("Identity")]
    public string zombieName;
    public string costumeTypeCode; // e.g. "01" for Grunt, "08" for Runner, "07" for Charger, etc.

    [Header("Base Stats")]
    public float baseHp = 100f;
    public float baseSpeed = 3f;
    public float baseAttackSpeed = 1f;  // attackTimer interval
    public float baseDamage = 50f;
    public float baseArmourHp = 0f;

    [Header("Tier Scaling (multiply base stats)")]
    public TierData[] tierScaling = new TierData[3];

    [Header("Damage Multipliers")]
    public DamageMultiplierEntry[] damageMultipliers;

    [Header("Allowed Mutations")]
    public bool allowSpike = true;
    public bool allowAcid = true;
    public bool allowExploder = true;
    public bool allowArmourShell = true;

    [Header("Death & Special")]
    public string deadEffect;
    public string specialAbility;

    public float GetHp(int tier) => baseHp * GetTierData(tier).hpMultiplier;
    public float GetSpeed(int tier) => baseSpeed * GetTierData(tier).speedMultiplier;
    public float GetAttackSpeed(int tier) => baseAttackSpeed * GetTierData(tier).attackSpeedMultiplier;
    public float GetDamage(int tier) => baseDamage * GetTierData(tier).damageMultiplier;
    public float GetArmourHp(int tier) => baseArmourHp * GetTierData(tier).armourMultiplier;

    private TierData GetTierData(int tier)
    {
        int index = Mathf.Clamp(tier - 1, 0, tierScaling.Length - 1);
        return tierScaling[index];
    }

    public Dictionary<DamageType, float> GetDamageMultipliers()
    {
        var dict = new Dictionary<DamageType, float>
        {
            { DamageType.HighcalliberBullet, 1f },
            { DamageType.LowcaliberBullet, 1f },
            { DamageType.MediumcaliberBullet, 1f },
            { DamageType.ShotgunPellet, 1f },
            { DamageType.Pulse, 1f },
            { DamageType.Fire, 1f },
            { DamageType.Acid, 1f },
            { DamageType.Explosive, 1f },
            { DamageType.Poison, 1f },
        };

        if (damageMultipliers != null)
        {
            foreach (var entry in damageMultipliers)
            {
                dict[entry.damageType] = entry.multiplier;
            }
        }

        return dict;
    }
}

[System.Serializable]
public struct TierData
{
    public float hpMultiplier;
    public float speedMultiplier;
    public float attackSpeedMultiplier;
    public float damageMultiplier;
    public float armourMultiplier;

    public static TierData Default => new TierData
    {
        hpMultiplier = 1f,
        speedMultiplier = 1f,
        attackSpeedMultiplier = 1f,
        damageMultiplier = 1f,
        armourMultiplier = 1f,
    };
}

[System.Serializable]
public struct DamageMultiplierEntry
{
    public DamageType damageType;
    public float multiplier;
}
