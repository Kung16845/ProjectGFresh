using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
public class SaveAndLoadListNpc : MonoBehaviour
{
    public DataCollentListNpc dataCollentListNpc;
    public NpcManager npcManager;
    [SerializeField] private string savePathDataListNpc;
    // Start is called before the first frame update
    void Start()
    {

        savePathDataListNpc = Path.Combine(Application.dataPath, "datalistNpc.json");
        npcManager = GameManager.Instance.npcManager;
    }
    public void SaveListNpc()
    {
        AddDataCollectListNpc();
        string json = JsonUtility.ToJson(dataCollentListNpc, true);
        File.WriteAllText(savePathDataListNpc, json);
    }
    public void AddDataCollectListNpc()
    {
        dataCollentListNpc.listDataNPC = npcManager.listNpc;
    }
    public void LoadDataListNpc()
    {
        if (File.Exists(savePathDataListNpc))
        {
            string json = File.ReadAllText(savePathDataListNpc);
            dataCollentListNpc = JsonUtility.FromJson<DataCollentListNpc>(json);
            SetListDataNpc();
            CreateAllListNpc();
        }
        else
        {
            dataCollentListNpc = new DataCollentListNpc();
        }

    }
    public void SetListDataNpc()
    {
        npcManager.listNpc = dataCollentListNpc.listDataNPC;

    }
    public void CreateAllListNpc()
    {
        if (npcManager.listNpc.Count != 0)
        {
            foreach (NpcClass npcClass in npcManager.listNpc)
            {
                CreatePrefabNpcFromJson(npcClass);
            }
        }
    }
    public void CreatePrefabNpcFromJson(NpcClass npcJson)
    {
        NpcClass newNpc = new NpcClass();

        string randomFirstName = npcJson.nameNpc;
        string randomLastName = npcJson.nameNpc;

        newNpc.nameNpc = randomFirstName + " " + randomLastName;
        newNpc.roleNpc = npcJson.roleNpc;

        // newNpc.endurance = Random.Range(1, 3);
        newNpc.endurance = npcJson.endurance;
        // newNpc.combat = Random.Range(1, 3);
        newNpc.combat = npcJson.combat;
        // newNpc.speed = Random.Range(1, 3);
        newNpc.speed = npcJson.speed;
        newNpc.countInventorySlot = 6;
        newNpc.bed = 1;
        newNpc.foodPerDay = 2;
        newNpc.hp = 100f;
        newNpc.morale = 50f;


        newNpc.idnpc = npcJson.idnpc;
        newNpc.idHead = npcJson.idHead;
        newNpc.idBody = npcJson.idBody;
        newNpc.idFeed = npcJson.idFeed;

        HeadCoutume headCoutume = npcManager.listHeadCoutume.FirstOrDefault(coutume => coutume.idHead == newNpc.idHead);
        BodyCoutume bodyCoutume = npcManager.listBodyCoutume.FirstOrDefault(coutume => coutume.idBody == newNpc.idBody);
        FeedCoutume feedCoutume = npcManager.listFeedCoutume.FirstOrDefault(coutume => coutume.idFeed == newNpc.idFeed);

        Transform transformSpawnNpc = npcManager.listPointSpawnerNpc.ElementAt(UnityEngine.Random.Range(0, npcManager.listPointSpawnerNpc.Count));

        GameObject npcOBJ = Instantiate(npcManager.prefabNpc, transformSpawnNpc.parent);
        npcOBJ.transform.position = transformSpawnNpc.position;
        NpcCoutume npcCoutume = npcOBJ.GetComponent<NpcCoutume>();

        npcCoutume.SetCostume(headCoutume, bodyCoutume, feedCoutume);

    }
    public void ResetDataListNpc()
    {
        DataCollentListItemsBoxes dataCollentListItemsBoxes = new DataCollentListItemsBoxes();
        string json = JsonUtility.ToJson(dataCollentListNpc, true);
        File.WriteAllText(savePathDataListNpc, json);
    }
}

[Serializable]
public class DataCollentListNpc
{
    public List<NpcClass> listDataNPC;
}

