using System.Collections;
using System.Collections.Generic;
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
        expenditionManager = FindObjectOfType<ExpenditionManager>();

        if (indexEXUI == 1)
        {   
            expenditionManager.uIButtonEXOne = this;
            // Debug.Log("Reset Button One");
            buttonOpen.onClick.RemoveAllListeners();
            buttonOpen.onClick.AddListener(ExpenditionManager.Instance.OpenUIExpenditionInventoryOne);
        }
        else if(indexEXUI == 2)
        {
            expenditionManager.uIButtonEXTwo = this;
            // Debug.Log("Reset Button Two");
            buttonOpen.onClick.RemoveAllListeners();
            buttonOpen.onClick.AddListener(ExpenditionManager.Instance.OpenUIExpenditionInventoryTwo);
        }
    }
    
    public void SetUIButtonEX(Sprite spriteHead, string finishDayHour)
    {
        textDayHourFinish.text = finishDayHour;
        imageHead.sprite = spriteHead;
    }
}
