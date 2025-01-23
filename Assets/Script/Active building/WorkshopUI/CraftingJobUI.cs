using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingJobUI : MonoBehaviour
{
    public Image itemIconImage;
    public TextMeshProUGUI itemNameText;
    public Slider progressSlider;
    public TextMeshProUGUI timeRemainingText;

    private CraftingJob craftingJob;

    public void Initialize(CraftingJob job)
    {
        craftingJob = job;

        if (itemIconImage != null)
            itemIconImage.sprite = job.craftingItem.itemIcon;

        if (itemNameText != null)
            itemNameText.text = job.craftingItem.itemName;

        if (progressSlider != null)
        {
            float totalCraftingTime = job.craftingItem.craftingTime / 1000f * 60f;
            progressSlider.minValue = 0f;
            progressSlider.maxValue = totalCraftingTime;
            progressSlider.value = totalCraftingTime - job.timeRemaining;
        }

        if (timeRemainingText != null)
            timeRemainingText.text = FormatTime(job.timeRemaining);
    }

    void Update()
    {
        if (craftingJob != null && !craftingJob.isComplete)
        {
            float totalCraftingTime = craftingJob.craftingItem.craftingTime / 1000f * 60f;

            // Update progress slider
            if (progressSlider != null)
                progressSlider.value = totalCraftingTime - craftingJob.timeRemaining;

            // Update time remaining text
            if (timeRemainingText != null)
                timeRemainingText.text = FormatTime(craftingJob.timeRemaining);
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60F);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
