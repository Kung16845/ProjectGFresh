using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Linq;
using UnityEngine.Rendering.Universal;


public class SaveAndLoadBuildManager : MonoBehaviour
{

    public DataColletBuilding dataColletBuilding;
    public BuildManager buildManager;
    public GameManager gameManager;
    [SerializeField] private string saveDataBuildingPath;

    private void Start()
    {
        saveDataBuildingPath = Path.Combine(Application.dataPath, "data_ListBuildinds.json");
        gameManager = FindObjectOfType<GameManager>();
        buildManager = gameManager.buildManager;


    }
    public void SaveBuildInScenes()
    {
        ResetDataBuilding();
        AddDataListBuilding();
        string json = JsonUtility.ToJson(dataColletBuilding, true);
        File.WriteAllText(saveDataBuildingPath, json);
    }
    private void PopulateBaseInfo(InfoBuilding info, Building building, UpgradeBuilding upgrade)
    {
        info.nameBuild = building.nameBuild;
        info.finishBuildingHour = building.finishBuildingHour;
        info.transformX = building.transform.position.x;
        info.transformY = building.transform.position.y;
        info.levelBuild = upgrade.currentLevel;
        info.isBuildingUpgrad = upgrade.isUpgradBuilding;
        info.isBuildFinishedUpgrad = upgrade.isFinishedUpgrad;
        info.finishUpgradeHour = upgrade.finishUpgradeHour;
    }

    public void AddDataListBuilding()
    {
        foreach (BuiltBuildingInfo build in buildManager.builtBuildings)
        {
            GameObject buildGameObject = build.building.gameObject;
            Building building = buildGameObject.GetComponent<Building>();
            UpgradeBuilding upgrade = building.GetComponent<UpgradeBuilding>();

            if (building.nameBuild == "Garden")
            {
                InfoBuildSmallGarden info = new InfoBuildSmallGarden();
                PopulateBaseInfo(info, building, upgrade);
                info.yielduration = buildGameObject.GetComponent<GardenBuilding>().yieldduration;
                dataColletBuilding.listinfoBuildSmallGardens.Add(info);
            }
            else if (building.nameBuild == "Medium garden")
            {
                InfoBuildMediumGarden info = new InfoBuildMediumGarden();
                PopulateBaseInfo(info, building, upgrade);
                MediumGarden mg = buildGameObject.GetComponent<MediumGarden>();
                info.isHerbalPlant = mg.isHerbalPlanted;
                info.yielduration = mg.yieldduration;
                dataColletBuilding.listinfoBuildMediumGardens.Add(info);
            }
            else
            {
                InfoBuilding info = new InfoBuilding();
                PopulateBaseInfo(info, building, upgrade);
                dataColletBuilding.listInfoBuilding.Add(info);
            }
        }
    }
    public void LoadBuildInScenes()
    {
        if (File.Exists(saveDataBuildingPath))
        {
            string json = File.ReadAllText(saveDataBuildingPath);
            dataColletBuilding = JsonUtility.FromJson<DataColletBuilding>(json) ?? new DataColletBuilding();

            foreach (InfoBuilding infoBuilding in dataColletBuilding.listInfoBuilding)
                CreateBuilding(infoBuilding);

            foreach (InfoBuildSmallGarden infoBuilding in dataColletBuilding.listinfoBuildSmallGardens)
                CreateBuildingSmallGarden(infoBuilding);

            foreach (InfoBuildMediumGarden infoBuilding in dataColletBuilding.listinfoBuildMediumGardens)
                CreateBuildingMediumGarden(infoBuilding);
        }
        else
        {
            dataColletBuilding = new DataColletBuilding();
        }
    }
    private Building InstantiateAndSetupBuilding(InfoBuilding info)
    {
        Building newBuilding = Instantiate(
            buildManager.listALLBuilding.FirstOrDefault(b => b.nameBuild == info.nameBuild));

        newBuilding.nameBuild = info.nameBuild;
        newBuilding.finishBuildingHour = info.finishBuildingHour;
        newBuilding.transform.position = new Vector2(info.transformX, info.transformY);

        UpgradeBuilding upgrade = newBuilding.GetComponent<UpgradeBuilding>();
        upgrade.currentLevel = info.levelBuild;
        upgrade.isUpgradBuilding = info.isBuildingUpgrad;
        upgrade.isFinishedUpgrad = info.isBuildFinishedUpgrad;
        upgrade.finishUpgradeHour = info.finishUpgradeHour;

        Vector2 pos = newBuilding.transform.position;
        Tile tile = buildManager.tiles.FirstOrDefault(t => t.transform.position.x == pos.x && t.transform.position.y == pos.y);
        tile.isOccupied = true;

        buildManager.builtBuildings.Add(
            new BuiltBuildingInfo(newBuilding, info.levelBuild, null, newBuilding.buildingType));

        return newBuilding;
    }

    public void CreateBuilding(InfoBuilding info)
    {
        InstantiateAndSetupBuilding(info);
    }

    public void CreateBuildingSmallGarden(InfoBuildSmallGarden info)
    {
        Building b = InstantiateAndSetupBuilding(info);
        b.GetComponent<GardenBuilding>().yieldduration = info.yielduration;
    }

    public void CreateBuildingMediumGarden(InfoBuildMediumGarden info)
    {
        Building b = InstantiateAndSetupBuilding(info);
        MediumGarden mg = b.GetComponent<MediumGarden>();
        mg.yieldduration = info.yielduration;
        mg.isHerbalPlanted = info.isHerbalPlant;
    }
    public void ResetDataBuilding()
    {
        dataColletBuilding = new DataColletBuilding();
        string json = JsonUtility.ToJson(dataColletBuilding, true);
        File.WriteAllText(saveDataBuildingPath, json);
    }
}
[Serializable]
public class DataColletBuilding
{
    public List<InfoBuilding> listInfoBuilding = new List<InfoBuilding>();
    public List<InfoBuildSmallGarden> listinfoBuildSmallGardens = new List<InfoBuildSmallGarden>();
    public List<InfoBuildMediumGarden> listinfoBuildMediumGardens = new List<InfoBuildMediumGarden>();
}
[Serializable]
public class InfoBuilding
{
    public float transformX;
    public float transformY;
    public string nameBuild;
    public int levelBuild;
    public int finishBuildingHour;
    public int finishUpgradeHour;
    public bool isBuildingUpgrad;
    public bool isBuildFinishedUpgrad;
}
[Serializable]
public class InfoBuildSmallGarden : InfoBuilding
{
    public int yielduration;
}
[Serializable]
public class InfoBuildMediumGarden : InfoBuilding
{
    public int yielduration;
    public bool isHerbalPlant;
}