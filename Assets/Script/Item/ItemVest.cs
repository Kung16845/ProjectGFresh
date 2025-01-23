using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemVest : ItemClass
{   
    [Header("Stat Vest")]
    public float damageIncreasePercent;
    public float speedIncreasePercent;
    public float staminaIncreasePercent;
    public string buff;
    public string debuff;
    public float riskdecrease;
    public float Repairmaterial;
    public float Quality;
    // Start is called before the first frame update
     public override Dictionary<string, float> GetStats()
    {
        return new Dictionary<string, float>
        {
            { "damageIncrease", damageIncreasePercent },
            { "speedIncrease", speedIncreasePercent },
            { "staminaIncrease", staminaIncreasePercent },
            { "rarityItem", rarityItem}
        };
    }

    public override Dictionary<string, float> GetMaxStatValues()
    {
        return new Dictionary<string, float>
        {
            { "damageIncrease", 2 },
            { "speedIncrease", 2 },
            { "staminaIncrease", 2 },
            { "rarityItem", 5}
        };
    }
}
