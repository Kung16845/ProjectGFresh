using UnityEngine;

public class Clinic : BaseBuilding
{
    private UImanger uImanger;
    private UpgradeUi upgradeUi;
    private PatienManger patienManger;

    // Public for ClinicUI to read
    public int CurrentActiveCurebed => GetContributionForLevel(upgradeBuilding.currentLevel).curebeds;

    protected override void Start()
    {
        base.Start();
        uImanger = FindObjectOfType<UImanger>();
        patienManger = FindObjectOfType<PatienManger>();
    }

    protected override BuildingContribution GetContributionForLevel(int level)
    {
        switch (level)
        {
            case 2:
                return new BuildingContribution { curebeds = 2, discontent = 10f };
            case 3:
                return new BuildingContribution { curebeds = 4, discontent = 8f };
            default:
                return new BuildingContribution { curebeds = 1, discontent = 5f };
        }
    }

    protected override void Update()
    {
        base.Update();
        if (patienManger != null)
            patienManger.UpdateJobs(patienManger.activeHealingClinicPatient);
    }

    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.ClinicUI);
            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                uImanger.DisableUIPanel(UImanger.UIPanel.ClinicUpgradeUI);
            }
        }
    }

    public void AssignUpgradeData()
    {
        upgradeUi = FindObjectOfType<UpgradeUi>();
        upgradeUi.Initialize(upgradeBuilding);
    }
}
