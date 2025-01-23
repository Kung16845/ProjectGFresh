using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrenade : ItemClass
{   
    [Header("Stat Grenade")]
    public float damage;
    public bool isAOE;
    public float AoeRange;
    public float armTime;
    public float MinimumRange;
    public float Maxrange;
    public string effect;
    public string typeGrende;

    public override Dictionary<string, float> GetStats()
    {
        return new Dictionary<string, float>
        {
            { "damage", damage },
            { "AoeRange", AoeRange },
            { "armTime", armTime },
            { "rarityItem", rarityItem}
        };
    }

    public override Dictionary<string, float> GetMaxStatValues()
    {
        return new Dictionary<string, float>
        {
            { "damage", 500 },
            { "AoeRange", 3 },
            { "armTime", 3 },
            { "rarityItem", 5}
        };
    }
}
