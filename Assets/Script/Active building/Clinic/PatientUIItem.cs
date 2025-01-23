using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PatientUIItem : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image faceImage;
    public TextMeshProUGUI hpText;
    public Slider hpSlider;
    public Button actionButton; // Assign via Inspector

    private int npcId;
    private ClinicUI clinicUIRef;

    public void SetData(string npcName, Sprite faceSprite, float hp)
    {
        if (nameText != null) nameText.text = npcName;
        if (faceImage != null && faceSprite != null) faceImage.sprite = faceSprite;
        if (hpText != null) hpText.text = $"HP: {hp.ToString("F2")}";

       if (hpSlider != null)
        {
            hpSlider.value = hp / 100f; // Assuming max HP = 100

            // Define our colors
            Color redColor;
            Color yellowColor;
            Color greenColor;

            // Parse hex strings into Colors
            ColorUtility.TryParseHtmlString("#80111A", out redColor);
            ColorUtility.TryParseHtmlString("#D4AB15", out yellowColor);
            ColorUtility.TryParseHtmlString("#41CF22", out greenColor);

            // We will use a three-segment gradient:
            // 0% -> Red
            // 50% -> Yellow
            // 100% -> Green

            Color finalColor;

            // Normalize HP percentage
            float hpPercent = hp; // hp is already 0 to 100, if it can exceed 100 adjust accordingly

            if (hpPercent <= 50f)
            {
                // From 0% to 50%: Red to Yellow
                // If hp = 0 -> Red, hp = 50 -> Yellow
                float t = hpPercent / 50f; // t goes from 0 at hp=0 to 1 at hp=50
                finalColor = Color.Lerp(redColor, yellowColor, t);
            }
            else
            {
                // From 50% to 100%: Yellow to Green
                // If hp = 50 -> Yellow, hp = 100 -> Green
                float t = (hpPercent - 50f) / 50f; // t goes from 0 at hp=50 to 1 at hp=100
                finalColor = Color.Lerp(yellowColor, greenColor, t);
            }

            Image fillImage = hpSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = finalColor;
            }
        }

    }

    public void InitializeButton(System.Action onClickAction)
    {
        if (actionButton != null)
        {
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() => onClickAction());
        }
    }
}
