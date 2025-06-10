using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NpcManager : MonoBehaviour
{
    private string[] firstNames = { "John", "Jane", "Alex", "Emily", "Chris", "Sara" };
    private string[] lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia" };
    public int startIDNPC = 7000;
    [Header("List Coutume")]
    public List<HeadCoutume> listHeadCoutume = new List<HeadCoutume>();
    public List<BodyCoutume> listBodyCoutume = new List<BodyCoutume>();
    public List<FeedCoutume> listFeedCoutume = new List<FeedCoutume>();
    [Header("Npc")]
    public List<NpcClass> listNpc = new List<NpcClass>();
    // public List<NpcClass> listNpcWorking = new List<NpcClass>();
    // public List<NpcClass> listNpcWorkingMoreOneDay = new List<NpcClass>();
    public UIInventory uIInventory;
    public InventoryItemPresent inventoryItemPresent;
    public List<Transform> listPointSpawnerNpc;
    public GameObject prefabNpc;
    [Header("TextMeshProUGUI")]
    public TextMeshProUGUI levelEnduranceText;
    public TextMeshProUGUI levelCombatText;
    public TextMeshProUGUI levelSpeedText;
    public TextMeshProUGUI specialistNpcText;
    private void Awake()
    {
        // StartGameCreateGropNpx();
    }
    private void Start()
    {
        inventoryItemPresent = GameManager.Instance.inventoryItemPresent;

        // dropdown = FindObjectOfType<TMP_Dropdown>();
        // dropdown.onValueChanged.AddListener(OnDropdownValueChanged);


        // SetOptionDropDown();
        // OnDropdownValueChanged(0);
    }
    public List<TMP_Dropdown.OptionData> SetNpcOptionDropDown()
    {

        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();

        foreach (NpcClass npcData in listNpc)
        {
            if (npcData.isWorking) continue;

            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = npcData.nameNpc.ToString();

            Sprite newSpriteHeadNpc = listHeadCoutume.FirstOrDefault(coutumeHead => coutumeHead.idHead == npcData.idHead).spriteHead;
            option.image = newSpriteHeadNpc;

            newOptions.Add(option);
        }

        return newOptions;
    }
    public void StartGameCreateGropNpc()
    {
        for (int i = 0; i < 5; i++)
        {
            startIDNPC++;
            CreateNpc(startIDNPC);
        }

    }
    public void CreateNpc(int idNpc)
    {

        NpcClass newNpc = new NpcClass();

        string randomFirstName = firstNames[Random.Range(0, firstNames.Length)];
        string randomLastName = lastNames[Random.Range(0, lastNames.Length)];

        newNpc.nameNpc = randomFirstName + " " + randomLastName;
        newNpc.roleNpc = (SpecialistRoleNpc)Random.Range(0, 8);

        // newNpc.endurance = Random.Range(1, 3);
        newNpc.endurance = Random.Range(2, 9);
        // newNpc.combat = Random.Range(1, 3);
        newNpc.combat = Random.Range(4, 9);
        // newNpc.speed = Random.Range(1, 3);
        newNpc.speed = Random.Range(2, 9);
        newNpc.countInventorySlot = 6;
        newNpc.bed = 1;
        newNpc.foodPerDay = 2;
        newNpc.hp = 100f;
        newNpc.morale = 50f;


        newNpc.idnpc = idNpc;
        newNpc.idHead = Random.Range(0, listHeadCoutume.Count);
        newNpc.idBody = Random.Range(0, listBodyCoutume.Count);
        newNpc.idFeed = Random.Range(0, listFeedCoutume.Count);

        HeadCoutume headCoutume = listHeadCoutume.FirstOrDefault(coutume => coutume.idHead == newNpc.idHead);
        BodyCoutume bodyCoutume = listBodyCoutume.FirstOrDefault(coutume => coutume.idBody == newNpc.idBody);
        FeedCoutume feedCoutume = listFeedCoutume.FirstOrDefault(coutume => coutume.idFeed == newNpc.idFeed);

        Transform transformSpawnNpc = listPointSpawnerNpc.ElementAt(Random.Range(0, listPointSpawnerNpc.Count));

        GameObject npcOBJ = Instantiate(prefabNpc, transformSpawnNpc);
        npcOBJ.transform.position = transformSpawnNpc.position;
        NpcCoutume npcCoutume = npcOBJ.GetComponent<NpcCoutume>();

        npcCoutume.SetCostume(headCoutume, bodyCoutume, feedCoutume);

        listNpc.Add(newNpc);
    }
    public void OnDropdownValueChanged(int selectedValue)
    {
        // Debug.Log("Dropdown index changed to: " + selectedValue);
        // Debug.Log("Dropdown Option changed to: " + dropdown.options[selectedValue].text);

        // NpcClass npcClassSelest = listNpc.FirstOrDefault(npc => npc.idnpc == selectedValue);
        NpcClass npcClassSelect = listNpc.ElementAt(selectedValue);
        uIInventory.npcSelecting = npcClassSelect;

        Sprite newSpriteHeadNpc = listHeadCoutume.FirstOrDefault(coutumeHead => coutumeHead.idHead == npcClassSelect.idHead).spriteHead;
        uIInventory.spriteHeadNpc.sprite = newSpriteHeadNpc;

        inventoryItemPresent.UnlockSlotInventory(npcClassSelect.countInventorySlot, npcClassSelect.roleNpc, uIInventory.listItemDataInventoryEquipment);

        SetText(npcClassSelect);
    }
    public void SetText(NpcClass npcClass)
    {
        levelEnduranceText.text = npcClass.endurance.ToString();
        levelCombatText.text = npcClass.combat.ToString();
        levelSpeedText.text = npcClass.speed.ToString();
        specialistNpcText.text = npcClass.roleNpc.ToString();
    }
    public NpcClass GetNpcByClass(SpecialistRoleNpc role)
    {
        return listNpc.FirstOrDefault(n => n.roleNpc == role);
    }
    public NpcClass GetNpcById(int idNpc)
    {
        return listNpc.FirstOrDefault(n => n.idnpc == idNpc);
    }
    public HeadCoutume GetSpriteHeadCoutumeById(int idHead)
    {

        return listHeadCoutume.FirstOrDefault(c => c.idHead == idHead); ;

    }
    public BodyCoutume GetSpriteBodyCoutumeById(int idBody)
    {
        return listBodyCoutume.FirstOrDefault(c => c.idBody == idBody);
        
    }
    public FeedCoutume GetSpriteFeedCoutumeById(int idFeed)
    {
        return listFeedCoutume.FirstOrDefault(c => c.idFeed == idFeed);
        
    }
}
