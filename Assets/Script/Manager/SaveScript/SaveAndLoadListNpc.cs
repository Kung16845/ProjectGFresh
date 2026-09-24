using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveAndLoadListNpc : MonoBehaviour
{
    public DataCollentListNpc dataCollentListNpc;
    public NpcManager npcManager;
    [SerializeField] private string savePathDataListNpc;

    private void Start()
    {
        savePathDataListNpc = Path.Combine(Application.dataPath, "datalistNpc.json");
        EnsureDependencies();
    }

    private void EnsureDependencies()
    {
        if (npcManager == null)
        {
            npcManager = GameManager.Instance != null && GameManager.Instance.npcManager != null
                ? GameManager.Instance.npcManager
                : FindFirstObjectByType<NpcManager>();
        }
    }

    public void SaveListNpc()
    {
        EnsureDependencies();
        AddDataCollectListNpc();
        string json = JsonUtility.ToJson(dataCollentListNpc, true);
        File.WriteAllText(savePathDataListNpc, json);
    }

    public void AddDataCollectListNpc()
    {
        EnsureDependencies();
        if (dataCollentListNpc == null)
        {
            dataCollentListNpc = new DataCollentListNpc();
        }

        if (npcManager != null)
        {
            dataCollentListNpc.listDataNPC = npcManager.listNpc;
        }
    }

    public void LoadDataListNpc()
    {
        EnsureDependencies();

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
        if (npcManager != null && dataCollentListNpc != null)
        {
            npcManager.listNpc = dataCollentListNpc.listDataNPC ?? new List<NpcClass>();
        }
    }

    public void CreateAllListNpc()
    {
        if (npcManager == null || npcManager.listNpc == null || npcManager.listNpc.Count == 0) return;

        for (int i = 0; i < npcManager.listNpc.Count; i++)
        {
            NpcClass npcClass = npcManager.listNpc[i];
            if (npcClass != null)
            {
                CreatePrefabNpcFromJson(npcClass);
            }
        }
    }

    public void CreatePrefabNpcFromJson(NpcClass npcJson)
    {
        if (npcJson == null || npcManager == null || npcManager.prefabNpc == null) return;

        NpcClass newNpc = new NpcClass
        {
            nameNpc = npcJson.nameNpc,
            roleNpc = npcJson.roleNpc,
            endurance = npcJson.endurance,
            combat = npcJson.combat,
            speed = npcJson.speed,
            countInventorySlot = 6,
            bed = 1,
            foodPerDay = 2,
            hp = 100f,
            morale = 50f,
            idnpc = npcJson.idnpc,
            idHead = npcJson.idHead,
            idBody = npcJson.idBody,
            idFeed = npcJson.idFeed
        };

        HeadCoutume headCoutume = FindHeadCostume(npcManager.listHeadCoutume, newNpc.idHead);
        BodyCoutume bodyCoutume = FindBodyCostume(npcManager.listBodyCoutume, newNpc.idBody);
        FeedCoutume feedCoutume = FindFeedCostume(npcManager.listFeedCoutume, newNpc.idFeed);

        Transform transformSpawnNpc = null;
        if (npcManager.listPointSpawnerNpc != null && npcManager.listPointSpawnerNpc.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, npcManager.listPointSpawnerNpc.Count);
            transformSpawnNpc = npcManager.listPointSpawnerNpc[randomIndex];
        }

        Transform spawnParent = transformSpawnNpc != null ? transformSpawnNpc.parent : null;
        Vector3 spawnPosition = transformSpawnNpc != null ? transformSpawnNpc.position : Vector3.zero;

        GameObject npcOBJ = Instantiate(npcManager.prefabNpc, spawnPosition, Quaternion.identity, spawnParent);
        NpcCoutume npcCoutume = npcOBJ.GetComponent<NpcCoutume>();
        if (npcCoutume != null)
        {
            npcCoutume.SetCostume(headCoutume, bodyCoutume, feedCoutume);
        }
    }

    private HeadCoutume FindHeadCostume(List<HeadCoutume> list, int id)
    {
        if (list == null) return null;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null && list[i].idHead == id) return list[i];
        }
        return null;
    }

    private BodyCoutume FindBodyCostume(List<BodyCoutume> list, int id)
    {
        if (list == null) return null;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null && list[i].idBody == id) return list[i];
        }
        return null;
    }

    private FeedCoutume FindFeedCostume(List<FeedCoutume> list, int id)
    {
        if (list == null) return null;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null && list[i].idFeed == id) return list[i];
        }
        return null;
    }

    public void ResetDataListNpc()
    {
        dataCollentListNpc = new DataCollentListNpc { listDataNPC = new List<NpcClass>() };
        string json = JsonUtility.ToJson(dataCollentListNpc, true);
        File.WriteAllText(savePathDataListNpc, json);
    }
}

[Serializable]
public class DataCollentListNpc
{
    public List<NpcClass> listDataNPC = new List<NpcClass>();
}
