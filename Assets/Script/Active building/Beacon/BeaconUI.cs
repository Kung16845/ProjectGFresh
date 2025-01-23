using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BeaconUI : MonoBehaviour
{
    [Header("Beacon UI Elements")]
    public TextMeshProUGUI riskContributionText;
    public TextMeshProUGUI outpostLimitText;
    public TextMeshProUGUI rewardSpeedText;
    public TextMeshProUGUI npcChangeText;

    [Header("Outpost List UI")]
    public GameObject outpostPrefab;   // Prefab for each outpost entry in the list
    public Transform outpostListContent; // Content parent for outpost entries

    private Beacon beacon;
    private OutpostSystem outpostSystem;

    void Start()
    {
        // Find the necessary components
        beacon = FindObjectOfType<Beacon>();
        outpostSystem = FindObjectOfType<OutpostSystem>();
        UpdateBeaconStats(); // Update the UI on start
        DisplayActiveOutposts(); // Populate the outpost list
    }

    void Update()
    {
        // Continuously update the UI to reflect current stats
        UpdateBeaconStats();
    }

    void UpdateBeaconStats()
    {
        if (beacon != null)
        {
            riskContributionText.text = $"Risk Contribution: {beacon.GetRiskValueBasedOnLevel()}";
            outpostLimitText.text = $"Outpost Limit: {beacon.GetOutpostValueBasedOnLevel()}";
            rewardSpeedText.text = $"Reward Speed: {beacon.GetRewardValueBasedOnLevel()}%";
            npcChangeText.text = $"NPC Encouter Change: {beacon.GetNpcchangeValueBasedOnLevel()}";
        }
    }
    public void InitializeUpgradeData()
    {
        beacon.AssignUpgradeData();
    }
    public void DisplayActiveOutposts()
    {
        // Clear any previous entries
        foreach (Transform child in outpostListContent)
        {
            Destroy(child.gameObject);
        }

        // Populate the list with active outposts
        foreach (var reward in outpostSystem.outpostRewards)
        {
            GameObject outpostEntry = Instantiate(outpostPrefab, outpostListContent);
            TextMeshProUGUI nameText = outpostEntry.GetComponentInChildren<TextMeshProUGUI>();
            UnityEngine.UI.Image iconImage = outpostEntry.transform.GetComponentInChildren<UnityEngine.UI.Image>();

            nameText.text = reward.location.ToString();
            iconImage.sprite = reward.locationIcon;
        }
    }
}
