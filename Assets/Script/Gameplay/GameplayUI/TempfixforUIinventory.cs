using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempfixforUIinventory : MonoBehaviour
{
    public UIcontrollerDefense uIcontrollerDefense;
    public GameObject InventoryUI;
    public GameObject InventorybackpackUI;
    private bool isused;

    void Start()
    {
        isused = false;
        uIcontrollerDefense =  GetComponent<UIcontrollerDefense>();
    }
    void Update()
    {
        if (!isused && !InventoryUI.activeSelf)
        {
            isused = true;
            StartCoroutine(tempfixforUIbug());
        }
    }
    private IEnumerator tempfixforUIbug()
    {
        yield return new WaitForSeconds(0.1f);
        InventoryUI.SetActive(true);
        yield return new WaitForSeconds(1f);
        InventoryUI.SetActive(false);
    }
}
