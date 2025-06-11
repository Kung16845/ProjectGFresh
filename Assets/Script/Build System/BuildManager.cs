using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }
    [Header("Resources")]
    public int steel;
    public int plank;
    public int food;
    public int fuel;
    public int ammo;
    public int npc;
    public TextMeshProUGUI steelDisplay;
    public TextMeshProUGUI plankDisplay;
    public TextMeshProUGUI foodDisplay;
    public TextMeshProUGUI fuelDisplay;
    public TextMeshProUGUI ammoDisplay;
    public TextMeshProUGUI npcDisplay;
    [Header("Scipt")]
    public bool iswateractive;
    public bool iselecticitiesactive;
    public Building building;
    public Building buildingToPlace;
    public CustomCursor customCursor;
    public GameObject grid;
    public GameObject uIBuilding;
    public GameObject uIBuildingButton;
    [Header("Building Tracking")]
    public List<Building> listALLBuilding = new List<Building>();
    public List<BuiltBuildingInfo> builtBuildings = new List<BuiltBuildingInfo>();
    public List<Collider2D> collidersToManage = new List<Collider2D>();
    // private Dictionary<int, System.Action<int>> resourceHandlers;
    public Tile[] tiles;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        CollectColliders();
        // InitializeResourceHandlers();
    }
    // private void InitializeResourceHandlers()
    // {
    //     // Map item IDs to their respective resource update logic
    //     resourceHandlers = new Dictionary<int, System.Action<int>>
    //     {
    //         { 1020129, amount => { steel += amount; Debug.Log($"Added {amount} Steel. Total: {steel}"); } },
    //         { 1020128, amount => { plank += amount; Debug.Log($"Added {amount} Plank. Total: {plank}"); } },
    //         { 1020130, amount => { food += amount; Debug.Log($"Added {amount} Food. Total: {food}"); } },
    //         { 1020131, amount => { fuel += amount; Debug.Log($"Added {amount} Fuel. Total: {fuel}"); } },
    //         { 1020132, amount => { ammo += amount; Debug.Log($"Added {amount} Ammo. Total: {ammo}"); } }
    //     };
    // }


    // Update is called once per frame
    void Update()
    {
        UpdateResoureDisplay();
        if (buildingToPlace != null)
        {
            foreach (Tile tile in tiles)
            {
                if (tile.buildingType == buildingToPlace.buildingType)
                {
                    tile.gameObject.SetActive(true);
                }
                else
                {
                    tile.gameObject.SetActive(false);
                }

            }
        }
        if (Input.GetMouseButtonDown(0) && buildingToPlace != null)
        {
            BuildPlace();
        }
        else if (Input.GetMouseButtonDown(1) && buildingToPlace != null)
        {

            if (buildingToPlace != null)
            {
                steel += buildingToPlace.steelCost;
                plank += buildingToPlace.plankCost;
                npc += buildingToPlace.npcCost;
                buildingToPlace = null;
            }
            uIBuilding.SetActive(true);
            UIUpdateAfterBuildOrCancelBuild();

        }
    }
    #region  Building 
    private void BuildPlace()
    {
        Tile nearestTile = null;
        float nearestDistance = float.MaxValue;
        foreach (Tile tile in tiles)
        {
            if (!tile.gameObject.activeSelf)
            {
                continue; // Skip the rest of the loop if the tile is not active
            }
            float dist = Vector2.Distance(tile.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (dist < nearestDistance)
            {
                nearestDistance = dist;
                nearestTile = tile;
            }
            // Debug.Log("Check area buy");

        }

        if (nearestTile.isOccupied == false && nearestTile.gameObject.activeSelf == true && buildingToPlace != null)
        {
            // Instantiate the building and get a reference to it
            Building newBuilding = Instantiate(buildingToPlace, nearestTile.transform.position, Quaternion.identity);

            DateTime dateTime = GameManager.Instance.timeManager.dateTime;
            newBuilding.finishDayBuildingTime = dateTime.day + newBuilding.dayCost;
            // Update tile status
            nearestTile.isOccupied = true;
            nearestTile.buildingOnTile = newBuilding.GetComponent<Building>();

            // Add the building to the builtBuildings list
            int initialLevel = 1; // Buildings start at level 1
            BuiltBuildingInfo builtBuildingInfo = new BuiltBuildingInfo(newBuilding, initialLevel, nearestTile, buildingToPlace.buildingType);
            builtBuildings.Add(builtBuildingInfo);
            CollectColliders();

            // UI updates
            buildingToPlace = null;
            uIBuilding.SetActive(false);
            uIBuildingButton.SetActive(true);
            UIUpdateAfterBuildOrCancelBuild();
        }
    }
    public void BuyBuilding()
    {
        if (steel >= building.steelCost && plank >= building.plankCost && npc >= building.npcCost)
        {
            steel -= building.steelCost;
            plank -= building.plankCost;
            npc -= building.npcCost;
            Cursor.visible = false;
            customCursor.gameObject.SetActive(true);
            customCursor.GetComponent<SpriteRenderer>().sprite = building.OriginalSprite;
            buildingToPlace = building;
            uIBuilding.SetActive(false);
            grid.SetActive(true);
        }
    }
    public void DestroyBuilding(GameObject building)
    {
        // Find the built building info
        BuiltBuildingInfo buildingInfo = builtBuildings.Find(b => b.building == building);

        if (buildingInfo != null)
        {
            // Update tile status
            buildingInfo.tile.isOccupied = false;
            buildingInfo.tile.buildingOnTile = null;

            // Remove from list
            builtBuildings.Remove(buildingInfo);
            CollectColliders();

            // Destroy the building GameObject
            Destroy(building);
        }
    }
    #endregion
    #region Supply
    public void AddSupply(SupplyType supplyType, int amount)
    {
        switch (supplyType)
        {
            case SupplyType.Steel:
                steel += amount;
                Debug.Log($"Added {amount} Steel. Total: {steel}");
                break;
            case SupplyType.Plank:
                plank += amount;
                Debug.Log($"Added {amount} Plank. Total: {plank}");
                break;
            case SupplyType.Food:
                food += amount;
                Debug.Log($"Added {amount} Food. Total: {food}");
                break;
            case SupplyType.Fuel:
                fuel += amount;
                Debug.Log($"Added {amount} Fuel. Total: {fuel}");
                break;
            case SupplyType.Ammo:
                ammo += amount;
                Debug.Log($"Added {amount} Ammo. Total: {ammo}");
                break;
            case SupplyType.NPC:
                npc += amount;
                Debug.Log($"Added {amount} NPCs. Total: {npc}");
                break;
            default:
                Debug.LogWarning($"Supply type '{supplyType}' is not recognized.");
                break;
        }
    }
    public void AddResource(int itemId, int quantity)
    {
        if (resourceHandlers.TryGetValue(itemId, out var updateResource))
        {
            updateResource(quantity);
        }
    }
    #endregion
    #region UI
    public void UIUpdateAfterBuildOrCancelBuild()
    {
        customCursor.gameObject.SetActive(false);
        Cursor.visible = true;
        grid.SetActive(false);
    }
    public void UpdateResoureDisplay()
    {
        steelDisplay.text = steel.ToString();
        plankDisplay.text = plank.ToString();
        foodDisplay.text = food.ToString();
        fuelDisplay.text = fuel.ToString();
        ammoDisplay.text = ammo.ToString();
        npcDisplay.text = npc.ToString();
    }
    #endregion
    #region Colliders
    public void CollectColliders()
    {
        collidersToManage.Clear();

        // Collect colliders from tiles
        foreach (Tile tile in tiles)
        {
            Collider2D tileCollider = tile.GetComponent<Collider2D>();
            if (tileCollider != null)
            {
                collidersToManage.Add(tileCollider);
            }
        }

        // Collect colliders from built buildings
        foreach (BuiltBuildingInfo builtBuilding in builtBuildings)
        {
            Collider2D buildingCollider = builtBuilding.building.GetComponent<Collider2D>();
            if (buildingCollider != null)
            {
                collidersToManage.Add(buildingCollider);
            }
        }
    }

    public void DisableColliders()
    {
        foreach (Collider2D collider in collidersToManage)
        {
            if (collider != null)
            {
                collider.enabled = false;
            }
        }
        Debug.Log("All colliders have been disabled.");
    }
    public void EnableColliders()
    {
        foreach (Collider2D collider in collidersToManage)
        {
            if (collider != null)
            {
                collider.enabled = true;
            }
        }
        Debug.Log("All colliders have been enabled.");
    }
    #endregion



}
[System.Serializable]
public class BuiltBuildingInfo
{
    public Building building;
    public int level;
    public Tile tile;
    public BuildingType buildingType;
    public BuiltBuildingInfo(Building building, int level, Tile tile, BuildingType buildingType)
    {
        this.building = building;
        this.level = level;
        this.tile = tile;
        this.buildingType = buildingType;
    }
}
public enum SupplyType
{   
    None,
    Steel,
    Plank,
    Food,
    Fuel,
    Ammo,
    NPC
}