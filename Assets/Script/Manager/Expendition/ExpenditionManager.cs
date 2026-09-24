using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExpenditionManager : MonoBehaviour
{
    public static ExpenditionManager Instance { get; private set; }

    public NpcClass npcSelecying;
    public List<ItemData> listItemDataInventoryslot = new List<ItemData>();
    public List<ItemData> listItemDataInventoryEqicment = new List<ItemData>();
    public List<ItemData> listItemDataCarInventory = new List<ItemData>();
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

    public GameObject FindGameObjectWithUIExSelectPlace()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (!activeScene.isLoaded) return null;

        GameObject[] allGameObjects = activeScene.GetRootGameObjects();
        foreach (GameObject go in allGameObjects)
        {
            if (go == null) continue;
            UIExSelectPlace[] components = go.GetComponentsInChildren<UIExSelectPlace>(true);
            foreach (UIExSelectPlace component in components)
            {
                if (component != null)
                {
                    return component.gameObject;
                }
            }
        }

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
            return;
        }

        EnsureTransformsUIEx();
    }

    private void Start()
    {
        if (inventoryItemPresent == null)
        {
            inventoryItemPresent = FindFirstObjectByType<InventoryItemPresent>();
        }

        if (globalstat == null)
        {
            globalstat = FindFirstObjectByType<Globalstat>();
        }
    }

    public void EnsureTransformsUIEx()
    {
        if (transformsUIEx != null) return;

        GameObject selectPlaceObj = FindGameObjectWithUIExSelectPlace();
        if (selectPlaceObj != null)
        {
            UIExSelectPlace uIExSelectPlace = selectPlaceObj.GetComponent<UIExSelectPlace>();
            if (uIExSelectPlace != null)
            {
                transformsUIEx = uIExSelectPlace.transformParentUIEx;
            }
        }
    }

    public void AddItemToInventorySlot(ItemData itemDataAdd)
    {
        if (itemDataAdd == null) return;

        if (npcSelecying == null)
        {
            Debug.LogWarning("[ExpenditionManager] npcSelecying is not assigned. Cannot determine max slot count.");
            return;
        }

        if (listItemDataInventoryslot == null)
        {
            listItemDataInventoryslot = new List<ItemData>();
        }

        int maxSlots = npcSelecying.countInventorySlot;
        int leftover = itemDataAdd.count;

        // First, try to fill existing partial stacks
        for (int i = 0; i < listItemDataInventoryslot.Count; i++)
        {
            ItemData stack = listItemDataInventoryslot[i];
            if (stack != null && stack.idItem == itemDataAdd.idItem && stack.count < stack.maxCount)
            {
                int availableSpace = stack.maxCount - stack.count;
                int toAdd = Mathf.Min(availableSpace, leftover);
                stack.count += toAdd;
                leftover -= toAdd;

                if (leftover <= 0) break;
            }
        }

        // If leftover remains, create new stacks
        while (leftover > 0 && listItemDataInventoryslot.Count < maxSlots)
        {
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

        if (leftover > 0)
        {
            Debug.Log($"[ExpenditionManager] Discarded {leftover} '{itemDataAdd.nameItem}' items because inventory is full.");
        }
    }

    public void RemoveItemFromInventorySlot(ItemData itemDataRemove)
    {
        if (itemDataRemove == null || listItemDataInventoryslot == null) return;

        // Find last matching item from reverse
        for (int i = listItemDataInventoryslot.Count - 1; i >= 0; i--)
        {
            ItemData stack = listItemDataInventoryslot[i];
            if (stack != null && stack.idItem == itemDataRemove.idItem)
            {
                if (stack.count >= itemDataRemove.count)
                {
                    stack.count -= itemDataRemove.count;
                    if (stack.count <= 0)
                    {
                        listItemDataInventoryslot.RemoveAt(i);
                    }
                    return;
                }
                else
                {
                    Debug.LogWarning("[ExpenditionManager] Not enough items in inventory slot to remove.");
                    return;
                }
            }
        }

        Debug.LogWarning("[ExpenditionManager] Item to remove not found in inventory.");
    }

    public void CreateInventorySetExpendition(float timeScale, float riskValue, int indexSceneExpendition, bool isCar, bool isWalk, bool isTunnel)
    {
        if (uIInventoryExPrefab == null)
        {
            Debug.LogError("[ExpenditionManager] uIInventoryExPrefab is not assigned in the Inspector.");
            return;
        }

        EnsureTransformsUIEx();

        if (globalstat == null)
        {
            globalstat = FindFirstObjectByType<Globalstat>();
        }

        GameObject uIEx = Instantiate(uIInventoryExPrefab, transformsUIEx);
        UIInventoryEX uIInventoryEx = uIEx.GetComponent<UIInventoryEX>();
        if (uIInventoryEx == null)
        {
            Debug.LogError("[ExpenditionManager] UIInventoryEX component missing on instantiated prefab.");
            return;
        }

        uIInventoryEx.inventoryItemPresent = inventoryItemPresent;
        uIInventoryEx.timeScale = timeScale;

        float baseRisk = globalstat != null ? globalstat.expiditionrisk : 0f;
        uIInventoryEx.riskValue = Mathf.Min(riskValue + baseRisk, 90f);

        if (isCar)
        {
            uIInventoryEx.riskValue = Mathf.Min(uIInventoryEx.riskValue * 2f, 90f);
            if (globalstat != null)
            {
                globalstat.availablecar -= 1;
            }
        }

        if (isTunnel)
        {
            uIInventoryEx.riskValue = 0f;
        }

        uIInventoryEx.indexSceneExpendition = indexSceneExpendition;
        uIInventoryEx.isuseCar = isCar;
        uIInventoryEx.iswalk = isWalk;
        uIInventoryEx.isuseTunnel = isTunnel;

        uIEx.SetActive(true);
    }

    public void SetUIExButton(int indexEXUI, Sprite spriteHeadNpc, string textdayFinish)
    {
        if (indexEXUI == 1)
        {
            if (uIButtonEXOne != null)
            {
                uIButtonEXOne.SetUIButtonEX(spriteHeadNpc, textdayFinish);
            }
        }
        else
        {
            if (uIButtonEXTwo != null)
            {
                uIButtonEXTwo.SetUIButtonEX(spriteHeadNpc, textdayFinish);
            }
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
        if (globalstat == null)
        {
            globalstat = FindFirstObjectByType<Globalstat>();
        }

        float risk = globalstat != null ? globalstat.expiditionrisk : 0f;
        float randomValue = Random.Range(0f, 100f);
        return randomValue <= risk;
    }
}
