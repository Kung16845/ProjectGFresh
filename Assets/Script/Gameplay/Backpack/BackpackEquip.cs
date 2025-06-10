using UnityEngine;

public class BackpackEquip : MonoBehaviour
{
    private UIInventory uiInventory;
    private ItemBackpack currentBackpack;
    private StatManager statManager;
    private int defaultInventorySlotCount;

    void Start()
    {
        uiInventory = FindObjectOfType<UIInventory>();
        statManager = GetComponent<StatManager>();

        if (uiInventory != null)
        {
            uiInventory.OnBackpackChanged += OnBackpackChanged;
        }
    }

    void OnBackpackChanged(ItemBackpack backpack)
    {
        statManager.OnBackpackStatsChanged();
    }

    // Methods to provide modifiers to StatManager
    public float GetSpeedModifier()
    {
        return currentBackpack != null ? currentBackpack.IncreaseSpeed : 1f;
    }

    public float GetStaminaRecoverSpeedModifier()
    {
        return currentBackpack != null ? currentBackpack.StaminaRecoverSpeed : 1f;
    }

    void OnDisable()
    {
        if (uiInventory != null)
        {
            uiInventory.OnBackpackChanged -= OnBackpackChanged;
        }
    }
    void OnDestroy()
    {
        if (uiInventory != null)
        {
            uiInventory.OnBackpackChanged -= OnBackpackChanged;
            uiInventory.npcSelecting.countInventorySlot = defaultInventorySlotCount;
        }
    }
}
