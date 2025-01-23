using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ExpenditionManager : MonoBehaviour
{
    public static ExpenditionManager Instance { get; private set; }
    public GameObject FindGameObjectWithUIExSelectPlace()
    {
        // ดึง GameObjects ทั้งหมดในระดับ Root ของฉากปัจจุบัน
        GameObject[] allGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        // วนลูปหา GameObjects และตรวจสอบว่ามี Script UIExSelectPlace หรือไม่
        foreach (GameObject go in allGameObjects)
        {
            // ตรวจสอบใน GameObject และลูกของมัน (รวมถึง Inactive)
            UIExSelectPlace[] components = go.GetComponentsInChildren<UIExSelectPlace>(true);

            foreach (UIExSelectPlace component in components)
            {
                if (component != null)
                {
                    Debug.Log($"Found UIExSelectPlace on GameObject: {component.gameObject.name}");
                    return component.gameObject; // Return ตัว GameObject
                }
            }
        }

        Debug.Log("No GameObject with UIExSelectPlace found.");
        return null;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        UIExSelectPlace uIExSelectPlace = FindGameObjectWithUIExSelectPlace().GetComponent<UIExSelectPlace>();
        transformsUIEx = uIExSelectPlace.transformParentUIEx;
    }

    public NpcClass npcSelecying;
    public List<ItemData> listItemDataInventoryslot; // Items in the player's inventory (not the box)
    public List<ItemData> listItemDataInventoryEqicment;
    public List<ItemData> listItemDataCarInventory;
    public GameObject uIExOne;
    public GameObject uIExTwo;
    public GameObject playerObject;
    public GameObject uIInventoryExPrefab;
    public Transform transformsUIEx;
    public UIButtonEX uIButtonEXOne;
    public UIButtonEX uIButtonEXTwo;
    public bool isuseCar;
    public bool isuseTunnel;
    public bool iswalk;

    public Globalstat globalstat;
    public InventoryItemPresent inventoryItemPresent;

    private void Start()
    {   
       
        inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();
        globalstat = FindObjectOfType<Globalstat>();
    }

    // Alternate "AddItem" method for the player's inventory slots
    public void AddItemToInventorySlot(ItemData itemDataAdd)
    {
        if (npcSelecying == null)
        {
            Debug.LogWarning("npcSelecying is not assigned. Cannot determine max slot count.");
            return;
        }

        int maxSlots = npcSelecying.countInventorySlot;
        int leftover = itemDataAdd.count;

        // First, try to fill existing partial stacks of the same item.
        foreach (ItemData stack in listItemDataInventoryslot)
        {
            if (stack.idItem == itemDataAdd.idItem && stack.count < stack.maxCount)
            {
                int availableSpace = stack.maxCount - stack.count;
                int toAdd = Mathf.Min(availableSpace, leftover);
                stack.count += toAdd;
                leftover -= toAdd;

                if (leftover == 0)
                    break; // All items added successfully
            }
        }

        // If we still have leftover items, try to create new stacks
        while (leftover > 0 && listItemDataInventoryslot.Count < maxSlots)
        {
            // Determine how many items we can put in a new stack
            int itemsToStack = Mathf.Min(itemDataAdd.maxCount, leftover);

            ItemData newItemData = new ItemData
            {
                nameItem = itemDataAdd.nameItem,
                idItem = itemDataAdd.idItem,
                count = itemsToStack,
                maxCount = itemDataAdd.maxCount,
                itemtype = itemDataAdd.itemtype
            };

            listItemDataInventoryslot.Add(newItemData);
            leftover -= itemsToStack;
        }

        // If after filling partial stacks and creating new stacks we still have leftover,
        // it means we hit the slot limit. Discard the remaining items.
        if (leftover > 0)
        {
            Debug.Log($"Discarded {leftover} '{itemDataAdd.nameItem}' items because the inventory is full.");
            // At this point, we simply do nothing with the leftover items.
            // They are considered 'destroyed'.
        }
    }


    // Alternate "RemoveItem" method for the player's inventory slots
    public void RemoveItemFromInventorySlot(ItemData itemDataRemove)
    {
        // Find an item stack that matches the ID from the "end" of the list
        ItemData itemDataInInventory = listItemDataInventoryslot
            .LastOrDefault(item => item.idItem == itemDataRemove.idItem);

        if (itemDataInInventory != null)
        {
            if (itemDataInInventory.count >= itemDataRemove.count)
            {
                // We have enough items in that stack to remove
                itemDataInInventory.count -= itemDataRemove.count;

                // If the stack is now empty, remove it entirely
                if (itemDataInInventory.count == 0)
                {
                    listItemDataInventoryslot.Remove(itemDataInInventory);
                }
            }
            else
            {
                // Not enough items to remove. Handle as needed (e.g., show error message)
                Debug.LogWarning("Not enough items in inventory to remove.");
            }
        }
        else
        {
            // The item does not exist in the inventory at all
            Debug.LogWarning("Item to remove not found in inventory.");
        }
    }

    public void CreateInventorySetExpendition(float timeScale, float riskValue, int indexSceneExpendition, bool isCar, bool isWalk, bool isTunnel)
    {
        if (uIInventoryExPrefab == null)
        {
            Debug.LogError("itemPrefab is not assigned in the Inspector.");
            return;
        }

        GameObject uIEx = Instantiate(uIInventoryExPrefab, transformsUIEx);

        UIInventoryEX uIInventoryEx = uIEx.GetComponent<UIInventoryEX>();
        uIInventoryEx.inventoryItemPresent = inventoryItemPresent;
        uIInventoryEx.timeScale = timeScale;

        // Adjust riskValue based on global factors and transport mode
        uIInventoryEx.riskValue = Mathf.Min(riskValue + globalstat.expiditionrisk,90);

        if (isCar)
        {
            uIInventoryEx.riskValue = Mathf.Min(uIInventoryEx.riskValue * 2, 90);
            globalstat.availablecar -= 1;
        }
        if (isTunnel)
        {
            uIInventoryEx.riskValue = 0; // Tunnel has no risk
        }

        uIInventoryEx.indexSceneExpendition = indexSceneExpendition;

        // Store mode information for UI or logic, if needed
        uIInventoryEx.isuseCar = isCar;
        uIInventoryEx.iswalk = isWalk;
        uIInventoryEx.isuseTunnel = isTunnel;

        uIEx.SetActive(true);
    }
    public void SetUIExButton(int indexEXUI, Sprite spriteHeadNpc, string textdayFinish)
    {
        if (indexEXUI == 1)
        {
            uIButtonEXOne.GetComponent<UIButtonEX>().SetUIButtonEX(spriteHeadNpc, textdayFinish);
        }
        else
        {
            uIButtonEXTwo.GetComponent<UIButtonEX>().SetUIButtonEX(spriteHeadNpc, textdayFinish);
        }
    }

    public void OpenUIExpenditionInventoryOne()
    {
        if (uIExOne != null)
        {
            uIExOne.SetActive(true);
        }
    }
    public void OpenUIExpenditionInventoryTwo()
    {
        if (uIExTwo != null)
        {
            uIExTwo.SetActive(true);
        }
    }
    public bool IsActiveEvent()
    {
        float randomValue = Random.Range(0f, 100f);
        return randomValue <= globalstat.expiditionrisk;
    }
}
