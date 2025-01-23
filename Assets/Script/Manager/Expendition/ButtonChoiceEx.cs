using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonChoiceEx : MonoBehaviour
{
    public Button buttonCreateExpendition;
    public ExpenditionManager expenditionManager;
    public Transform transformParent;
    public int indexEXButton;

    void Start()
    {
        expenditionManager = FindObjectOfType<ExpenditionManager>();
        expenditionManager.transformsUIEx = transformParent;
        if (indexEXButton == 1)
        {
            Debug.Log("Reset Button One");
            buttonCreateExpendition.onClick.RemoveAllListeners();
            // buttonCreateExpendition.onClick.AddListener(() => ExpenditionManager.Instance.CreateInventorySetExpendition(8000,20));
        }
        else if (indexEXButton == 2)
        {

            Debug.Log("Reset Button Two");
            buttonCreateExpendition.onClick.RemoveAllListeners();
            // buttonCreateExpendition.onClick.AddListener(() => ExpenditionManager.Instance.CreateInventorySetExpendition(10000,20));
        }
        else if (indexEXButton == 3)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
