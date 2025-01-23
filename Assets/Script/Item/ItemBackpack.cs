using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBackpack : ItemClass
{   
    [Header("Stat Backpack")]
    public int slotIncreasing;
    public float StaminaRecoverSpeed;
    public float IncreaseSpeed;
    public string buff;
    public string debuff;
    public override Dictionary<string, float> GetStats()
    {
        return new Dictionary<string, float>
        {
            { "slotIncreasing", slotIncreasing },
            { "StaminaRecoverSpeed", StaminaRecoverSpeed },
            { "IncreaseSpeed", IncreaseSpeed },
            { "rarityItem", rarityItem}
        };
    }

    public override Dictionary<string, float> GetMaxStatValues()
    {
        return new Dictionary<string, float>
        {
            { "slotIncreasing", 6 },
            { "StaminaRecoverSpeed", 2},
            { "IncreaseSpeed", 2 },
            { "rarityItem", 5}
        };
    }
}
