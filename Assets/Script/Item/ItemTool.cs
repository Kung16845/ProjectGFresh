using System.Collections.Generic;
using UnityEngine;

public class ItemTool : ItemClass
{   
    [Header("Stat Tool")]
    public string role;

    public override Dictionary<string, float> GetStats()
    {
        return new Dictionary<string, float>
        {
            { "rarityItem", rarityItem }
        };
    }

    public override Dictionary<string, float> GetMaxStatValues()
    {
        return new Dictionary<string, float>
        {
            { "rarityItem", 5 }
        };
    }
}
