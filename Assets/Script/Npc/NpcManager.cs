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
    public List<NpcClass> listNpcWorking = new List<NpcClass>();
    // public List<NpcClass> listNpcWorkingMoreOneDay = new List<NpcClass>();
    public TMP_Dropdown dropdown;
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
        StartGameCreateGropNpx();
    }
    private void Start()
    {
        inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();

        // dropdown = FindObjectOfType<TMP_Dropdown>();
        // dropdown.onValueChanged.AddListener(OnDropdownValueChanged);


        // SetOptionDropDown();
        // OnDropdownValueChanged(0);
    }
    public void SetOptionDropDown()
    {
        dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();

        foreach (NpcClass npcData in listNpc)
        {   
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = npcData.nameNpc.ToString(); 
            
            Sprite newSpriteHeadNpc = listHeadCoutume.FirstOrDefault(coutumeHead => coutumeHead.idHead == npcData.idHead).spriteHead;
            option.image = newSpriteHeadNpc;

            newOptions.Add(option);
        }

        dropdown.AddOptions(newOptions);
    }
    public void StartGameCreateGropNpx()
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
        newNpc.foodPerDay= 2;
        newNpc.hp = 100f;
        newNpc.morale = 50f;
        
        
        newNpc.idnpc = idNpc;
        newNpc.idHead = Random.Range(0, listHeadCoutume.Count);
        newNpc.idBody = Random.Range(0, listBodyCoutume.Count);
        newNpc.idFeed = Random.Range(0, listFeedCoutume.Count);

        HeadCoutume headCoutume = listHeadCoutume.FirstOrDefault(coutume => coutume.idHead == newNpc.idHead);
        BodyCoutume bodyCoutume = listBodyCoutume.FirstOrDefault(coutume => coutume.idBody == newNpc.idBody);
        FeedCoutume feedCoutume = listFeedCoutume.FirstOrDefault(coutume => coutume.idFeed == newNpc.idFeed);

        Transform transformSpawnNpc = listPointSpawnerNpc.ElementAt(Random.Range(0,listPointSpawnerNpc.Count));

        GameObject npcOBJ = Instantiate(prefabNpc, transformSpawnNpc);
        npcOBJ.transform.position = transformSpawnNpc.position;
        NpcCoutume npcCoutume = npcOBJ.GetComponent<NpcCoutume>();

        npcCoutume.SetCostume(headCoutume, bodyCoutume, feedCoutume);

        listNpc.Add(newNpc);
    }
    public void NpcWorking(int idNpc, bool isWithinDay)
    {
        NpcClass npcWorking = listNpc.FirstOrDefault(npc => npc.idnpc == idNpc);

        // if (isWithinDay)
        //     listNpcWorking.Add(npcWorking);
        // else
        //     listNpcWorkingMoreOneDay.Add(npcWorking);

        listNpc.Remove(npcWorking);

        listNpcWorking.Add(npcWorking);
    }
    
    public void OnDropdownValueChanged(int selectedValue)
    {
        // Debug.Log("Dropdown index changed to: " + selectedValue);
        // Debug.Log("Dropdown Option changed to: " + dropdown.options[selectedValue].text);
     
        // NpcClass npcClassSelest = listNpc.FirstOrDefault(npc => npc.idnpc == selectedValue);
        NpcClass npcClassSelest = listNpc.ElementAt(selectedValue);
        uIInventory.npcSelecying = npcClassSelest;

        Sprite newSpriteHeadNpc = listHeadCoutume.FirstOrDefault(coutumeHead => coutumeHead.idHead == npcClassSelest.idHead).spriteHead;      
        uIInventory.spriteHeadNpc.sprite = newSpriteHeadNpc;

        inventoryItemPresent.UnlockSlotInventory(npcClassSelest.countInventorySlot, npcClassSelest.roleNpc,uIInventory.listItemDataInventoryEqicment);

        SetText(npcClassSelest);

        HandleSpecialistNpcChange(selectedValue);
    }
    public void SetText(NpcClass npcClass)
    {
        levelEnduranceText.text = npcClass.endurance.ToString();
        levelCombatText.text = npcClass.combat.ToString();
        levelSpeedText.text = npcClass.speed.ToString();
        specialistNpcText.text = npcClass.roleNpc.ToString();
    }
    public void HandleSpecialistNpcChange(int numSpecialistNpc)
    {
        SpecialistRoleNpc specialistNpc = (SpecialistRoleNpc)numSpecialistNpc;

        switch (specialistNpc)
        {
            case SpecialistRoleNpc.Handicraft:
                SetSpecialistNpc(specialistNpc);
                break;
            case SpecialistRoleNpc.Maintainance:
                SetSpecialistNpc(specialistNpc);
                break;
            case SpecialistRoleNpc.Network:
                SetSpecialistNpc(specialistNpc);
                break;
            case SpecialistRoleNpc.Scavenger:
                SetSpecialistNpc(specialistNpc);
                break;
            case SpecialistRoleNpc.Military_training:
                SetSpecialistNpc(specialistNpc);
                break;
            case SpecialistRoleNpc.Chemical:
                SetSpecialistNpc(specialistNpc);
                break;
            case SpecialistRoleNpc.Doctor:
                SetSpecialistNpc(specialistNpc);
                break;
            case SpecialistRoleNpc.Entertainer:
                SetSpecialistNpc(specialistNpc);
                break;
            default:
                Debug.LogWarning("Invalid SpecialistNpc selected");
                break;
        }
    }
    public void SetSpecialistNpc(SpecialistRoleNpc specialistNpc)
    {

        Debug.Log("Specialist Player : " + specialistNpc);

    }
    public void RemoveWorkerBySpecialist(SpecialistRoleNpc specialistToRemove)
    {
        // Find the first worker in the working list with the specified specialist role
        NpcClass npcToRemove = listNpcWorking.FirstOrDefault(npc => npc.roleNpc == specialistToRemove);

        if (npcToRemove != null)
        {
            // Remove the worker from the working list
            listNpcWorking.Remove(npcToRemove);

            // Optionally, move the worker back to the main NPC list
            listNpc.Add(npcToRemove);

            Debug.Log($"Removed worker with specialist role: {specialistToRemove} and returned them to available NPC list.");
        }
        else
        {
            Debug.LogWarning($"No worker found with specialist role: {specialistToRemove} in the working list.");
        }
    }
    public void MoveNpcToWorking(int npcId)
    {
        // Find the NPC in the normal list
        NpcClass npcToMove = listNpc.FirstOrDefault(npc => npc.idnpc == npcId);
        if (npcToMove != null)
        {
            // Remove from normal list
            listNpc.Remove(npcToMove);
            // Add to working list
            listNpcWorking.Add(npcToMove);
            // Set isActive to false
            npcToMove.isActive = false;
            Debug.Log($"NPC {npcToMove.nameNpc} (ID: {npcId}) moved to working list and set as inactive.");
        }
        else
        {
            Debug.LogWarning($"No NPC with ID {npcId} found in the normal NPC list.");
        }
    }

    public void MoveNpcBackToNormalList(int npcId)
    {
        // Find the NPC in the working list
        NpcClass npcToMoveBack = listNpcWorking.FirstOrDefault(npc => npc.idnpc == npcId);
        if (npcToMoveBack != null)
        {
            // Remove from working list
            listNpcWorking.Remove(npcToMoveBack);
            // Add back to normal list
            listNpc.Add(npcToMoveBack);
            // Set isActive to true
            npcToMoveBack.isActive = true;
            Debug.Log($"NPC {npcToMoveBack.nameNpc} (ID: {npcId}) moved back to normal list and set as active.");
        }
        else
        {
            Debug.LogWarning($"No NPC with ID {npcId} found in the working list.");
        }
    }

}
