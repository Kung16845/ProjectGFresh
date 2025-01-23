using UnityEngine;

public class ArmourEquip : MonoBehaviour
{
    private UIInventory uiInventory;
    private ItemVest currentVest;
    private StatManager statManager;

    void Start()
    {
        uiInventory = FindObjectOfType<UIInventory>();
        statManager = GetComponent<StatManager>();

        if (uiInventory != null)
        {
            uiInventory.OnVestChanged += OnVestChanged;
        }
    }

    void OnVestChanged(ItemVest vest)
    {
        currentVest = vest;
        statManager.OnArmourStatsChanged();
    }

    // Methods to provide modifiers to StatManager
    public float GetSpeedModifier()
    {
        return currentVest != null ? currentVest.speedIncreasePercent : 1f;
    }

    public float GetStaminaModifier()
    {
        return currentVest != null ? currentVest.staminaIncreasePercent : 1f;
    }

    public float GetDamageModifier()
    {
        return currentVest != null ? currentVest.damageIncreasePercent : 1f;
    }

    void OnDestroy()
    {
        if (uiInventory != null)
        {
            uiInventory.OnVestChanged -= OnVestChanged;
        }
    }
}
