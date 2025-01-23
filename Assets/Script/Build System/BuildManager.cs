using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BuildManager : MonoBehaviour
{   
    public static BuildManager Instance { get; private set; }
    [Header("Resources")]
    public int steel;
    public TextMeshProUGUI steelDisplay;
    public int plank;
    public TextMeshProUGUI plankDisplay;
    public int food;
    public TextMeshProUGUI foodDisplay;
    public int fuel;
    public TextMeshProUGUI fuelDisplay;
    public int ammo;
    public TextMeshProUGUI ammoDisplay;
    public int npc;
    public TextMeshProUGUI npcDisplay;
    [Header("Scipt")]
    public bool iswateractive;
    public bool iselecticitiesactive;
    public Building building;
    public Building buildingToPlace;
    public CustomCursor customCursor;
    public GameObject grid;
    public GameObject uIBuilding;
    public GameObject buttonBuild;
    [Header("Building Tracking")]
    public List<GameObject> listALLBuilding = new List<GameObject>();
    public List<BuiltBuildingInfo> builtBuildings = new List<BuiltBuildingInfo>();
    public List<Collider2D> collidersToManage = new List<Collider2D>();
    private Dictionary<int, System.Action<int>> resourceHandlers;
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
        InitializeResourceHandlers();
    }
    private void InitializeResourceHandlers()
    {
        // Map item IDs to their respective resource update logic
        resourceHandlers = new Dictionary<int, System.Action<int>>
        {
            { 1020129, amount => { steel += amount; Debug.Log($"Added {amount} Steel. Total: {steel}"); } },
            { 1020128, amount => { plank += amount; Debug.Log($"Added {amount} Plank. Total: {plank}"); } },
            { 1020130, amount => { food += amount; Debug.Log($"Added {amount} Food. Total: {food}"); } },
            { 1020131, amount => { fuel += amount; Debug.Log($"Added {amount} Fuel. Total: {fuel}"); } },
            { 1020132, amount => { ammo += amount; Debug.Log($"Added {amount} Ammo. Total: {ammo}"); } }
        };
    }
    public void AddResource(int itemId, int quantity)
    {
        if (resourceHandlers.TryGetValue(itemId, out var updateResource))
        {
            updateResource(quantity);
            UpdateResoureDisplay();
        }
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
    // Update is called once per frame
    void Update()
    {
        UpdateResoureDisplay();
        if (buildingToPlace != null)
        {
            // Debug.Log("Check area plce");
            foreach (Tile tile in tiles)
            {
                if (tile.isTileLarge == true && buildingToPlace.isBuildingLarge == true)
                {
                    tile.gameObject.SetActive(true);
                }
                else if (tile.isTileMedium == true && buildingToPlace.isBuildingMedium == true)
                {
                    tile.gameObject.SetActive(true);
                }
                else if (tile.isTileSmall == true && buildingToPlace.isBuildingSmall == true)
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

            Tile nearstTile = null;
            float nearstDistance = float.MaxValue;
            foreach (Tile tile in tiles)
            {
                if (!tile.gameObject.activeSelf)
                {
                    continue; // Skip the rest of the loop if the tile is not active
                }
                float dist = Vector2.Distance(tile.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (dist < nearstDistance)
                {
                    nearstDistance = dist;
                    nearstTile = tile;
                }
                // Debug.Log("Check area buy");

            }
            
            if (nearstTile.isOccupied == false && nearstTile.gameObject.activeSelf == true && buildingToPlace != null)
            {
                // Instantiate the building and get a reference to it
                GameObject newBuilding = Instantiate(buildingToPlace.gameObject, nearstTile.transform.position, Quaternion.identity);

                DateTime dateTime = FindObjectOfType<TimeManager>().dateTime;
                Building buildingScript = newBuilding.GetComponent<Building>();
                buildingScript .finishDayBuildingTime = dateTime.day + buildingScript.dayCost; 
                // Update tile status
                nearstTile.isOccupied = true;
                nearstTile.buildingOnTile = newBuilding.GetComponent<Building>();

                // Add the building to the builtBuildings list
                int initialLevel = 1; // Buildings start at level 1
                BuiltBuildingInfo builtBuildingInfo = new BuiltBuildingInfo(newBuilding, initialLevel, nearstTile);
                builtBuildings.Add(builtBuildingInfo);
                CollectColliders();

                // UI updates
                buildingToPlace = null;
                uIBuilding.SetActive(false);
                buttonBuild.SetActive(true);
                UIUpdateAfterBuildOrCancelBuild();
            }
        }
        else if (Input.GetMouseButtonDown(1) && buildingToPlace != null)
        {

            if (buildingToPlace != null)
            {
                steel += buildingToPlace.steelCost;
                plank += buildingToPlace.plankCost;
                food += buildingToPlace.foodCost;
                fuel += buildingToPlace.fuelCost;
                ammo += buildingToPlace.ammoCost;
                npc += buildingToPlace.npcCost;
                buildingToPlace = null;
            }
            uIBuilding.SetActive(true);
            UIUpdateAfterBuildOrCancelBuild();
             
        }
    }
    
    public void UIUpdateAfterBuildOrCancelBuild()
    {
        customCursor.gameObject.SetActive(false);
        Cursor.visible = true;
        grid.SetActive(false);
        
       
    }
    public void BuyBuilding()
    {   
        
        if (steel >= building.steelCost && plank >= building.plankCost && food >= building.foodCost
        && fuel >= building.fuelCost && ammo >= building.ammoCost && npc >= building.npcCost)
        {   
         
            steel -= building.steelCost;
            plank -= building.plankCost;
            food -= building.foodCost;
            fuel -= building.fuelCost;
            ammo -= building.ammoCost;
            npc -= building.npcCost;
            // มีภาพลากตามเมาส์ 
            Cursor.visible = false;
            customCursor.gameObject.SetActive(true);
            customCursor.GetComponent<SpriteRenderer>().sprite = building.GetComponent<SpriteRenderer>().sprite;
            //
            buildingToPlace = building;
            uIBuilding.SetActive(false);
            grid.SetActive(true);

        }
    }
    public void DestroyBuilding(GameObject building)
    {
        // Find the built building info
        BuiltBuildingInfo buildingInfo = builtBuildings.Find(b => b.buildingGameObject == building);

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
            Collider2D buildingCollider = builtBuilding.buildingGameObject.GetComponent<Collider2D>();
            if (buildingCollider != null)
            {
                collidersToManage.Add(buildingCollider);
            }
        }
    }
    public void AddSupply(string supplyType, int amount)
    {
        switch (supplyType.ToLower())
        {
            case "steel":
                steel += amount;
                Debug.Log($"Added {amount} Steel. Total: {steel}");
                break;
            case "plank":
                plank += amount;
                Debug.Log($"Added {amount} Plank. Total: {plank}");
                break;
            case "food":
                food += amount;
                Debug.Log($"Added {amount} Food. Total: {food}");
                break;
            case "fuel":
                fuel += amount;
                Debug.Log($"Added {amount} Fuel. Total: {fuel}");
                break;
            case "ammo":
                ammo += amount;
                Debug.Log($"Added {amount} Ammo. Total: {ammo}");
                break;
            case "npc":
                npc += amount;
                Debug.Log($"Added {amount} NPCs. Total: {npc}");
                break;
            default:
                Debug.LogWarning($"Supply type '{supplyType}' is not recognized.");
                break;
        }

        // Update the resource display after adding supply
        UpdateResoureDisplay();
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
}
[System.Serializable]
public class BuiltBuildingInfo
{
    public GameObject buildingGameObject;
    public int level;
    public Tile tile;

    public BuiltBuildingInfo(GameObject buildingGameObject, int level, Tile tile)
    {
        this.buildingGameObject = buildingGameObject;
        this.level = level;
        this.tile = tile;
    }
}