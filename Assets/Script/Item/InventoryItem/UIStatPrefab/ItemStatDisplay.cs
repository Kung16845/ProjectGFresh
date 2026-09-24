using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemStatDisplay : MonoBehaviour
{
    [Header("Item Reference")]
    public ItemClass item;

    [Header("UI Prefabs")]
    public GameObject statPanelPrefab;
    public GameObject statElementPrefab;
    public Transform statPanelParent;

    private GameObject statPanelInstance;
    private StatPanelUI statPanelUI;
    private UIInventory uIInventory;

    private void Start()
    {
        uIInventory = FindFirstObjectByType<UIInventory>();
        if (item == null)
        {
            item = GetComponent<ItemClass>();
            if (item == null)
            {
                Debug.LogWarning("ItemClass component not found on the GameObject.");
            }
        }

        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogWarning("Button component not found on the GameObject.");
        }
    }

    public void OnButtonClick()
    {
        if (statPanelInstance == null)
        {
            ShowStatPanel();
        }
        else
        {
            CloseStatPanel();
        }
    }

    private void ShowStatPanel()
    {
        if (statPanelPrefab == null || item == null) return;

        if (statPanelParent == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                statPanelParent = canvas.transform;
            }
            else
            {
                Debug.LogError("No Canvas found in the scene. Please assign a parent for the stat panel.");
                return;
            }
        }

        statPanelInstance = Instantiate(statPanelPrefab, statPanelParent);
        statPanelUI = statPanelInstance.GetComponent<StatPanelUI>();
        if (statPanelUI == null)
        {
            Debug.LogError("StatPanelUI script not found on the stat panel prefab.");
            return;
        }

        statPanelUI.itemStatDisplay = this;

        if (statPanelUI.itemImage != null)
        {
            statPanelUI.itemImage.sprite = (item.itemIcon != null) ? item.itemIcon : (item.IconSprite != null ? item.IconSprite.sprite : null);
        }

        if (statPanelUI.itemNameText != null)
        {
            statPanelUI.itemNameText.text = item.nameItem;
        }

        Dictionary<string, float> itemStats = item.GetStats();
        Dictionary<string, float> maxStatValues = item.GetMaxStatValues();

        if (statElementPrefab != null && statPanelUI.statContainer != null && itemStats != null)
        {
            foreach (var stat in itemStats)
            {
                string statName = stat.Key;
                float statValueFloat = stat.Value;
                float maxStatValue = (maxStatValues != null && maxStatValues.ContainsKey(statName)) ? maxStatValues[statName] : 100f;

                GameObject statElementInstance = Instantiate(statElementPrefab, statPanelUI.statContainer);
                StatElementUI statElementUI = statElementInstance.GetComponent<StatElementUI>();
                if (statElementUI == null)
                {
                    continue;
                }

                if (statElementUI.statText != null)
                {
                    statElementUI.statText.text = $"{statName}: {statValueFloat}";
                }
                if (statElementUI.statSlider != null)
                {
                    statElementUI.statSlider.maxValue = maxStatValue;
                    statElementUI.statSlider.value = Mathf.Clamp(statValueFloat, 0, maxStatValue);
                }
            }
        }
    }

    private void CloseStatPanel()
    {
        if (statPanelInstance != null)
        {
            Destroy(statPanelInstance);
            statPanelInstance = null;
            statPanelUI = null;
        }
    }

    private void OnDisable()
    {
        CloseStatPanel();
    }

    public void DeletethisItem()
    {
        if (uIInventory != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("UIInventory reference is missing.");
        }
    }
}
