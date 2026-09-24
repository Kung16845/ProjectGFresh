using System.Collections.Generic;
using UnityEngine;

public class UILockGameObject : MonoBehaviour
{
    public List<InventorySlots> listinvenrotySlots;
    public List<GameObject> listgameObjectsLockIcon;

    private SlotType[] _lastSlotTypes;

    private void Start()
    {
        InitializeCache();
        ActiveUILOckSlot();
    }

    private void Update()
    {
        if (HasSlotStatesChanged())
        {
            ActiveUILOckSlot();
        }
    }

    private void InitializeCache()
    {
        if (listinvenrotySlots != null)
        {
            _lastSlotTypes = new SlotType[listinvenrotySlots.Count];
            for (int i = 0; i < listinvenrotySlots.Count; i++)
            {
                _lastSlotTypes[i] = listinvenrotySlots[i] != null 
                    ? listinvenrotySlots[i].slotTypeInventory 
                    : SlotType.SlotLock;
            }
        }
    }

    private bool HasSlotStatesChanged()
    {
        if (listinvenrotySlots == null || _lastSlotTypes == null || _lastSlotTypes.Length != listinvenrotySlots.Count)
        {
            return true;
        }

        for (int i = 0; i < listinvenrotySlots.Count; i++)
        {
            SlotType current = listinvenrotySlots[i] != null ? listinvenrotySlots[i].slotTypeInventory : SlotType.SlotLock;
            if (_lastSlotTypes[i] != current)
            {
                return true;
            }
        }
        return false;
    }

    public void ActiveUILOckSlot()
    {
        if (listinvenrotySlots == null || listgameObjectsLockIcon == null) return;

        int count = Mathf.Min(listinvenrotySlots.Count, listgameObjectsLockIcon.Count);

        if (_lastSlotTypes == null || _lastSlotTypes.Length != listinvenrotySlots.Count)
        {
            _lastSlotTypes = new SlotType[listinvenrotySlots.Count];
        }

        for (int i = 0; i < count; i++)
        {
            InventorySlots slot = listinvenrotySlots[i];
            GameObject lockIcon = listgameObjectsLockIcon[i];

            if (slot == null || lockIcon == null) continue;

            SlotType currentType = slot.slotTypeInventory;
            _lastSlotTypes[i] = currentType;

            bool shouldLock = (currentType == SlotType.SlotLock);
            if (lockIcon.activeSelf != shouldLock)
            {
                lockIcon.SetActive(shouldLock);
            }
        }
    }
}
