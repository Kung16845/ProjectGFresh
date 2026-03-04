using UnityEngine;

public class Beacon : BaseBuilding
{
    private UImanger uImanger;
    private UpgradeUi upgradeUi;
    private bool upgradeUIDisabled = false;

    protected override void Start()
    {
        base.Start();
        uImanger = FindObjectOfType<UImanger>();
    }

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

    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.BeaconUI);

            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel && !upgradeUIDisabled)
            {
                uImanger.DisableUIPanel(UImanger.UIPanel.BeaconUpgradeUI);
                upgradeUIDisabled = true;
            }
        }
    }

    public void AssignUpgradeData()
    {
        upgradeUi = FindObjectOfType<UpgradeUi>();
        upgradeUi.Initialize(upgradeBuilding);
    }

    // Public getters for BeaconUI display
    public float GetRiskValueBasedOnLevel() => GetContributionForLevel(upgradeBuilding.currentLevel).riskReduction;
    public int GetOutpostValueBasedOnLevel() => GetContributionForLevel(upgradeBuilding.currentLevel).outpostLimit;
    public float GetRewardValueBasedOnLevel() => GetContributionForLevel(upgradeBuilding.currentLevel).rewardSpeed;
    public float GetNpcchangeValueBasedOnLevel() => GetContributionForLevel(upgradeBuilding.currentLevel).npcChange;
}
