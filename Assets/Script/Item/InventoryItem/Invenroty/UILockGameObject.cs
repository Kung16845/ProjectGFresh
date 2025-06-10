using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UILockGameObject : MonoBehaviour
{
    public List<InventorySlots> listinvenrotySlots;
    public List<GameObject> listgameObjectsLockIcon;
    void Update()
    {
        ActiveUILOckSlot();
    }
    public void ActiveUILOckSlot()
    {
        for (int i = 0; i < listinvenrotySlots.Count; i++)
        {
            SlotType slotTypeslotType = listinvenrotySlots.ElementAt(i).slotTypeInventory;
            if (slotTypeslotType == SlotType.SlotLock)
            {
                listgameObjectsLockIcon.ElementAt(i).SetActive(true);
            }
            else
            {
                listgameObjectsLockIcon.ElementAt(i).SetActive(false);
            }
        }
    }
}
