using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class ManagerSceneEX : MonoBehaviour
{   
    public List<DoorsSceneEx> listDoorSceneExes;

    public void UpdateStatusDoorInSceneEx(List<BreakableDoor> listBreakableDoors,int indexSceneEx)
    {
        DoorsSceneEx listDoorSceneExIsOpen = listDoorSceneExes.FirstOrDefault(scene => scene.idSceneEx == indexSceneEx);
        for(int i = 0; i < listBreakableDoors.Count;i++)
        {
            listBreakableDoors.ElementAt(i).isopen = listDoorSceneExIsOpen.listUnLockDoorInSceneEx.ElementAt(i);
        }
    }
    public void AddDataUpdataDoorUnlockFromSceneExInManagerSceneEx(List<BreakableDoor> listBreakableDoors,int indexSceneEx)
    {
        DoorsSceneEx listDoorSceneExIsOpen = listDoorSceneExes.FirstOrDefault(scene => scene.idSceneEx == indexSceneEx);
        listDoorSceneExIsOpen.listUnLockDoorInSceneEx.Clear();
        foreach (BreakableDoor breakableDoor in listBreakableDoors)
        {
            listDoorSceneExIsOpen.listUnLockDoorInSceneEx.Add(breakableDoor.isopen);
        }
    }
}
[Serializable]
public class DoorsSceneEx
{
    public int idSceneEx;
    public List<bool> listUnLockDoorInSceneEx;
}