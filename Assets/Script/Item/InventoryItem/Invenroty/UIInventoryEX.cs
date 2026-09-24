using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInventoryEX : UIInventory
{
    public List<InventorySlots> listInvenrotyCarSlotsUI = new List<InventorySlots>();
    public List<ItemData> listItemDataCarInventorySlot = new List<ItemData>();
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
    public List<GameObject> listEvnet = new List<GameObject>();

    private void Awake()
    {
        SetValuableUIInventory();
        if (GameManager.Instance != null)
        {
            expenditionManager = GameManager.Instance.expenditionManager;
        }
    }

    public void Start()
    {
        if (isuseCar && UICarInventory != null)
        {
            UICarInventory.SetActive(true);
        }

        if (GameManager.Instance != null)
        {
            globalstat = GameManager.Instance.globalstat;
        }

        sceneSystem = FindFirstObjectByType<SceneSystem>();
        SetPlayerExpendition();

        if (expenditionManager != null)
        {
            if (indexButtonExpendition == 1)
            {
                expenditionManager.uIExOne = gameObject;
            }
            else if (indexButtonExpendition == 2)
            {
                expenditionManager.uIExTwo = gameObject;
            }
        }
    }

    public void ConventAllUIItemInListCarInventorySlotToListItemData(List<ItemData> listSlotItemDatas)
    {
        if (listSlotItemDatas == null || listInvenrotyCarSlotsUI == null) return;

        listSlotItemDatas.Clear();
        for (int i = 0; i < listInvenrotyCarSlotsUI.Count; i++)
        {
            var slot = listInvenrotyCarSlotsUI[i];
            if (slot == null) continue;

            ItemClass itemClass = slot.GetComponentInChildren<ItemClass>();
            if (itemClass != null)
            {
                listSlotItemDatas.Add(itemClass.ToItemData());
            }
        }
    }

    public void SetPlayerExpendition()
    {
        if (!isExpenditon) return;

        if (GameManager.Instance != null)
        {
            expenditionManager = GameManager.Instance.expenditionManager;
            inventoryItemPresent = GameManager.Instance.inventoryItemPresent;
            npcManager = GameManager.Instance.npcManager;
        }

        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (expenditionManager != null && playerMovement != null)
        {
            expenditionManager.playerObject = playerMovement.gameObject;
        }

        statAmplifier = FindFirstObjectByType<StatAmplifier>();

        if (expenditionManager != null)
        {
            listItemDataCarInventorySlot = expenditionManager.listItemDataCarInventory;
            npcSelecting = expenditionManager.npcSelecying;
            iswalk = expenditionManager.iswalk;
            isuseCar = expenditionManager.isuseCar;
            isuseTunnel = expenditionManager.isuseTunnel;
        }

        if (npcManager != null && npcSelecting != null)
        {
            npcManager.uIInventory = this;
            npcManager.levelCombatText = levelCombatText;
            npcManager.levelEnduranceText = levelEnduranceText;
            npcManager.levelSpeedText = levelSpeedText;
            npcManager.specialistNpcText = specialistNpcText;

            if (levelCombatText != null) levelCombatText.text = npcSelecting.combat.ToString();
            if (levelEnduranceText != null) levelEnduranceText.text = npcSelecting.endurance.ToString();
            if (levelSpeedText != null) levelSpeedText.text = npcSelecting.speed.ToString();
            if (specialistNpcText != null) specialistNpcText.text = npcSelecting.roleNpc.ToString();

            if (dropdown != null)
            {
                dropdown.ClearOptions();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData { text = npcSelecting.nameNpc };
                dropdown.AddOptions(new List<TMP_Dropdown.OptionData> { option });
            }

            if (spriteHeadNpc != null && npcManager.listHeadCoutume != null)
            {
                for (int i = 0; i < npcManager.listHeadCoutume.Count; i++)
                {
                    if (npcManager.listHeadCoutume[i] != null && npcManager.listHeadCoutume[i].idHead == npcSelecting.idHead)
                    {
                        spriteHeadNpc.sprite = npcManager.listHeadCoutume[i].spriteHead;
                        break;
                    }
                }
            }

            if (expenditionManager != null)
            {
                SetInventoryItemDataEx(expenditionManager.listItemDataInventoryslot, expenditionManager.listItemDataInventoryEqicment);
                if (expenditionManager.playerObject != null)
                {
                    SetCostumeNpcExpentdition(npcSelecting, expenditionManager.playerObject);
                }
            }

            if (statAmplifier != null)
            {
                statAmplifier.SetStatAmplifer(npcSelecting);
                statAmplifier.endurance = npcSelecting.endurance;
                statAmplifier.combat = npcSelecting.combat;
                statAmplifier.speed = npcSelecting.speed;
                statAmplifier.specialistRole = npcSelecting.roleNpc;
                statAmplifier.InitializeAmplifiers();
            }
        }

        if (isuseCar && UICarInventory != null)
        {
            UICarInventory.SetActive(true);
        }

        LoadCarSlotsFromData();
        RefreshUIInventory();
    }

    public override void RefreshUIInventory()
    {
        base.RefreshUIInventory();

        if (listItemDataCarInventorySlot != null && listItemDataCarInventorySlot.Count >= 1)
        {
            CombineAndSplitItems(listItemDataCarInventorySlot);
            RefreshCarInventorySlots();
        }
    }

    public void RefreshCarInventorySlots()
    {
        ClearAllChildInvenrotyCarSlot();
        if (listItemDataCarInventorySlot == null || listInvenrotyCarSlotsUI == null) return;

        listItemDataCarInventorySlot.Sort((a, b) => a.idItem.CompareTo(b.idItem));

        int limit = Mathf.Min(12, listInvenrotyCarSlotsUI.Count);
        for (int i = 0; i < limit; i++)
        {
            if (i < listItemDataCarInventorySlot.Count && listInvenrotyCarSlotsUI[i] != null)
            {
                CreateUIItem(listItemDataCarInventorySlot[i], listInvenrotyCarSlotsUI[i]);
            }
        }
    }

    public void ClearAllChildInvenrotyCarSlot()
    {
        if (listInvenrotyCarSlotsUI == null) return;

        for (int i = 0; i < listInvenrotyCarSlotsUI.Count; i++)
        {
            var carSlot = listInvenrotyCarSlotsUI[i];
            if (carSlot == null) continue;

            Transform slotTransform = carSlot.transform;
            for (int c = slotTransform.childCount - 1; c >= 0; c--)
            {
                Transform child = slotTransform.GetChild(c);
                if (child != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    public override void ConventDataUIToItemData()
    {
        base.ConventDataUIToItemData();
        SyncCarSlotsToItemData();
    }

    private void SyncCarSlotsToItemData()
    {
        if (listItemDataCarInventorySlot == null)
        {
            listItemDataCarInventorySlot = new List<ItemData>();
        }
        listItemDataCarInventorySlot.Clear();

        if (listInvenrotyCarSlotsUI == null) return;

        InventoryItemPresent presenter = (inventoryItemPresent != null) 
            ? inventoryItemPresent 
            : InventoryItemPresent.Instance;

        for (int i = 0; i < listInvenrotyCarSlotsUI.Count; i++)
        {
            var slot = listInvenrotyCarSlotsUI[i];
            if (slot == null) continue;

            var itemClass = slot.GetComponentInChildren<ItemClass>();
            if (itemClass != null)
            {
                var itemData = presenter != null 
                    ? presenter.ConventItemClassToItemData(itemClass) 
                    : itemClass.ToItemData();

                bool exists = false;
                for (int s = 0; s < listItemDataCarInventorySlot.Count; s++)
                {
                    if (listItemDataCarInventorySlot[s].idItem == itemData.idItem && 
                        listItemDataCarInventorySlot[s].count == itemData.count)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    listItemDataCarInventorySlot.Add(itemData);
                }
            }
        }
    }

    public void BindCarSlotsToData()
    {
        ClearAllChildInvenrotyCarSlot();
        if (listInvenrotyCarSlotsUI == null || listItemDataCarInventorySlot == null) return;

        for (int i = 0; i < listInvenrotyCarSlotsUI.Count; i++)
        {
            if (i < listItemDataCarInventorySlot.Count && listInvenrotyCarSlotsUI[i] != null)
            {
                CreateUIItem(listItemDataCarInventorySlot[i], listInvenrotyCarSlotsUI[i]);
            }
        }
    }

    public void LoadCarSlotsFromData()
    {
        ClearAllChildInvenrotyCarSlot();
        if (listItemDataCarInventorySlot == null || listInvenrotyCarSlotsUI == null) return;

        listItemDataCarInventorySlot.Sort((a, b) => a.idItem.CompareTo(b.idItem));

        for (int i = 0; i < listItemDataCarInventorySlot.Count; i++)
        {
            ItemData itemData = listItemDataCarInventorySlot[i];
            InventorySlots availableSlot = null;

            for (int s = 0; s < listInvenrotyCarSlotsUI.Count; s++)
            {
                InventorySlots slot = listInvenrotyCarSlotsUI[s];
                if (slot != null && slot.transform.childCount == 0)
                {
                    availableSlot = slot;
                    break;
                }
            }

            if (availableSlot != null)
            {
                CreateUIItem(itemData, availableSlot);
            }
        }
    }

    public void CallFuntionAddListenerButton()
    {
        if (expenditionManager == null) return;

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
        if (expenditionManager == null) return;

        expenditionManager.iswalk = iswalk;
        expenditionManager.isuseCar = isuseCar;
        expenditionManager.isuseTunnel = isuseTunnel;
        expenditionManager.npcSelecying = npcSelecting;
        expenditionManager.listItemDataInventoryEqicment = listItemDataInventoryEquipment;
        expenditionManager.listItemDataInventoryslot = listItemDataInventorySlot;
        expenditionManager.listItemDataCarInventory = listItemDataCarInventorySlot;
    }

    public void SetInventoryItemDataEx(List<ItemData> listDataInventoryslot, List<ItemData> listDataInventoryEqicment)
    {
        listItemDataInventorySlot = listDataInventoryslot ?? new List<ItemData>();
        listItemDataInventoryEquipment = listDataInventoryEqicment ?? new List<ItemData>();
        RefreshUIInventory();
    }

    public void SendNpcExpendition()
    {   
        if (expenditionManager == null) return;

        istraveling = true;
        CountdownTimeDay countdownTimeDay = expenditionManager.gameObject.AddComponent<CountdownTimeDay>();
        countdownTimeDay.timeScale = timeScale;
        countdownTimeDay.uIInventoryEX = this;
        countdownTimeDay.SetStartExpendition();

        if (npcSelecting != null)
        {
            npcSelecting.isWorking = true;
        }

        SetUIExButton(countdownTimeDay);

        if (uINpcSending != null) uINpcSending.SetActive(true);
        gameObject.SetActive(false);
    }

    public void SetUIExButton(CountdownTimeDay countdownTimeDay)
    {
        if (expenditionManager == null || countdownTimeDay == null) return;

        if (expenditionManager.uIExOne == null)
        {
            expenditionManager.uIExOne = gameObject;
            if (expenditionManager.uIButtonEXOne != null)
            {
                countdownTimeDay.iconCompleteSend = expenditionManager.uIButtonEXOne.iconComplete;
            }
            indexButtonExpendition = 1;
        }
        else
        {
            expenditionManager.uIExTwo = gameObject;
            if (expenditionManager.uIButtonEXTwo != null)
            {
                countdownTimeDay.iconCompleteSend = expenditionManager.uIButtonEXTwo.iconComplete;
            }
            indexButtonExpendition = 2;
        }

        string textdayFinish = $"Day : {finishDayCraftingTime}\n{finishHourCraftingTime}:{finishMinutesCraftingTime:D2}";
        Sprite headSprite = null;

        if (npcManager != null && npcManager.listHeadCoutume != null && npcSelecting != null)
        {
            for (int i = 0; i < npcManager.listHeadCoutume.Count; i++)
            {
                if (npcManager.listHeadCoutume[i] != null && npcManager.listHeadCoutume[i].idHead == npcSelecting.idHead)
                {
                    headSprite = npcManager.listHeadCoutume[i].spriteHead;
                    break;
                }
            }
        }

        expenditionManager.SetUIExButton(indexButtonExpendition, headSprite, textdayFinish);
    }

    public void GoExpendition()
    {   
        if (expenditionManager != null)
        {
            expenditionManager.npcSelecying = npcSelecting;
            expenditionManager.listItemDataInventoryEqicment = listItemDataInventoryEquipment;
            expenditionManager.listItemDataInventoryslot = listItemDataInventorySlot;
        }

        if (sceneSystem != null)
        {
            sceneSystem.SwitchScene(indexSceneExpendition);
        }
    }

    public void CancleGoExpenditionAndGoHone()
    {
        if (expenditionManager == null) return;

        CountdownTimeDay countdownTimeDay = expenditionManager.gameObject.AddComponent<CountdownTimeDay>();
        countdownTimeDay.timeScale = timeScale;
        countdownTimeDay.uIInventoryEX = this;
        countdownTimeDay.SetStartExpendition();
        GameObject uIIconComplete = null;

        if (indexButtonExpendition == 1)
        {
            expenditionManager.uIExOne = gameObject;
            if (expenditionManager.uIButtonEXOne != null)
            {
                countdownTimeDay.iconCompleteSend = expenditionManager.uIButtonEXOne.iconComplete;
                uIIconComplete = expenditionManager.uIButtonEXOne.iconComplete;
            }
        }
        else if (indexButtonExpendition == 2)
        {
            expenditionManager.uIExTwo = gameObject;
            if (expenditionManager.uIButtonEXTwo != null)
            {
                countdownTimeDay.iconCompleteSend = expenditionManager.uIButtonEXTwo.iconComplete;
                uIIconComplete = expenditionManager.uIButtonEXTwo.iconComplete;
            }
        }

        if (uIIconComplete != null)
        {
            uIIconComplete.SetActive(false);
        }

        Sprite headSprite = null;
        if (npcManager != null && npcManager.listHeadCoutume != null && npcSelecting != null)
        {
            for (int i = 0; i < npcManager.listHeadCoutume.Count; i++)
            {
                if (npcManager.listHeadCoutume[i] != null && npcManager.listHeadCoutume[i].idHead == npcSelecting.idHead)
                {
                    headSprite = npcManager.listHeadCoutume[i].spriteHead;
                    break;
                }
            }
        }

        string textdayFinish = $"Day : {countdownTimeDay.finishDayCraftingTime}\n{countdownTimeDay.finishHourCraftingTime}:{countdownTimeDay.finishMinutesCraftingTime:D2}";
        expenditionManager.SetUIExButton(indexButtonExpendition, headSprite, textdayFinish);
        gameObject.SetActive(false);
    }

    public void ResetSlotUIEx()
    {
        if (expenditionManager == null) return;

        GameObject uIIconComplete = null;

        if (indexButtonExpendition == 1 && expenditionManager.uIButtonEXOne != null)
        {
            uIIconComplete = expenditionManager.uIButtonEXOne.iconComplete;
        }
        else if (indexButtonExpendition == 2 && expenditionManager.uIButtonEXTwo != null)
        {
            uIIconComplete = expenditionManager.uIButtonEXTwo.iconComplete;
        }

        if (uIIconComplete != null)
        {
            uIIconComplete.SetActive(false);
        }
        expenditionManager.SetUIExButton(indexButtonExpendition, null, null);
    }

    public void ChoiceLeaveOurSupplies()
    {
        if (globalstat != null)
        {
            globalstat.expiditionactiveeventactive = false;
        }
        listItemDataInventorySlot.Clear();
        if (listItemDataCarInventorySlot != null)
        {
            listItemDataCarInventorySlot.Clear();
        }
        RefreshUIInventory();
        Destroy(gameObject);
    }

    public void ChoiceFightForIt()
    {
    }

    public void Nottogive()
    {
        if (IsEventTriggered())
        {
            listItemDataInventorySlot.Clear();
            if (listItemDataCarInventorySlot != null)
            {
                listItemDataCarInventorySlot.Clear();
            }
        }
        if (globalstat != null)
        {
            globalstat.expiditionactiveeventactive = false;
        }
        RefreshUIInventory();
        Destroy(gameObject);
    }

    public void ChoiceGiveThemHalfourSupplies()
    {
        if (isuseCar && listItemDataCarInventorySlot != null)
        {
            for (int i = 0; i < listItemDataCarInventorySlot.Count; i++)
            {
                ItemData item = listItemDataCarInventorySlot[i];
                if (item != null)
                {
                    item.count = (item.count <= 1) ? 0 : item.count / 2;
                }
            }
        }

        for (int i = 0; i < listItemDataInventorySlot.Count; i++)
        {
            ItemData item = listItemDataInventorySlot[i];
            if (item != null)
            {
                item.count = (item.count <= 1) ? 0 : item.count / 2;
            }
        }

        if (globalstat != null)
        {
            globalstat.expiditionactiveeventactive = false;
        }
        RefreshUIInventory();
        Destroy(gameObject);
    }

    public bool IsEventTriggered()
    {
        float randomValue = UnityEngine.Random.Range(0f, 100f);
        return randomValue < riskValue;
    }

    private void OnEnable()
    {
        RefreshUIInventory();
        if (istraveling)
        {
            if (UICarInventory != null) UICarInventory.SetActive(false);
            if (UIBOxInventory != null) UIBOxInventory.SetActive(false);
        }
        if (isArriveEx && !isArriveHome)
        {
            SetDataMoveSceneForEventExpendition();
            if (uINpcSending != null) uINpcSending.SetActive(false);
            if (uINpcArriveEx != null) uINpcArriveEx.SetActive(true);
            if (uINpcGoBack != null && uINpcGoBack.activeSelf && uINpcArriveEx != null)
            {
                uINpcArriveEx.SetActive(false);
            }
        }
        else if (isArriveHome && isArriveEx)
        {
            if (IsEventTriggered())
            {
                SetDataMoveSceneForEventExpendition();
                if (CloseButton != null) CloseButton.SetActive(false);
                if (globalstat != null) globalstat.expiditionactiveeventactive = true;
                if (uINpcGoBack != null) uINpcGoBack.SetActive(false);
                if (listEvnet != null && listEvnet.Count > 0)
                {
                    int randomValue = UnityEngine.Random.Range(0, listEvnet.Count);
                    if (listEvnet[randomValue] != null)
                    {
                        listEvnet[randomValue].SetActive(true);
                    }
                }
            }
            else
            {
                Destroy(gameObject);
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
        if (gameobjectsceneIndex != 0)
        {
            return;
        }

        if (expenditionManager != null)
        {
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
            expenditionManager.listItemDataInventoryEqicment.Clear();
            expenditionManager.listItemDataInventoryslot.Clear();
        }

        ClearItemDataInAllInventorySlotToListDataBoxes();
        ClearItemDataInAllInventoryCarSlotToListDataBoxes();

        if (isuseCar && globalstat != null)
        {
            globalstat.UnaviableCar += 1;
        }

        if (npcSelecting != null)
        {
            npcSelecting.isWorking = false;
        }
    }

    public void EndSceneExpendition()
    {
        ClearItemDataInAllInventorySlotToListDataBoxes();
        ClearItemDataInAllInventoryCarSlotToListDataBoxes();
        if (expenditionManager != null)
        {
            expenditionManager.listItemDataInventoryEqicment.Clear();
            expenditionManager.listItemDataInventoryslot.Clear();
        }
        listItemDataInventoryEquipment.Clear();
        listItemDataInventorySlot.Clear();
    }

    public void DeleteObjectAndTransferInventory()
    {
        if (!istraveling)
        {
            ClearItemDataInAllInventorySlotToListDataBoxes();
            if (isuseCar && globalstat != null)
            {
                globalstat.availablecar += 1;
            }
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void ClearItemDataInAllInventoryCarSlotToListDataBoxes()
    {
        if (listInvenrotyCarSlotsUI == null) return;

        InventoryItemPresent presenter = (inventoryItemPresent != null) 
            ? inventoryItemPresent 
            : InventoryItemPresent.Instance;

        for (int i = 0; i < listInvenrotyCarSlotsUI.Count; i++)
        {
            InventorySlots slotsItem = listInvenrotyCarSlotsUI[i];
            if (slotsItem == null) continue;

            ItemClass itemClass = slotsItem.GetComponentInChildren<ItemClass>();
            if (itemClass != null)
            {
                ItemData itemData = itemClass.ToItemData();

                if (BuildManager.Instance != null)
                {
                    BuildManager.Instance.AddResource(itemData.idItem, itemData.count);
                }

                if (presenter != null)
                {
                    presenter.AddItem(itemData);
                }

                Destroy(itemClass.gameObject);
            }
        }
    }
}
