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
    public void AddDataListBuilding()
    {
        foreach (BuiltBuildingInfo build in buildManager.builtBuildings)
        {
            InfoBuilding infoBuilding = new InfoBuilding();

            GameObject buildGameObject = build.building.gameObject;
            Building building = buildGameObject.GetComponent<Building>();
            UpgradeBuilding upgradeLevel = building.GetComponent<UpgradeBuilding>();

            infoBuilding.nameBuild = building.nameBuild;
            infoBuilding.dayBuildingFinist = building.finishDayBuildingTime;

            infoBuilding.transformX = building.transform.position.x;
            infoBuilding.transformY = building.transform.position.y;

            infoBuilding.levelBuild = upgradeLevel.currentLevel;
            infoBuilding.isBuildingUpgrad = upgradeLevel.isUpgradBuilding;
            infoBuilding.isBuildFinishedUpgrad = upgradeLevel.isFinishedUpgrad;
            infoBuilding.dayBuildingUpgradFinist = upgradeLevel.finishDayBuildingUpgradTime;

            if (infoBuilding.nameBuild == "Garden")
            {
                InfoBuildSmallGarden infoBuildSmallGarden = new InfoBuildSmallGarden();

                infoBuildSmallGarden.nameBuild = building.nameBuild;
                infoBuildSmallGarden.dayBuildingFinist = building.finishDayBuildingTime;

                infoBuildSmallGarden.transformX = building.transform.position.x;
                infoBuildSmallGarden.transformY = building.transform.position.y;

                infoBuildSmallGarden.levelBuild = upgradeLevel.currentLevel;
                infoBuildSmallGarden.isBuildingUpgrad = upgradeLevel.isUpgradBuilding;
                infoBuildSmallGarden.isBuildFinishedUpgrad = upgradeLevel.isFinishedUpgrad;
                infoBuildSmallGarden.dayBuildingUpgradFinist = upgradeLevel.finishDayBuildingUpgradTime;

                infoBuildSmallGarden.yielduration = buildGameObject.GetComponent<GardenBuilding>().yieldduration;

                dataColletBuilding.listinfoBuildSmallGardens.Add(infoBuildSmallGarden);
            }
            else if (infoBuilding.nameBuild == "Medium garden")
            {
                InfoBuildMediumGarden infoBuildMediumGarden = new InfoBuildMediumGarden();

                infoBuildMediumGarden.nameBuild = building.nameBuild;
                infoBuildMediumGarden.dayBuildingFinist = building.finishDayBuildingTime;

                infoBuildMediumGarden.transformX = building.transform.position.x;
                infoBuildMediumGarden.transformY = building.transform.position.y;

                infoBuildMediumGarden.levelBuild = upgradeLevel.currentLevel;
                infoBuildMediumGarden.isBuildingUpgrad = upgradeLevel.isUpgradBuilding;
                infoBuildMediumGarden.isBuildFinishedUpgrad = upgradeLevel.isFinishedUpgrad;
                infoBuildMediumGarden.dayBuildingUpgradFinist = upgradeLevel.finishDayBuildingUpgradTime;

                MediumGarden mediumGardenScript = buildGameObject.GetComponent<MediumGarden>();
                infoBuildMediumGarden.isHerbalPlant = mediumGardenScript.isHerbalPlanted;
                infoBuildMediumGarden.yielduration = mediumGardenScript.yieldduration;

                dataColletBuilding.listinfoBuildMediumGardens.Add(infoBuildMediumGarden);
            }
            else
            {

                dataColletBuilding.listInfoBuilding.Add(infoBuilding);
            }

        }
    }
    public void LoadBuildInScenes()
    {
        if (File.Exists(saveDataBuildingPath))
        {
            string json = File.ReadAllText(saveDataBuildingPath);
            dataColletBuilding = JsonUtility.FromJson<DataColletBuilding>(json);

            if (dataColletBuilding == null)
            {
                dataColletBuilding = new DataColletBuilding();
            }

            if (dataColletBuilding.listInfoBuilding == null)
            {
                dataColletBuilding.listInfoBuilding = new List<InfoBuilding>();
            }
            if (dataColletBuilding.listinfoBuildSmallGardens == null)
            {
                dataColletBuilding.listinfoBuildSmallGardens = new List<InfoBuildSmallGarden>();
            }
            if (dataColletBuilding.listinfoBuildMediumGardens == null)
            {
                dataColletBuilding.listinfoBuildMediumGardens = new List<InfoBuildMediumGarden>();
            }

            if (dataColletBuilding.listInfoBuilding.Count > 0)
            {
                foreach (InfoBuilding infoBuilding in dataColletBuilding.listInfoBuilding)
                {
                    CreateBuilding(infoBuilding);
                }
            }
            if (dataColletBuilding.listinfoBuildSmallGardens.Count > 0)
            {
                foreach (InfoBuildSmallGarden infoBuilding in dataColletBuilding.listinfoBuildSmallGardens)
                {
                    CreateBuildingSmallGarden(infoBuilding);
                }
            }

            if (dataColletBuilding.listinfoBuildMediumGardens.Count > 0)
            {
                foreach (InfoBuildMediumGarden infoBuilding in dataColletBuilding.listinfoBuildMediumGardens)
                {
                    CreateBuildingMediumGarden(infoBuilding);
                }
            }
        }
        else
        {
            dataColletBuilding = new DataColletBuilding();
            dataColletBuilding.listInfoBuilding = new List<InfoBuilding>();
            dataColletBuilding.listinfoBuildSmallGardens = new List<InfoBuildSmallGarden>();
            dataColletBuilding.listinfoBuildMediumGardens = new List<InfoBuildMediumGarden>();
        }
    }
    public void CreateBuilding(InfoBuilding infoBuilding)
    {
        Building newBuildingObject = Instantiate(buildManager.listALLBuilding.FirstOrDefault(build
        => build.nameBuild == infoBuilding.nameBuild));

        newBuildingObject.nameBuild = infoBuilding.nameBuild;
        newBuildingObject.finishDayBuildingTime = infoBuilding.dayBuildingFinist;

        Vector2 newVector = new Vector2(infoBuilding.transformX, infoBuilding.transformY);
        newBuildingObject.transform.position = newVector;

        UpgradeBuilding upgradeBuildingScript = newBuildingObject.GetComponent<UpgradeBuilding>();
        upgradeBuildingScript.currentLevel = infoBuilding.levelBuild;
        upgradeBuildingScript.isUpgradBuilding = infoBuilding.isBuildingUpgrad;
        upgradeBuildingScript.isFinishedUpgrad = infoBuilding.isBuildFinishedUpgrad;
        upgradeBuildingScript.finishDayBuildingUpgradTime = infoBuilding.dayBuildingUpgradFinist;

        Tile tile = buildManager.tiles.FirstOrDefault(tile => tile.transform.position.x == newVector.x && tile.transform.position.y == newVector.y);
        tile.isOccupied = true;

        BuiltBuildingInfo newBuiltBuildingInfo = new BuiltBuildingInfo(newBuildingObject, infoBuilding.levelBuild, null, newBuildingObject.buildingType);
        buildManager.builtBuildings.Add(newBuiltBuildingInfo);
    }
    public void CreateBuildingSmallGarden(InfoBuildSmallGarden infoBuildSmallGarden)
    {
        Building newbuildSmallGarden = Instantiate(buildManager.listALLBuilding.FirstOrDefault(build =>
        build.nameBuild == infoBuildSmallGarden.nameBuild));

        newbuildSmallGarden.nameBuild = infoBuildSmallGarden.nameBuild;
        newbuildSmallGarden.finishDayBuildingTime = infoBuildSmallGarden.dayBuildingFinist;

        Vector2 newVector = new Vector2(infoBuildSmallGarden.transformX, infoBuildSmallGarden.transformY);
        newbuildSmallGarden.transform.position = newVector;

        UpgradeBuilding upgradeBuildingScript = newbuildSmallGarden.GetComponent<UpgradeBuilding>();
        upgradeBuildingScript.currentLevel = infoBuildSmallGarden.levelBuild;
        upgradeBuildingScript.isUpgradBuilding = infoBuildSmallGarden.isBuildingUpgrad;
        upgradeBuildingScript.isFinishedUpgrad = infoBuildSmallGarden.isBuildFinishedUpgrad;
        upgradeBuildingScript.finishDayBuildingUpgradTime = infoBuildSmallGarden.dayBuildingUpgradFinist;

        GardenBuilding gardenBuildingScript = newbuildSmallGarden.GetComponent<GardenBuilding>();
        gardenBuildingScript.yieldduration = infoBuildSmallGarden.yielduration;

        Tile tile = buildManager.tiles.FirstOrDefault(tile => tile.transform.position.x == newVector.x && tile.transform.position.y == newVector.y);
        tile.isOccupied = true;

        BuiltBuildingInfo newBuiltBuildingInfo = new BuiltBuildingInfo(newbuildSmallGarden, infoBuildSmallGarden.levelBuild, null,newbuildSmallGarden.buildingType);
        buildManager.builtBuildings.Add(newBuiltBuildingInfo);

    }
    public void CreateBuildingMediumGarden(InfoBuildMediumGarden infoBuildMediumlGarden)
    {
        Building newbuildMediumGarden = Instantiate(buildManager.listALLBuilding.FirstOrDefault(build
        => build.nameBuild == infoBuildMediumlGarden.nameBuild));

        newbuildMediumGarden.nameBuild = infoBuildMediumlGarden.nameBuild;
        newbuildMediumGarden.finishDayBuildingTime = infoBuildMediumlGarden.dayBuildingFinist;

        Vector2 newVector = new Vector2(infoBuildMediumlGarden.transformX, infoBuildMediumlGarden.transformY);
        newbuildMediumGarden.transform.position = newVector;

        UpgradeBuilding upgradeBuildingScript = newbuildMediumGarden.GetComponent<UpgradeBuilding>();
        upgradeBuildingScript.currentLevel = infoBuildMediumlGarden.levelBuild;
        upgradeBuildingScript.isUpgradBuilding = infoBuildMediumlGarden.isBuildingUpgrad;
        upgradeBuildingScript.isFinishedUpgrad = infoBuildMediumlGarden.isBuildFinishedUpgrad;
        upgradeBuildingScript.finishDayBuildingUpgradTime = infoBuildMediumlGarden.dayBuildingUpgradFinist;

        MediumGarden gardenBuildingScript = newbuildMediumGarden.GetComponent<MediumGarden>();
        gardenBuildingScript.yieldduration = infoBuildMediumlGarden.yielduration;
        gardenBuildingScript.isHerbalPlanted = infoBuildMediumlGarden.isHerbalPlant;

        Tile tile = buildManager.tiles.FirstOrDefault(tile => tile.transform.position.x == newVector.x && tile.transform.position.y == newVector.y);
        tile.isOccupied = true;

        BuiltBuildingInfo newBuiltBuildingInfo = new BuiltBuildingInfo(newbuildMediumGarden, infoBuildMediumlGarden.levelBuild, null,newbuildMediumGarden.buildingType);
        buildManager.builtBuildings.Add(newBuiltBuildingInfo);
    }
    public void ResetDataBuilding()
    {
        dataColletBuilding = new DataColletBuilding();
        dataColletBuilding.listInfoBuilding = new List<InfoBuilding>();
        dataColletBuilding.listinfoBuildSmallGardens = new List<InfoBuildSmallGarden>();
        dataColletBuilding.listinfoBuildMediumGardens = new List<InfoBuildMediumGarden>();
        string json = JsonUtility.ToJson(dataColletBuilding, true);
        File.WriteAllText(saveDataBuildingPath, json);
    }
}
[Serializable]
public class DataColletBuilding
{   
    public List<InfoBuilding> listInfoBuilding;
    public List<InfoBuildSmallGarden> listinfoBuildSmallGardens;
    public List<InfoBuildMediumGarden> listinfoBuildMediumGardens;
}
[Serializable]
public class InfoBuilding
{
    public float transformX;
    public float transformY;
    public string nameBuild;
    public int levelBuild;
    public int dayBuildingFinist;
    public int dayBuildingUpgradFinist;
    public bool isBuildingUpgrad;
    public bool isBuildFinishedUpgrad;
}
[Serializable]
public class InfoBuildWorkshop : InfoBuilding
{
    //ไม่ต้องเก็บค่าอะไร
}
[Serializable]
public class InfoBuildWaterPump : InfoBuilding
{
    //ไม่ต้องเก็บค่าอะไร
}
[Serializable]
public class InfoBuildSmallGarden : InfoBuilding
{
    public int yielduration;
}
[Serializable]
public class InfoBuildSolar : InfoBuilding
{
    //ไม่ต้องเก็บค่าอะไร
}
[Serializable]
public class InfoBuildBeacon : InfoBuilding
{
    //ไม่ต้องเก็บค่าอะไร
}
[Serializable]
public class InfoBuildSmallBed : InfoBuilding
{
    //ไม่ต้องเก็บค่าอะไร
}
[Serializable]
public class InfoBuildLounge : InfoBuilding
{
    //ไม่ต้องเก็บค่าอะไร
}
[Serializable]
public class InfoBuildChemicallab : InfoBuilding
{
    //ไม่ต้องเก็บค่าอะไร
}
[Serializable]
public class InfoBuildClinic : InfoBuilding
{
    //ไม่ต้องเก็บค่าอะไร
}
[Serializable]
public class InfoBuildMediumBed : InfoBuilding
{
    //ไม่ต้องเก็บค่าอะไร
}
[Serializable]
public class InfoBuildMoonshine : InfoBuilding
{
    //ไม่ต้องเก็บค่าอะไร
}
[Serializable]
public class InfoBuildMediumGarden : InfoBuilding
{
    public int yielduration;
    public bool isHerbalPlant;
}
[Serializable]
public class InfoBuildCarWorkshop : InfoBuilding
{
    //ไม่ต้องเก็บค่าอะไร
}
public class InfoBuildFieldHospital : InfoBuilding
{
    //ไม่ต้องเก็บค่าอะไร
}
public class InfoBuildWatchTower : InfoBuilding
{
    //ไม่ต้องเก็บค่าอะไร
}