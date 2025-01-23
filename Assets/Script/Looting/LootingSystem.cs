using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using DG.Tweening; 
using System.Linq;

public class LootingSystem : MonoBehaviour
{
    [SerializeField] private Transform itemsContainer; // The UI container where loot items will be displayed
    [SerializeField] private GameObject lootItemUIPrefab;
    [SerializeField] private GameObject LootUI;
    [SerializeField] private GameObject PlayerInventory;
    public LootPool lootPool; 
    public float openDuration = 1000f; // 1000 = 1 second
    public Slider lootProgressSlider; 
    private float openProgress = 0f;
    public bool isLooting = false;
    public bool inrange;
    public bool itemdropped = false;
    private UIcontrollerExpidition uIcontrollerExpidition;

    public KeyCode lootKey = KeyCode.F;
    private InventoryItemPresent inventoryItemPresent;
    private ExpenditionManager expenditionManager;
    private ActionController actionController;
    private bool Uiisopened;
    public  UIInventory uIInventoryEX;
    private SpriteRenderer spriteRenderer;

    // List of items currently dropped and awaiting player action:
    public List<ItemData> droppedItems = new List<ItemData>();

    // Tracks if we've already shown loot UI once

    void Start()
    {
        actionController = FindObjectOfType<ActionController>();
        uIcontrollerExpidition = FindObjectOfType<UIcontrollerExpidition>();
        inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();
        expenditionManager = FindObjectOfType<ExpenditionManager>();
        lootProgressSlider.gameObject.SetActive(false);
        spriteRenderer = GetComponent<SpriteRenderer>();
        Uiisopened = false;
    }
    public void InitializeLootSystem(UIInventory inventory)
    {
        inventory.currentLootingSystem = this;
    }

    void Update()
    {
        if (inrange && !itemdropped)
        {
            if (Input.GetKey(lootKey))
            {
                openProgress += (100f / (openDuration / 1000f)) * Time.deltaTime;
                openProgress = Mathf.Clamp(openProgress, 0f, 100f);
                UpdateLootProgressUI(openProgress);
                actionController.canwalk = false;
                if (openProgress >= 100f)
                {
                    GiveLoot();
                    ResetLooting();
                }
            }
            else if (Input.GetKeyUp(lootKey))
            {
                ResetLooting();
            }
        }
        else if (inrange && itemdropped)
        {
            if(droppedItems.Count > 0 && (Input.GetKey(lootKey) || Input.GetKeyDown(KeyCode.Tab)))
            {
                Debug.Log("CheckUI");
                if(Input.GetKey(lootKey))
                {
                    uIcontrollerExpidition.ToggleMainInventoryUI();
                }
                OpenLootUI();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inrange = true;
            if (droppedItems.Count == 0 && !itemdropped)
            {
                spriteRenderer.DOColor(Color.green, 0.5f);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inrange = false;
            if (droppedItems.Count == 0)
            {
                spriteRenderer.DOColor(Color.white, 0.5f);
                if(itemdropped)
                    OnLootUIClosed();
            }
            else if (droppedItems.Count >= 1)
            {
                OnLootUIClosed();
                spriteRenderer.DOColor(Color.yellow, 0.5f);
            }
        }
    }

    private void ResetLooting()
    {
        isLooting = false;
        openProgress = 0f;
        UpdateLootProgressUI(openProgress);
        actionController.canwalk = true;
        lootProgressSlider.gameObject.SetActive(false);
    }

    private void GiveLoot()
    {
        if (lootPool == null || lootPool.lootItems.Count == 0) return;
        List<LootPool.LootResult> lootResults = lootPool.GetRandomLoot();
        if (lootResults == null || lootResults.Count == 0) return;

        foreach (var lootResult in lootResults)
        {
            if (lootResult.item != null && lootResult.amount > 0)
            {
                ItemData newItemData = new ItemData
                {
                    nameItem = lootResult.item.nameItem,
                    idItem = lootResult.item.idItem,
                    count = lootResult.amount,
                    maxCount = lootResult.item.maxCount,
                    itemtype = lootResult.item.itemtype
                };

                droppedItems.Add(newItemData);
            }
        }

        if (droppedItems.Count > 0)
        {
            itemdropped = true;
            OpenLootUI();
            Debug.Log("Loot Granted");
        }
    }

    private void UpdateLootProgressUI(float progress)
    {
        if (lootProgressSlider != null)
        {
            lootProgressSlider.gameObject.SetActive(true);
            lootProgressSlider.value = progress / 100f;
        }
    }
    public void RemoveItemFromLootList(int itemId, int quantity)
    {
        var itemToRemove = droppedItems.FirstOrDefault(item => item.idItem == itemId);
        if (itemToRemove != null)
        {
            itemToRemove.count -= quantity;
            if (itemToRemove.count <= 0)
            {
                droppedItems.Remove(itemToRemove);
            }

            // Refresh the Loot UI if it's open
            if (LootUI.activeSelf)
            {
                OpenLootUI();
            }
        }
    }

    public void OpenLootUI()
    {
        if (Uiisopened) return;
        Uiisopened = true;
        // Clear any existing UI elements
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }
        PlayerInventory.SetActive(true);
        LootUI.SetActive(true);
        uIInventoryEX.currentLootingSystem = this;
        // Initialize the loot system in the inventory

        foreach (var item in droppedItems)
        {
            UIItemData prefabData = inventoryItemPresent.listUIItemPrefab.FirstOrDefault(prefab => prefab.idItem == item.idItem);

            if (prefabData != null)
            {
                GameObject newItemUI = Instantiate(prefabData.gameObject, itemsContainer);
                UIItemData uiItemData = newItemUI.GetComponent<UIItemData>();
                uiItemData.originatingLootSystem = this;

                ItemClass itemClass = newItemUI.GetComponent<ItemClass>();
                itemClass.nameItem = item.nameItem;
                itemClass.idItem = item.idItem;
                itemClass.quantityItem = item.count;
                itemClass.maxCountItem = item.maxCount;
                itemClass.itemtype = item.itemtype;

                uiItemData.idItem = item.idItem;
                uiItemData.nameItem = item.nameItem;
                uiItemData.slotTypeParent = SlotType.SlotLoot;

                Sprite icon = inventoryItemPresent.GetItemIconByID(item.idItem);
                if (icon != null)
                {
                    uiItemData.itemIconImage.sprite = icon;
                }

                uiItemData.UpdateDataUI(itemClass);
            }
            else
            {
                Debug.LogWarning($"No matching UI prefab found for item ID: {item.idItem}");
            }
        }

        gameObject.SetActive(true);
    }


        public void OnLootUIClosed()
        {
            Debug.Log("CheckUI3");
            Uiisopened = false;
            LootUI.SetActive(false);
            PlayerInventory.SetActive(false);
        }
    }
