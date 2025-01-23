using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MediumGardenUI : MonoBehaviour
{
    public MediumGarden mediumGarden;
    public InventoryItemPresent inventoryItemPresent;

    // UI elements
    public TextMeshProUGUI modeText;        // Displays whether it's herbal or normal mode
    public TextMeshProUGUI foodGainText;    // Displays current daily yield
    public TextMeshProUGUI yieldStatusText; // Displays yield status
    public GameObject yieldButton;          // Button to increase yield
    public GameObject toggleModeButton;     // Button or GameObject to toggle between herbal and normal

    void Start()
    {
        if (inventoryItemPresent == null)
            inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();

        if (mediumGarden == null)
            mediumGarden = FindObjectOfType<MediumGarden>();

        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        UpdateModeText();
        UpdateFoodGainText();
        UpdateYieldButtonAndText();
        UpdateToggleModeButton(); // New function to manage toggle button's active state
    }

    void UpdateModeText()
    {
        // Display the current mode based on whether herbal is planted and building level
        if (mediumGarden.isHerbalPlanted && mediumGarden.upgradeBuilding.currentLevel >= 2)
        {
            modeText.text = "Mode: Herbal";
        }
        else
        {
            modeText.text = "Mode: Normal";
        }
    }

    void UpdateFoodGainText()
    {
        foodGainText.text = "Current Daily Yield: " + mediumGarden.currentYield;
    }

    void UpdateYieldButtonAndText()
    {
        int seedCount = GetSeedCount();

        if (mediumGarden.yieldduration > 0)
        {
            yieldButton.SetActive(false);
            yieldStatusText.text = "Yield is increased! Remaining time: " + mediumGarden.yieldduration + " days.";
        }
        else if (seedCount >= 2 && mediumGarden.yieldduration == 0)
        {
            yieldButton.SetActive(true);
            yieldStatusText.text = "Yield has not increased.";
        }
        else
        {
            yieldButton.SetActive(false);
            yieldStatusText.text = "Yield has not increased. Require 2 Seeds.";
        }
    }

    // New function to manage the toggleModeButton's active state
    void UpdateToggleModeButton()
    {
        // Determine if toggle is possible based on current mode and building level
        bool canToggle = false;

        if (mediumGarden.isHerbalPlanted)
        {
            // If currently herbal, can always toggle back to normal
            canToggle = true;
        }
        else
        {
            // If currently normal, can toggle to herbal only if level >= 2
            if (mediumGarden.upgradeBuilding.currentLevel >= 2)
            {
                canToggle = true;
            }
        }

        // Set the toggleModeButton active state based on canToggle
        toggleModeButton.SetActive(canToggle);
    }

    int GetSeedCount()
    {
        int seedCount = 0;
        foreach (ItemData item in inventoryItemPresent.listItemsDataBox)
        {
            if (item.idItem == 1020119) // Seed ID
            {
                seedCount = item.count;
                break;
            }
        }
        return seedCount;
    }

    public void IncreaseYield()
    {
        int seedCount = GetSeedCount();

        if (seedCount >= 2)
        {
            mediumGarden.yieldduration = 3; // Set yield duration to 3 days
            UseSeeds(2); // Use 2 seeds
            UpdateUI();
        }
    }

    // Use seeds after increasing yield
    void UseSeeds(int amount)
    {
        foreach (ItemData item in inventoryItemPresent.listItemsDataBox)
        {
            if (item.idItem == 1020119 && item.count >= amount)
            {
                item.count -= amount;
                break;
            }
        }
    }

    public void AssignUpgradeData()
    {
        mediumGarden.AssignUpgradeData();
    }

    public void ToggleMode()
    {
        if (mediumGarden.isHerbalPlanted)
        {
            // Switch to normal
            mediumGarden.isHerbalPlanted = false;
        }
        else
        {
            // Switch to herbal, only if level >= 2
            if (mediumGarden.upgradeBuilding.currentLevel >= 2)
            {
                mediumGarden.isHerbalPlanted = true;
            }
            else
            {
                Debug.Log("Cannot switch to herbal mode. Upgrade building to level 2 first.");
                // Optionally show a message on UI:
                modeText.text = "Cannot switch to herbal mode. Building level must be >= 2.";
            }
        }

        UpdateUI();
    }
}
