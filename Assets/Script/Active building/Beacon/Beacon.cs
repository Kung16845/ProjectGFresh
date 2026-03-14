using UnityEngine;

public class Beacon : BaseBuilding
{
    protected override BuildingContribution GetContributionForLevel(int level)
    {
        switch (level)
        {
            case 2:
                return new BuildingContribution
                {
                    riskReduction = 15f,
                    outpostLimit = 3,
                    rewardSpeed = 25f,
                    npcChange = 7f
                };
            default:
                return new BuildingContribution
                {
                    riskReduction = 10f,
                    outpostLimit = 2,
                    rewardSpeed = 0f,
                    npcChange = 5f
                };
        }
    }

    // Public getters for BeaconUI display
    public float GetRiskValueBasedOnLevel() => GetContributionForLevel(upgradeBuilding.currentLevel).riskReduction;
    public int GetOutpostValueBasedOnLevel() => GetContributionForLevel(upgradeBuilding.currentLevel).outpostLimit;
    public float GetRewardValueBasedOnLevel() => GetContributionForLevel(upgradeBuilding.currentLevel).rewardSpeed;
    public float GetNpcchangeValueBasedOnLevel() => GetContributionForLevel(upgradeBuilding.currentLevel).npcChange;
}
