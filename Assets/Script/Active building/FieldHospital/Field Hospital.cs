using UnityEngine;

public class FieldHospital : BaseBuilding
{
    private PatienManger patienManger;

    // Public for FieldHospitalUI to read
    public int CurrentActiveCurebed => GetContributionForLevel(upgradeBuilding.currentLevel).curebeds;
    public float CurrentHealingSpeed => GetContributionForLevel(upgradeBuilding.currentLevel).healingSpeed;

    protected override void Start()
    {
        base.Start();
        patienManger = FindObjectOfType<PatienManger>();
    }

    protected override BuildingContribution GetContributionForLevel(int level)
    {
        switch (level)
        {
            case 2:
                return new BuildingContribution { curebeds = 7, discontent = 25f, healingSpeed = 10f };
            default:
                return new BuildingContribution { curebeds = 3, discontent = 20f, healingSpeed = 0f };
        }
    }

    protected override void Update()
    {
        base.Update();
        if (patienManger != null)
            patienManger.UpdateJobs(patienManger.activeHealingHospitalPatient);
    }
}
