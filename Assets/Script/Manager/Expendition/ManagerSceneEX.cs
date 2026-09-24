using System;
using System.Collections.Generic;
using UnityEngine;

public class ManagerSceneEX : MonoBehaviour
{
    public List<DoorsSceneEx> listDoorSceneExes = new List<DoorsSceneEx>();

    public DoorsSceneEx GetOrCreateDoorsSceneEx(int indexSceneEx)
    {
        if (listDoorSceneExes == null)
        {
            listDoorSceneExes = new List<DoorsSceneEx>();
        }

        for (int i = 0; i < listDoorSceneExes.Count; i++)
        {
            if (listDoorSceneExes[i] != null && listDoorSceneExes[i].idSceneEx == indexSceneEx)
            {
                if (listDoorSceneExes[i].listUnLockDoorInSceneEx == null)
                {
                    listDoorSceneExes[i].listUnLockDoorInSceneEx = new List<bool>();
                }
                return listDoorSceneExes[i];
            }
        }

        DoorsSceneEx newEntry = new DoorsSceneEx
        {
            idSceneEx = indexSceneEx,
            listUnLockDoorInSceneEx = new List<bool>()
        };
        listDoorSceneExes.Add(newEntry);
        return newEntry;
    }

    public void UpdateStatusDoorInSceneEx(List<BreakableDoor> listBreakableDoors, int indexSceneEx)
    {
        if (listBreakableDoors == null) return;

        DoorsSceneEx doorSceneEx = GetOrCreateDoorsSceneEx(indexSceneEx);
        if (doorSceneEx.listUnLockDoorInSceneEx == null)
        {
            doorSceneEx.listUnLockDoorInSceneEx = new List<bool>();
        }

        // If door count matches or partially exists, restore status
        for (int i = 0; i < listBreakableDoors.Count; i++)
        {
            BreakableDoor door = listBreakableDoors[i];
            if (door == null) continue;

            if (i < doorSceneEx.listUnLockDoorInSceneEx.Count)
            {
                door.isopen = doorSceneEx.listUnLockDoorInSceneEx[i];
            }
            else
            {
                doorSceneEx.listUnLockDoorInSceneEx.Add(door.isopen);
            }
        }
    }

    public void AddDataUpdataDoorUnlockFromSceneExInManagerSceneEx(List<BreakableDoor> listBreakableDoors, int indexSceneEx)
    {
        if (listBreakableDoors == null) return;

        DoorsSceneEx doorSceneEx = GetOrCreateDoorsSceneEx(indexSceneEx);
        doorSceneEx.listUnLockDoorInSceneEx.Clear();

        for (int i = 0; i < listBreakableDoors.Count; i++)
        {
            BreakableDoor door = listBreakableDoors[i];
            if (door != null)
            {
                doorSceneEx.listUnLockDoorInSceneEx.Add(door.isopen);
            }
        }
    }
}

[Serializable]
public class DoorsSceneEx
{
    public int idSceneEx;
    public List<bool> listUnLockDoorInSceneEx = new List<bool>();
}