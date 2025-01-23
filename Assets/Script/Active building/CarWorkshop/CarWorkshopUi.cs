using TMPro; // For TextMeshPro
using UnityEngine;
using UnityEngine.UI;

public class CarWorkshopUi : MonoBehaviour
{
    public GameObject refillcarprefab;
    public Transform RefillParent;
    public CarWorkshop carWorkshop;
    private Globalstat globalstat;

    void Start()
    {
        globalstat = FindObjectOfType<Globalstat>();
        carWorkshop = FindObjectOfType<CarWorkshop>();
        CreateCarSlots();
    }

    void OnEnable()
    {
        CreateCarSlots();
    }

    public void CreateCarSlots()
    {
        // Clear existing prefabs if any
        foreach (Transform child in RefillParent)
        {
            Destroy(child.gameObject);
        }

        // Maximum car slots based on CarWorkshop level
        int maxCarSlots = (carWorkshop.upgradeBuilding.currentLevel == carWorkshop.upgradeBuilding.maxLevel) ? 2 : 1;

        // Create prefabs for unavailable cars, up to the max slots
        int carsToDisplay = Mathf.Min(globalstat.UnaviableCar, maxCarSlots);

        for (int i = 0; i < carsToDisplay; i++)
        {
            GameObject carSlotObj = Instantiate(refillcarprefab, RefillParent);

            // Find UI elements in the prefab
            Button refillButton = carSlotObj.GetComponentInChildren<Button>();
            Slider fuelSlider = carSlotObj.GetComponentInChildren<Slider>();
            TMP_Text fuelText = carSlotObj.GetComponentInChildren<TMP_Text>(); // Use TMP_Text for TextMeshPro

            // Set up the slider and text
            int requiredFuel = carWorkshop.upgradeBuilding.currentLevel == carWorkshop.upgradeBuilding.maxLevel ? 3 : 4;
            int currentFuel = Mathf.FloorToInt(carWorkshop.buildManager.fuel); // Convert fuel to int

            fuelSlider.maxValue = requiredFuel;
            fuelSlider.value = Mathf.Min(currentFuel, requiredFuel);

            // Update the TextMeshPro text to show fuel status (e.g., "3/4")
            fuelText.text = $"{currentFuel}/{requiredFuel}";

            // Enable or disable the button based on whether there's enough fuel
            refillButton.interactable = currentFuel >= requiredFuel;

            // Add listener to the button
            refillButton.onClick.AddListener(() =>
            {
                TryRefillCar(carSlotObj, fuelSlider, fuelText, requiredFuel);
            });
        }
    }
    public void InitializeUpgradeData()
    {
        carWorkshop.AssignUpgradeData();
    }
    private void TryRefillCar(GameObject carSlotObj, Slider fuelSlider, TMP_Text fuelText, int requiredFuel)
    {
        int currentFuel = Mathf.FloorToInt(carWorkshop.buildManager.fuel); // Ensure fuel is an int

        if (currentFuel >= requiredFuel)
        {
            carWorkshop.AddCartoGlobalstat();

            // Destroy the slot UI since it's now "refueled"
            Destroy(carSlotObj);
        }
        else
        {
            // Not enough fuel, update UI to indicate the issue
            fuelText.text = $"{currentFuel}/{requiredFuel} (Not enough fuel!)";
        }
        CreateCarSlots();
    }
}
