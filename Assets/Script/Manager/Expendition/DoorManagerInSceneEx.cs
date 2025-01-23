using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManagerInSceneEx : MonoBehaviour
{   
    public int indexSceneExpendition;
    public ManagerSceneEX managerSceneEX;
    public Outpostup outpostup;
    public List<BreakableDoor> listDoors = new List<BreakableDoor>();
    private void Awake()
    {
        managerSceneEX = FindObjectOfType<ManagerSceneEX>();
        managerSceneEX.UpdateStatusDoorInSceneEx(listDoors,indexSceneExpendition);
    }
    private void OnDestroy() {
        managerSceneEX.AddDataUpdataDoorUnlockFromSceneExInManagerSceneEx(listDoors,indexSceneExpendition);
    }

}
