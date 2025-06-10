using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIInventoryEX : UIInventory
{
    public List<InventorySlots> listInvenrotyCarSlotsUI = new List<InventorySlots>();
    public List<ItemData> listItemDataCarInventorySlot;
    public float timeScale;
    public float riskValue;
    public int indexButtonExpendition;
    public int indexSceneExpendition;
    public bool isArriveEx;
    public bool isArriveHome;
    public bool isExpenditon;
    public bool isuseCar;
    public bool isuseTunnel;
    public bool iswalk;
    public bool istraveling;
    public int finishDayCraftingTime;
    public int finishHourCraftingTime;
    public int finishMinutesCraftingTime;
    public ExpenditionManager expenditionManager;
    public SceneSystem sceneSystem;
    public GameObject uIBoxesInventory;
    public GameObject UICarInventory;
    public GameObject UIBOxInventory;
    public GameObject uINpcSending;
    public GameObject uINpcArriveEx;
    public GameObject uINpcGoBack;
    public GameObject CloseButton;
    public Globalstat globalstat;

    public List<GameObject> listEvnet;
    private void Awake()
    {
        SetValuableUIInventory();
        expenditionManager = GameManager.Instance.expenditionManager;
    }
    public void Start()
    {
        if(isuseCar && UICarInventory != null)
            UICarInventory.SetActive(true);
        globalstat = GameManager.Instance.globalstat;
        sceneSystem = FindFirstObjectByType<SceneSystem>();
        SetPlayerExpendition();
        if (indexButtonExpendition == 1)
        {
            expenditionManager.uIExOne = this.gameObject;
        }
        else if (indexButtonExpendition == 2)
        {
            expenditionManager.uIExTwo = this.gameObject;
        }
    }
     public void ConventAllUIItemInListCarInventorySlotToListItemData(List<ItemData> listSlotItemDatas)
    {
        listSlotItemDatas.Clear(); // Clear old data
        for (int i = 0; i < listInvenrotyCarSlotsUI.Count; i++)
        {
            ItemClass itemClass = listInvenrotyCarSlotsUI.ElementAt(i).GetComponentInChildren<ItemClass>();
            if (itemClass != null)
            {
                ItemData itemData = inventoryItemPresent.ConventItemClassToItemData(itemClass);
                listSlotItemDatas.Add(itemData);
            }
        }
    }
    public void SetPlayerExpendition()
    {
        if (isExpenditon)
        {
            Debug.Log("Enter Scene Expendition");
            expenditionManager = GameManager.Instance.expenditionManager;
            inventoryItemPresent = GameManager.Instance.inventoryItemPresent;
            expenditionManager.playerObject = FindFirstObjectByType<PlayerMovement>().gameObject;
            statAmplifier = FindFirstObjectByType<StatAmplifier>();
            listItemDataCarInventorySlot = expenditionManager.listItemDataCarInventory;
            GameObject npcPlayer = expenditionManager.playerObject;

            npcSelecting = expenditionManager.npcSelecying;
            npcManager = GameManager.Instance.npcManager;

            npcManager.uIInventory = this;
            npcManager.levelCombatText = levelCombatText;
            npcManager.levelEnduranceText = levelEnduranceText;
            npcManager.levelSpeedText = levelSpeedText;
            npcManager.specialistNpcText = specialistNpcText;

            levelCombatText.text = npcSelecting.combat.ToString();
            levelEnduranceText.text = npcSelecting.endurance.ToString();
            levelSpeedText.text = npcSelecting.speed.ToString();
            specialistNpcText.text = npcSelecting.roleNpc.ToString();

            iswalk = expenditionManager.iswalk;
            isuseCar = expenditionManager.isuseCar;
            isuseTunnel = expenditionManager.isuseTunnel;

            dropdown.ClearOptions();
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = npcSelecting.nameNpc;

            dropdown.AddOptions(new List<TMP_Dropdown.OptionData> { option });
            spriteHeadNpc.sprite = npcManager.listHeadCoutume.FirstOrDefault(npcCoustume => npcCoustume.idHead == npcSelecting.idHead).spriteHead;

            SetInventoryItemDataEx(expenditionManager.listItemDataInventoryslot, expenditionManager.listItemDataInventoryEqicment);
            SetCostumeNpcExpentdition(npcSelecting, npcPlayer);

            if (statAmplifier != null)
            {
                statAmplifier.SetStatAmplifer(npcSelecting);
                statAmplifier.endurance = npcSelecting.endurance;
                statAmplifier.combat = npcSelecting.combat;
                statAmplifier.speed = npcSelecting.speed;
                statAmplifier.specialistRole = npcSelecting.roleNpc;
                statAmplifier.InitializeAmplifiers();
            }
            if(isuseCar)
                UICarInventory.SetActive(true);

            // Load car slot data into UI
            LoadCarSlotsFromData();

            RefreshUIInventory();
        }
    }
    public override void RefreshUIInventory()
    {
        base.RefreshUIInventory(); // Refresh normal slots and equipment from the parent class logic
        if(listItemDataCarInventorySlot.Count >= 1)
        {
            Debug.Log($"Before Refresh: {listItemDataCarInventorySlot.Count} items");
            CombineAndSplitItems(listItemDataCarInventorySlot);
            RefreshCarInventorySlots();
            Debug.Log($"After Refresh: {listItemDataCarInventorySlot.Count} items");
        }
        // BindCarSlotsToData();
    }
   public void RefreshCarInventorySlots()
    {
        // Step 1: Clear all children from slots
        ClearAllChildInvenrotyCarSlot();

        // Step 2: Sort and update data
        var orderedItems = listItemDataCarInventorySlot.OrderBy(item => item.idItem).ToList();

        // Step 3: Sync UI with data
        for (int i = 0; i < 12; i++)
        {
            if (i < listItemDataCarInventorySlot.Count)
            {
                // Place this item in slot i
                CreateUIItem(listItemDataCarInventorySlot[i], listInvenrotyCarSlotsUI[i]);
            }
        }
    }


   public void ClearAllChildInvenrotyCarSlot()
    {
        foreach (var carSlot in listInvenrotyCarSlotsUI)
        {
            if (carSlot != null && carSlot.transform.childCount > 0)
            {
                foreach (Transform child in carSlot.transform)
                {
                    Debug.Log($"Destroying child {child.name} in slot {carSlot.name}");
                    Destroy(child.gameObject); // Destroy each child
                }
            }
        }
    }

    public override void ConventDataUIToItemData()
    {
        base.ConventDataUIToItemData(); // Convert normal and equipment slots
        SyncCarSlotsToItemData();
    }
    private void SyncCarSlotsToItemData()
    {
        listItemDataCarInventorySlot.Clear(); // Clear once here
        foreach (var slot in listInvenrotyCarSlotsUI)
        {
            var itemClass = slot.GetComponentInChildren<ItemClass>();
            if (itemClass != null)
            {
                var itemData = inventoryItemPresent.ConventItemClassToItemData(itemClass);

                // Avoid duplicates
                if (!listItemDataCarInventorySlot.Any(item => item.idItem == itemData.idItem && item.count == itemData.count))
                {
                    listItemDataCarInventorySlot.Add(itemData);
                }
            }
        }
    }
    public void BindCarSlotsToData()
    {
        // Step 1: Clear all children in the car slots
        ClearAllChildInvenrotyCarSlot();

        // Step 2: Loop through both lists and bind each slot to the corresponding data
        for (int i = 0; i < listInvenrotyCarSlotsUI.Count; i++)
        {
            // Ensure the data list has enough items for the slot
            if (i < listItemDataCarInventorySlot.Count)
            {
                // Get the corresponding item data
                var itemData = listItemDataCarInventorySlot[i];

                // Create a UI item in the slot
                CreateUIItem(itemData, listInvenrotyCarSlotsUI[i]);
            }
        }

        // Debugging to ensure alignment
        Debug.Log($"Car slots and inventory data bound successfully. Total slots: {listInvenrotyCarSlotsUI.Count}, Total items: {listItemDataCarInventorySlot.Count}");
    }

   public void LoadCarSlotsFromData()
    {
        ClearAllChildInvenrotyCarSlot();

        // Ensure data is ordered by ID
        var orderedItems = listItemDataCarInventorySlot.OrderBy(item => item.idItem).ToList();

        foreach (var itemData in orderedItems)
        {
            var availableSlot = listInvenrotyCarSlotsUI.FirstOrDefault(slot => slot.transform.childCount == 0);
            if (availableSlot != null)
            {
                CreateUIItem(itemData, availableSlot);
            }
        }
    }
    public void CallFuntionAddListenerButton()
    {
        if (indexButtonExpendition == 1)
        {
            expenditionManager.OpenUIExpenditionInventoryOne();
        }
        else
        {
            expenditionManager.OpenUIExpenditionInventoryTwo();
        }
    }

    public void SetDataMoveSceneForEventExpendition()
    {
        Debug.Log("SetDataMoveSceneForEventExpendition");
        expenditionManager.iswalk = iswalk;
        expenditionManager.isuseCar = isuseCar;
        expenditionManager.isuseTunnel = isuseTunnel;
        expenditionManager.npcSelecying = this.npcSelecting;
        expenditionManager.listItemDataInventoryEqicment = this.listItemDataInventoryEquipment;
        expenditionManager.listItemDataInventoryslot = this.listItemDataInventorySlot;
        expenditionManager.listItemDataCarInventory = this.listItemDataCarInventorySlot;
    }
    public void SetInventoryItemDataEx(List<ItemData> listDataInventoryslot, List<ItemData> listDataInventoryEqicment)
    {
        listItemDataInventorySlot.Clear();
        listItemDataInventoryEquipment.Clear();
        listItemDataInventorySlot = listDataInventoryslot;
        listItemDataInventoryEquipment = listDataInventoryEqicment;
        RefreshUIInventory();
    }
    public void SendNpcExpendition()
    {   
        istraveling = true;
        CountdownTimeDay countdownTimeDay = expenditionManager.AddComponent<CountdownTimeDay>();
        countdownTimeDay.timeScale = timeScale;
        countdownTimeDay.uIInventoryEX = this;
        countdownTimeDay.SetStartExpendition();
        npcSelecting.isWorking = true;
        DateTime dateTime = countdownTimeDay.timeManager.dateTime;

        // if (dateTime.day <= countdownTimeDay.finishDayCraftingTime)
        // {
        //     npcManager.listNpcWorking.Add(npcSelecying);
        // }
        // else
        // {
        //     npcManager.listNpcWorkingMoreOneDay.Add(npcSelecying);
        // }
        SetUIExButton(countdownTimeDay);

        uINpcSending.SetActive(true);
        this.gameObject.SetActive(false);
    }
    public void SetUIExButton(CountdownTimeDay countdownTimeDay)
    {
        if (expenditionManager.uIExOne == null)
        {
            expenditionManager.uIExOne = this.gameObject;
            countdownTimeDay.iconCompleteSend = expenditionManager.uIButtonEXOne.iconComplete;
            indexButtonExpendition = 1;
        }
        else
        {
            expenditionManager.uIExTwo = this.gameObject;
            countdownTimeDay.iconCompleteSend = expenditionManager.uIButtonEXTwo.iconComplete;
            indexButtonExpendition = 2;
        }


        string textdayFinish = "Day : " + finishDayCraftingTime.ToString() + "\n"
        + finishHourCraftingTime.ToString() + ":" + finishMinutesCraftingTime.ToString();
        Sprite spriteHeadNpc = npcManager.listHeadCoutume.FirstOrDefault(head => head.idHead == npcSelecting.idHead).spriteHead;

        expenditionManager.SetUIExButton(indexButtonExpendition, spriteHeadNpc, textdayFinish);
    }
    public void GoExpendition()
    {   
        expenditionManager.npcSelecying = npcSelecting;
        expenditionManager.listItemDataInventoryEqicment = listItemDataInventoryEquipment;
        expenditionManager.listItemDataInventoryslot = listItemDataInventorySlot;

        sceneSystem.SwitchScene(indexSceneExpendition);
    }

    public void CancleGoExpenditionAndGoHone()
    {
        CountdownTimeDay countdownTimeDay = expenditionManager.AddComponent<CountdownTimeDay>();
        countdownTimeDay.timeScale = timeScale;
        countdownTimeDay.uIInventoryEX = this;
        countdownTimeDay.SetStartExpendition();
        GameObject uIIconComplete = null;

        if (indexButtonExpendition == 1)
        {
            expenditionManager.uIExOne = this.gameObject;
            countdownTimeDay.iconCompleteSend = expenditionManager.uIButtonEXOne.iconComplete;
            uIIconComplete = expenditionManager.uIButtonEXOne.iconComplete;
        }
        else if (indexButtonExpendition == 2)
        {
            expenditionManager.uIExTwo = this.gameObject;
            countdownTimeDay.iconCompleteSend = expenditionManager.uIButtonEXTwo.iconComplete;
            uIIconComplete = expenditionManager.uIButtonEXTwo.iconComplete;
        }

        uIIconComplete.gameObject.SetActive(false);

        Sprite spriteHeadNpc = npcManager.listHeadCoutume.FirstOrDefault(head => head.idHead == npcSelecting.idHead).spriteHead;
        string textdayFinish = "Day : " + countdownTimeDay.finishDayCraftingTime.ToString() + "\n"
        + countdownTimeDay.finishHourCraftingTime.ToString() + ":" + countdownTimeDay.finishMinutesCraftingTime.ToString();

        expenditionManager.SetUIExButton(indexButtonExpendition, spriteHeadNpc, textdayFinish);
        this.gameObject.SetActive(false);
    }
    public void ResetSlotUIEx()
    {
        GameObject uIIconComplete = null;

        if (indexButtonExpendition == 1)
        {
            uIIconComplete = expenditionManager.uIButtonEXOne.iconComplete;
        }
        else if (indexButtonExpendition == 2)
        {
            uIIconComplete = expenditionManager.uIButtonEXTwo.iconComplete;
        }

        uIIconComplete.gameObject.SetActive(false);
        expenditionManager.SetUIExButton(indexButtonExpendition, null, null);
    }
    public void ChoiceLeaveOurSupplies()
    {
        globalstat.expiditionactiveeventactive = false;
        listItemDataInventorySlot.Clear();
        listItemDataCarInventorySlot.Clear();
        RefreshUIInventory();
        Destroy(this.gameObject);
    }
    public void ChoiceFightForIt()
    {
        // เปลีย่นแมพต่อสู้
    }
    public void Nottogive()
    {
        if(IsEventTriggered())
        {
            listItemDataInventorySlot.Clear();
            listItemDataCarInventorySlot.Clear();
        }
        globalstat.expiditionactiveeventactive = false;
        RefreshUIInventory();
        Destroy(this.gameObject);
    }
    public void ChoiceGiveThemHalfourSupplies()
    {
        if(isuseCar)
        {
            foreach (ItemData item in listItemDataCarInventorySlot)
            {
                if (item.count == 1)
                {
                    item.count = 0;
                }
                item.count /= 2;
            }
        }
        foreach (ItemData item in listItemDataInventorySlot)
        {
            if (item.count == 1)
            {
                item.count = 0;
            }
            item.count /= 2;
        }
        globalstat.expiditionactiveeventactive = false;
        RefreshUIInventory();
        Destroy(this.gameObject);
    }
    public bool IsEventTriggered()
    {
        float randomValue = Random.Range(0f, 100f); // สุ่มตัวเลขระหว่าง 0 ถึง 100
        return randomValue < riskValue; // คืนค่า true ถ้า randomValue น้อยกว่า riskValue
    }
    private void OnEnable()
    {
        RefreshUIInventory();
        if(istraveling)
        {
            UICarInventory.SetActive(false);
            UIBOxInventory.SetActive(false);
        }
        if (isArriveEx && !isArriveHome)
        {
            SetDataMoveSceneForEventExpendition();
            uINpcSending.SetActive(false);
            uINpcArriveEx.SetActive(true);
            if (uINpcGoBack.activeSelf)
            {
                uINpcArriveEx.SetActive(false);
            }
        }
        else if (isArriveHome && isArriveEx)
        {
            if (IsEventTriggered())
            {
                SetDataMoveSceneForEventExpendition();
                CloseButton.SetActive(false);
                globalstat.expiditionactiveeventactive = true;
                Debug.Log("Found Event");
                uINpcGoBack.SetActive(false);
                int randomValue = UnityEngine.Random.Range(0, 2);
                listEvnet.ElementAt(randomValue).gameObject.SetActive(true);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnDisable()
    {
        ConventDataUIToItemData();
    }
    private void OnDestroy()
    {
        int gameobjectsceneIndex = gameObject.scene.buildIndex;
        Debug.Log("Scene index Game object : " + gameobjectsceneIndex);
        if (gameobjectsceneIndex != 0)
        {
            return;
        }
        if (indexButtonExpendition == 1)
        {
            expenditionManager.uIExOne = null;
        }
        else
        {
            expenditionManager.uIExTwo = null;
        }
        ResetSlotUIEx();
        expenditionManager.SetUIExButton(indexButtonExpendition, null, null);
        ClearItemDataInAllInventorySlotToListDataBoxes();
        ClearItemDataInAllInventoryCarSlotToListDataBoxes();
        if(isuseCar)
            globalstat.UnaviableCar += 1;
        npcSelecting.isWorking = false;
        expenditionManager.listItemDataInventoryEqicment.Clear();
        expenditionManager.listItemDataInventoryslot.Clear();
    }
    public void EndSceneExpendition()
    {

        Debug.Log(" EndSceneExpendition");
        ClearItemDataInAllInventorySlotToListDataBoxes();
        ClearItemDataInAllInventoryCarSlotToListDataBoxes();
        // SetDataMoveSceneForEventExpendition();
        expenditionManager.listItemDataInventoryEqicment.Clear();
        expenditionManager.listItemDataInventoryslot.Clear();
        listItemDataInventoryEquipment.Clear();
        listItemDataInventorySlot.Clear();
        // this.gameObject.SetActive(false);
        // sceneSystem.SwitchScene(0);
    }
    public void DeleteObjectAndTransferInventory()
    {
        Debug.Log("Deleting object and transferring inventory data to data box");

        if(!istraveling)
        {
            ClearItemDataInAllInventorySlotToListDataBoxes();
            if(isuseCar)
                globalstat.availablecar += 1;
            Destroy(this.gameObject);
        }
        else 
            this.gameObject.SetActive(false);
    }
     public void ClearItemDataInAllInventoryCarSlotToListDataBoxes()
    {
        Debug.Log("ClearItemDataInAllInventoryCarSlotToListDataBoxes");

        foreach (InventorySlots slotsItem in listInvenrotyCarSlotsUI)
        {
            ItemClass itemClass = slotsItem.GetComponentInChildren<ItemClass>();
            if (itemClass != null)
            {
                // Convert ItemClass to ItemData
                ItemData itemData = inventoryItemPresent.ConventItemClassToItemData(itemClass);

                // Check if the item ID maps to a resource
                if (BuildManager.Instance != null)
                {
                    BuildManager.Instance.AddResource(itemData.idItem, itemData.count);
                }
                inventoryItemPresent.AddItem(itemData);
                // Destroy the item GameObject
                Destroy(itemClass.gameObject);
            }
        }
    }
    
}
