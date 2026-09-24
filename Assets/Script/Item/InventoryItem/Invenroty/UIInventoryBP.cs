using UnityEngine;

public class UIInventoryBP : UIInventory
{
    private void Start()
    {
        SetValuableUIInventory();
        RefreshUIInventory();
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
            {
                npc.isWorking = false;
            }
        }
    }
}
