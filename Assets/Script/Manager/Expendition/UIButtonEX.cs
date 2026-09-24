using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonEX : MonoBehaviour
{
    public TextMeshProUGUI textDayHourFinish;
    public Image imageHead;
    public GameObject iconComplete;
    public Button buttonOpen;

    public int indexEXUI;
    public ExpenditionManager expenditionManager;

    private void Start()
    {
        if (expenditionManager == null)
        {
            expenditionManager = ExpenditionManager.Instance != null ? ExpenditionManager.Instance : FindFirstObjectByType<ExpenditionManager>();
        }

        if (expenditionManager != null && buttonOpen != null)
        {
            buttonOpen.onClick.RemoveAllListeners();

            if (indexEXUI == 1)
            {
                expenditionManager.uIButtonEXOne = this;
                buttonOpen.onClick.AddListener(expenditionManager.OpenUIExpenditionInventoryOne);
            }
            else if (indexEXUI == 2)
            {
                expenditionManager.uIButtonEXTwo = this;
                buttonOpen.onClick.AddListener(expenditionManager.OpenUIExpenditionInventoryTwo);
            }
        }
    }

    private void OnDestroy()
    {
        if (buttonOpen != null)
        {
            buttonOpen.onClick.RemoveAllListeners();
        }
    }

    public void SetUIButtonEX(Sprite spriteHead, string finishDayHour)
    {
        if (textDayHourFinish != null)
        {
            textDayHourFinish.text = finishDayHour;
        }

        if (imageHead != null)
        {
            imageHead.sprite = spriteHead;
        }
    }
}
