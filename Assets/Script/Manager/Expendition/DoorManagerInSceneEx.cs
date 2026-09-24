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
        if (managerSceneEX == null)
        {
            managerSceneEX = FindFirstObjectByType<ManagerSceneEX>();
        }

        if (managerSceneEX != null)
        {
            managerSceneEX.UpdateStatusDoorInSceneEx(listDoors, indexSceneExpendition);
        }
    }

    private void OnDestroy()
    {
        if (managerSceneEX != null)
        {
            managerSceneEX.AddDataUpdataDoorUnlockFromSceneExInManagerSceneEx(listDoors, indexSceneExpendition);
        }
    }
}
