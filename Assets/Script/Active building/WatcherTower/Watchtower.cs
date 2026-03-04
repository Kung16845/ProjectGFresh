using UnityEngine;

public class Watchtower : BaseBuilding
{
    protected override BuildingContribution GetContributionForLevel(int level)
    {
        // Fixed values, no upgrade levels
        return new BuildingContribution
        {
            riskReduction = 40f,
            npcChange = 7f
        };
    }
}
