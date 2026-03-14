using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIInventoryBP : UIInventory
{

    public void Start()
    {
        SetValuableUIInventory();
        RefreshUIInventory();
        // SelectNpcDefenseScene();
    }

    private void OnEnable()
    {
        RefreshUIInventory();
    }
    private void OnDisable()
    {
        ConventDataUIToItemData();
    }
    private void OnDestroy()
    {
        ClearItemDataInAllInventorySlotToListDataBoxes();
        if (npcManager != null && npcSelecting != null)
        {
            NpcClass npc = npcManager.GetNpcById(npcSelecting.idnpc);
            if (npc != null)
                npc.isWorking = false;
        }
    }
}
