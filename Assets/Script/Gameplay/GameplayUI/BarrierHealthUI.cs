using UnityEngine;
using UnityEngine.UI;

public class BarrierHealthUI : MonoBehaviour
{
    public Barrier barrier; // Reference to the Barrier script
    public Slider healthSlider; // Reference to the Slider UI component
      private void Start()
    {
        barrier = FindObjectOfType<Barrier>();
        if (barrier != null)
        {
            // Initialize slider values based on barrier's health
            healthSlider.maxValue = barrier.maxHp;
            healthSlider.value = barrier.currentHp;
        }

        // Add a listener to handle slider value changes
        healthSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void Update()
    {
        if (barrier != null)
        {
            // Update the slider's value to reflect currentHp
            healthSlider.value = barrier.currentHp;
        }
    }

    // This function is triggered when the slider value changes
    private void OnSliderValueChanged(float value)
    {
        if (barrier != null)
        {
            barrier.currentHp = value; // Sync the barrier's currentHp with the slider
        }
    }

    private void OnDestroy()
    {
        // Remove the listener when the object is destroyed
        healthSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }
}
