using UnityEngine;
using UnityEngine.UI;

public class BombChargeUI : MonoBehaviour
{
    [Header("References")]
    public ShootBomb shootBomb; // Reference to the ShootBomb script
    public Slider chargeSlider; // UI Slider to track charge time

    private void Start()
    {
        // Ensure chargeSlider is assigned
        if (chargeSlider == null)
        {
            Debug.LogError("Charge Slider is not assigned in the inspector!");
            return;
        }

        // Dynamically find the ShootBomb component if not assigned
        if (shootBomb == null)
        {
            shootBomb = FindObjectOfType<ShootBomb>();
            if (shootBomb == null)
            {
                Debug.LogError("ShootBomb component not found in the scene!");
                return;
            }
        }

        // Initialize slider values
        chargeSlider.minValue = 0;
        chargeSlider.maxValue = shootBomb.maxChargeTime;
        chargeSlider.value = 0;

        // Initially hide the slider
        chargeSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (shootBomb == null || chargeSlider == null) return;

        // Show the slider only when charging
        bool isCharging = shootBomb.hasPressedG;
        chargeSlider.gameObject.SetActive(isCharging);

        if (isCharging)
        {
            // Update the slider value to match the current charge time
            chargeSlider.value = shootBomb.currentChargeTime;
        }
    }
}
