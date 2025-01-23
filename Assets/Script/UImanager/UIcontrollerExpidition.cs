using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIcontrollerExpidition : MonoBehaviour
{
    public GameObject MainInventoryUI;
    public GameObject InventoryUI;
    public ActionController actionController;
    private bool isInventoryActive = false;
    void Start()
    {
        actionController.canuseweapon = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMainInventoryUI();
        }

    }
    void ToggleUI(bool maininventoryActive,bool InventoryUIbakcapck)
    {
        if (MainInventoryUI != null) MainInventoryUI.SetActive(maininventoryActive);
        if  (InventoryUI != null) InventoryUI.SetActive(InventoryUIbakcapck);
    }
    public void ToggleMainInventoryUI()
    {
        isInventoryActive = !isInventoryActive;
        if (isInventoryActive)
        {
            actionController.canwalk = false;
            actionController.canuseweapon = false;
            ToggleUI(true,true);
        }
        else
        {
            actionController.canwalk = true;
            actionController.canuseweapon = true;
            ToggleUI(false,false);
        }
    }
}
