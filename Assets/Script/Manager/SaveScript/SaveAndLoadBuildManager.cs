using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveAndLoadBuildManager : MonoBehaviour
{
    public DataColletBuilding dataColletBuilding;
    public BuildManager buildManager;
    public GameManager gameManager;
    [SerializeField] private string saveDataBuildingPath;

    private void Start()
    {
        saveDataBuildingPath = Path.Combine(Application.dataPath, "data_ListBuildinds.json");
        EnsureDependencies();
    }

    private void EnsureDependencies()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance != null ? GameManager.Instance : FindFirstObjectByType<GameManager>();
        }

        if (buildManager == null && gameManager != null)
        {
            buildManager = gameManager.buildManager;
        }

        if (buildManager == null)
        {
            buildManager = FindFirstObjectByType<BuildManager>();
        }
    }

    public void SaveBuildInScenes()
    {
        EnsureDependencies();
        ResetDataBuildingInMemory();
        AddDataListBuilding();
        string json = JsonUtility.ToJson(dataColletBuilding, true);
        File.WriteAllText(saveDataBuildingPath, json);
    }

    public void AddDataListBuilding()
    {
        EnsureDependencies();
        if (buildManager == null || buildManager.builtBuildings == null) return;

        for (int i = 0; i < buildManager.builtBuildings.Count; i++)
        {
            BuiltBuildingInfo build = buildManager.builtBuildings[i];
            if (build == null || build.building == null) continue;

            Building building = build.building;
            GameObject buildGameObject = building.gameObject;
            UpgradeBuilding upgradeLevel = building.GetComponent<UpgradeBuilding>();

            int level = upgradeLevel != null ? upgradeLevel.currentLevel : 1;
            bool isUpgrading = upgradeLevel != null && upgradeLevel.isUpgradBuilding;
            bool isFinishedUpgrade = upgradeLevel != null && upgradeLevel.isFinishedUpgrad;
            int finishUpgradeTime = upgradeLevel != null ? upgradeLevel.finishDayBuildingUpgradTime : 0;

            if (building.nameBuild == "Garden")
            {
                GardenBuilding gardenComp = buildGameObject.GetComponent<GardenBuilding>();
                int yield = gardenComp != null ? gardenComp.yieldduration : 0;

                InfoBuildSmallGarden infoBuildSmallGarden = new InfoBuildSmallGarden
                {
                    nameBuild = building.nameBuild,
                    dayBuildingFinist = building.finishDayBuildingTime,
                    transformX = building.transform.position.x,
                    transformY = building.transform.position.y,
                    levelBuild = level,
                    isBuildingUpgrad = isUpgrading,
                    isBuildFinishedUpgrad = isFinishedUpgrade,
                    dayBuildingUpgradFinist = finishUpgradeTime,
                    yielduration = yield
                };

                dataColletBuilding.listinfoBuildSmallGardens.Add(infoBuildSmallGarden);
            }
            else if (building.nameBuild == "Medium garden")
            {
                MediumGarden mediumGardenScript = buildGameObject.GetComponent<MediumGarden>();
                bool isHerbal = mediumGardenScript != null && mediumGardenScript.isHerbalPlanted;
                int yield = mediumGardenScript != null ? mediumGardenScript.yieldduration : 0;

                InfoBuildMediumGarden infoBuildMediumGarden = new InfoBuildMediumGarden
                {
                    nameBuild = building.nameBuild,
                    dayBuildingFinist = building.finishDayBuildingTime,
                    transformX = building.transform.position.x,
                    transformY = building.transform.position.y,
                    levelBuild = level,
                    isBuildingUpgrad = isUpgrading,
                    isBuildFinishedUpgrad = isFinishedUpgrade,
                    dayBuildingUpgradFinist = finishUpgradeTime,
                    isHerbalPlant = isHerbal,
                    yielduration = yield
                };

                dataColletBuilding.listinfoBuildMediumGardens.Add(infoBuildMediumGarden);
            }
            else
            {
                InfoBuilding infoBuilding = new InfoBuilding
                {
                    nameBuild = building.nameBuild,
                    dayBuildingFinist = building.finishDayBuildingTime,
                    transformX = building.transform.position.x,
                    transformY = building.transform.position.y,
                    levelBuild = level,
                    isBuildingUpgrad = isUpgrading,
                    isBuildFinishedUpgrad = isFinishedUpgrade,
                    dayBuildingUpgradFinist = finishUpgradeTime
                };

                dataColletBuilding.listInfoBuilding.Add(infoBuilding);
            }
        }
    }

    public void LoadBuildInScenes()
    {
        EnsureDependencies();

        if (File.Exists(saveDataBuildingPath))
        {
            string json = File.ReadAllText(saveDataBuildingPath);
            dataColletBuilding = JsonUtility.FromJson<DataColletBuilding>(json);

            if (dataColletBuilding == null)
            {
                ResetDataBuildingInMemory();
            }

            if (dataColletBuilding.listInfoBuilding != null)
            {
                for (int i = 0; i < dataColletBuilding.listInfoBuilding.Count; i++)
                {
                    CreateBuilding(dataColletBuilding.listInfoBuilding[i]);
                }
            }

            if (dataColletBuilding.listinfoBuildSmallGardens != null)
            {
                for (int i = 0; i < dataColletBuilding.listinfoBuildSmallGardens.Count; i++)
                {
                    CreateBuildingSmallGarden(dataColletBuilding.listinfoBuildSmallGardens[i]);
                }
            }

            if (dataColletBuilding.listinfoBuildMediumGardens != null)
            {
                for (int i = 0; i < dataColletBuilding.listinfoBuildMediumGardens.Count; i++)
                {
                    CreateBuildingMediumGarden(dataColletBuilding.listinfoBuildMediumGardens[i]);
                }
            }
        }
        else
        {
            ResetDataBuildingInMemory();
        }
    }

    private Building FindBuildingPrefab(string nameBuild)
    {
        if (buildManager == null || buildManager.listALLBuilding == null) return null;
        for (int i = 0; i < buildManager.listALLBuilding.Count; i++)
        {
            if (buildManager.listALLBuilding[i] != null && buildManager.listALLBuilding[i].nameBuild == nameBuild)
            {
                return buildManager.listALLBuilding[i];
            }
        }
        return null;
    }

    private Tile FindTileAt(Vector2 pos)
    {
        if (buildManager == null || buildManager.tiles == null) return null;
        for (int i = 0; i < buildManager.tiles.Length; i++)
        {
            Tile tile = buildManager.tiles[i];
            if (tile != null)
            {
                Vector3 tilePos = tile.transform.position;
                if (Mathf.Approximately(tilePos.x, pos.x) && Mathf.Approximately(tilePos.y, pos.y))
                {
                    return tile;
                }
            }
        }
        return null;
    }

    public void CreateBuilding(InfoBuilding infoBuilding)
    {
        if (infoBuilding == null || buildManager == null) return;

        Building prefab = FindBuildingPrefab(infoBuilding.nameBuild);
        if (prefab == null)
        {
            Debug.LogWarning($"[SaveAndLoadBuildManager] Prefab not found for: {infoBuilding.nameBuild}");
            return;
        }

        Building newBuildingObject = Instantiate(prefab);
        newBuildingObject.nameBuild = infoBuilding.nameBuild;
        newBuildingObject.finishDayBuildingTime = infoBuilding.dayBuildingFinist;

        Vector2 newVector = new Vector2(infoBuilding.transformX, infoBuilding.transformY);
        newBuildingObject.transform.position = newVector;

        UpgradeBuilding upgradeBuildingScript = newBuildingObject.GetComponent<UpgradeBuilding>();
        if (upgradeBuildingScript != null)
        {
            upgradeBuildingScript.currentLevel = infoBuilding.levelBuild;
            upgradeBuildingScript.isUpgradBuilding = infoBuilding.isBuildingUpgrad;
            upgradeBuildingScript.isFinishedUpgrad = infoBuilding.isBuildFinishedUpgrad;
            upgradeBuildingScript.finishDayBuildingUpgradTime = infoBuilding.dayBuildingUpgradFinist;
        }

        Tile tile = FindTileAt(newVector);
        if (tile != null)
        {
            tile.isOccupied = true;
        }

        BuiltBuildingInfo newBuiltBuildingInfo = new BuiltBuildingInfo(newBuildingObject, infoBuilding.levelBuild, null, newBuildingObject.buildingType);
        buildManager.builtBuildings.Add(newBuiltBuildingInfo);
    }

    public void CreateBuildingSmallGarden(InfoBuildSmallGarden infoBuildSmallGarden)
    {
        if (infoBuildSmallGarden == null || buildManager == null) return;

        Building prefab = FindBuildingPrefab(infoBuildSmallGarden.nameBuild);
        if (prefab == null)
        {
            Debug.LogWarning($"[SaveAndLoadBuildManager] Prefab not found for: {infoBuildSmallGarden.nameBuild}");
            return;
        }

        Building newbuildSmallGarden = Instantiate(prefab);
        newbuildSmallGarden.nameBuild = infoBuildSmallGarden.nameBuild;
        newbuildSmallGarden.finishDayBuildingTime = infoBuildSmallGarden.dayBuildingFinist;

        Vector2 newVector = new Vector2(infoBuildSmallGarden.transformX, infoBuildSmallGarden.transformY);
        newbuildSmallGarden.transform.position = newVector;

        UpgradeBuilding upgradeBuildingScript = newbuildSmallGarden.GetComponent<UpgradeBuilding>();
        if (upgradeBuildingScript != null)
        {
            upgradeBuildingScript.currentLevel = infoBuildSmallGarden.levelBuild;
            upgradeBuildingScript.isUpgradBuilding = infoBuildSmallGarden.isBuildingUpgrad;
            upgradeBuildingScript.isFinishedUpgrad = infoBuildSmallGarden.isBuildFinishedUpgrad;
            upgradeBuildingScript.finishDayBuildingUpgradTime = infoBuildSmallGarden.dayBuildingUpgradFinist;
        }

        GardenBuilding gardenBuildingScript = newbuildSmallGarden.GetComponent<GardenBuilding>();
        if (gardenBuildingScript != null)
        {
            gardenBuildingScript.yieldduration = infoBuildSmallGarden.yielduration;
        }

        Tile tile = FindTileAt(newVector);
        if (tile != null)
        {
            tile.isOccupied = true;
        }

        BuiltBuildingInfo newBuiltBuildingInfo = new BuiltBuildingInfo(newbuildSmallGarden, infoBuildSmallGarden.levelBuild, null, newbuildSmallGarden.buildingType);
        buildManager.builtBuildings.Add(newBuiltBuildingInfo);
    }

    public void CreateBuildingMediumGarden(InfoBuildMediumGarden infoBuildMediumlGarden)
    {
        if (infoBuildMediumlGarden == null || buildManager == null) return;

        Building prefab = FindBuildingPrefab(infoBuildMediumlGarden.nameBuild);
        if (prefab == null)
        {
            Debug.LogWarning($"[SaveAndLoadBuildManager] Prefab not found for: {infoBuildMediumlGarden.nameBuild}");
            return;
        }

        Building newbuildMediumGarden = Instantiate(prefab);
        newbuildMediumGarden.nameBuild = infoBuildMediumlGarden.nameBuild;
        newbuildMediumGarden.finishDayBuildingTime = infoBuildMediumlGarden.dayBuildingFinist;

        Vector2 newVector = new Vector2(infoBuildMediumlGarden.transformX, infoBuildMediumlGarden.transformY);
        newbuildMediumGarden.transform.position = newVector;

        UpgradeBuilding upgradeBuildingScript = newbuildMediumGarden.GetComponent<UpgradeBuilding>();
        if (upgradeBuildingScript != null)
        {
            upgradeBuildingScript.currentLevel = infoBuildMediumlGarden.levelBuild;
            upgradeBuildingScript.isUpgradBuilding = infoBuildMediumlGarden.isBuildingUpgrad;
            upgradeBuildingScript.isFinishedUpgrad = infoBuildMediumlGarden.isBuildFinishedUpgrad;
            upgradeBuildingScript.finishDayBuildingUpgradTime = infoBuildMediumlGarden.dayBuildingUpgradFinist;
        }

        MediumGarden gardenBuildingScript = newbuildMediumGarden.GetComponent<MediumGarden>();
        if (gardenBuildingScript != null)
        {
            gardenBuildingScript.yieldduration = infoBuildMediumlGarden.yielduration;
            gardenBuildingScript.isHerbalPlanted = infoBuildMediumlGarden.isHerbalPlant;
        }

        Tile tile = FindTileAt(newVector);
        if (tile != null)
        {
            tile.isOccupied = true;
        }

        BuiltBuildingInfo newBuiltBuildingInfo = new BuiltBuildingInfo(newbuildMediumGarden, infoBuildMediumlGarden.levelBuild, null, newbuildMediumGarden.buildingType);
        buildManager.builtBuildings.Add(newBuiltBuildingInfo);
    }

    private void ResetDataBuildingInMemory()
    {
        dataColletBuilding = new DataColletBuilding
        {
            listInfoBuilding = new List<InfoBuilding>(),
            listinfoBuildSmallGardens = new List<InfoBuildSmallGarden>(),
            listinfoBuildMediumGardens = new List<InfoBuildMediumGarden>()
        };
    }

    public void ResetDataBuilding()
    {
        ResetDataBuildingInMemory();
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
    public int dayBuildingFinist;
    public int dayBuildingUpgradFinist;
    public bool isBuildingUpgrad;
    public bool isBuildFinishedUpgrad;
}

[Serializable]
public class InfoBuildWorkshop : InfoBuilding { }

[Serializable]
public class InfoBuildWaterPump : InfoBuilding { }

[Serializable]
public class InfoBuildSmallGarden : InfoBuilding
{
    public int yielduration;
}

[Serializable]
public class InfoBuildSolar : InfoBuilding { }

[Serializable]
public class InfoBuildBeacon : InfoBuilding { }

[Serializable]
public class InfoBuildSmallBed : InfoBuilding { }

[Serializable]
public class InfoBuildLounge : InfoBuilding { }

[Serializable]
public class InfoBuildChemicallab : InfoBuilding { }

[Serializable]
public class InfoBuildClinic : InfoBuilding { }

[Serializable]
public class InfoBuildMediumBed : InfoBuilding { }

[Serializable]
public class InfoBuildMoonshine : InfoBuilding { }

[Serializable]
public class InfoBuildMediumGarden : InfoBuilding
{
    public int yielduration;
    public bool isHerbalPlant;
}

[Serializable]
public class InfoBuildCarWorkshop : InfoBuilding { }

[Serializable]
public class InfoBuildFieldHospital : InfoBuilding { }

[Serializable]
public class InfoBuildWatchTower : InfoBuilding { }