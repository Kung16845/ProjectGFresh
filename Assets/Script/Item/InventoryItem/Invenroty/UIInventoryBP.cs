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
        int gameobjectsceneIndex = gameObject.scene.buildIndex;
        Debug.Log("Scene index Game object : " + gameobjectsceneIndex);
        ClearItemDataInAllInventorySlotToListDataBoxes();
        NpcClass npc = npcManager.GetNpcById(npcSelecting.idnpc);
        npc.isWorking = false;
    }
}
