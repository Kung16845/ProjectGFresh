using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    public Slider staminaSlider; // Reference to the stamina slider
    public void InitializeStaminaSlider(float maxStamina)
    {
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = maxStamina; // Start fully filled
        }
    }
    public void UpdateStaminaSlider(float currentStamina)
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
        }
    }
}
