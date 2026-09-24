using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    [Header("Resources")]
    [FormerlySerializedAs("steel")]
    public int steel;
    [FormerlySerializedAs("plank")]
    public int plank;
    [FormerlySerializedAs("food")]
    public int food;
    [FormerlySerializedAs("fuel")]
    public int fuel;
    [FormerlySerializedAs("ammo")]
    public int ammo;
    [FormerlySerializedAs("npc")]
    public int npc;

    [Header("Resource Displays")]
    [FormerlySerializedAs("steelDisplay")]
    public TextMeshProUGUI steelDisplay;
    [FormerlySerializedAs("plankDisplay")]
    public TextMeshProUGUI plankDisplay;
    [FormerlySerializedAs("foodDisplay")]
    public TextMeshProUGUI foodDisplay;
    [FormerlySerializedAs("fuelDisplay")]
    public TextMeshProUGUI fuelDisplay;
    [FormerlySerializedAs("ammoDisplay")]
    public TextMeshProUGUI ammoDisplay;
    [FormerlySerializedAs("npcDisplay")]
    public TextMeshProUGUI npcDisplay;

    [Header("Script & Game Objects")]
    [FormerlySerializedAs("iswateractive")]
    public bool iswateractive;
    [FormerlySerializedAs("iselecticitiesactive")]
    public bool iselecticitiesactive;
    [FormerlySerializedAs("building")]
    public Building building;
    [FormerlySerializedAs("buildingToPlace")]
    public Building buildingToPlace;
    [FormerlySerializedAs("customCursor")]
    public CustomCursor customCursor;
    [FormerlySerializedAs("grid")]
    public GameObject grid;
    [FormerlySerializedAs("uIBuilding")]
    public GameObject uIBuilding;
    [FormerlySerializedAs("uIBuildingButton")]
    public GameObject uIBuildingButton;

    [Header("Building Tracking")]
    [FormerlySerializedAs("listALLBuilding")]
    public List<Building> listALLBuilding = new List<Building>();
    [FormerlySerializedAs("builtBuildings")]
    public List<BuiltBuildingInfo> builtBuildings = new List<BuiltBuildingInfo>();
    [FormerlySerializedAs("collidersToManage")]
    public List<Collider2D> collidersToManage = new List<Collider2D>();
    [FormerlySerializedAs("tiles")]
    public Tile[] tiles;

    private Dictionary<int, System.Action<int>> resourceHandlers;
    private Camera _mainCamera;

    // Cache last resource values to avoid string allocations and canvas rebuilds on every frame
    private int _lastSteel = -1;
    private int _lastPlank = -1;
    private int _lastFood = -1;
    private int _lastFuel = -1;
    private int _lastAmmo = -1;
    private int _lastNpc = -1;

    // Track last placement state to avoid iterating and calling SetActive on all tiles every frame
    private Building _lastBuildingToPlace;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        _mainCamera = Camera.main;

        if (tiles == null || tiles.Length == 0)
        {
            tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        }

        CollectColliders();
        InitializeResourceHandlers();
    }

    private void InitializeResourceHandlers()
    {
        resourceHandlers = new Dictionary<int, System.Action<int>>
        {
            { 1020129, amount => { steel += amount; Debug.Log($"Added {amount} Steel. Total: {steel}"); } },
            { 1020128, amount => { plank += amount; Debug.Log($"Added {amount} Plank. Total: {plank}"); } },
            { 1020130, amount => { food += amount; Debug.Log($"Added {amount} Food. Total: {food}"); } },
            { 1020131, amount => { fuel += amount; Debug.Log($"Added {amount} Fuel. Total: {fuel}"); } },
            { 1020132, amount => { ammo += amount; Debug.Log($"Added {amount} Ammo. Total: {ammo}"); } }
        };
    }

    private void Update()
    {
        UpdateResoureDisplay();

        // Only update tile visibility if the buildingToPlace reference changed
        if (buildingToPlace != _lastBuildingToPlace)
        {
            _lastBuildingToPlace = buildingToPlace;
            UpdateTileVisibilityForPlacement();
        }

        if (buildingToPlace != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                BuildPlace();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                CancelBuildingPlacement();
            }
        }
    }

    private void UpdateTileVisibilityForPlacement()
    {
        if (tiles == null) return;

        if (buildingToPlace != null)
        {
            BuildingType targetType = buildingToPlace.buildingType;
            for (int i = 0; i < tiles.Length; i++)
            {
                Tile tile = tiles[i];
                if (tile != null)
                {
                    tile.gameObject.SetActive(tile.buildingType == targetType);
                }
            }
        }
    }

    private void CancelBuildingPlacement()
    {
        if (buildingToPlace != null)
        {
            steel += buildingToPlace.steelCost;
            plank += buildingToPlace.plankCost;
            npc += buildingToPlace.npcCost;
            buildingToPlace = null;
            _lastBuildingToPlace = null;
        }

        if (uIBuilding != null)
        {
            uIBuilding.SetActive(true);
        }

        UIUpdateAfterBuildOrCancelBuild();
    }

    #region Building Placement
    private void BuildPlace()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null) return;
        }

        Tile nearestTile = null;
        float nearestDistance = float.MaxValue;
        Vector2 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        for (int i = 0; i < tiles.Length; i++)
        {
            Tile tile = tiles[i];
            if (tile == null || !tile.gameObject.activeSelf)
            {
                continue;
            }

            float dist = Vector2.Distance(tile.transform.position, mouseWorldPos);
            if (dist < nearestDistance)
            {
                nearestDistance = dist;
                nearestTile = tile;
            }
        }

        if (nearestTile != null && !nearestTile.isOccupied && nearestTile.gameObject.activeSelf && buildingToPlace != null)
        {
            Building newBuilding = Instantiate(buildingToPlace, nearestTile.transform.position, Quaternion.identity);

            if (GameManager.Instance != null && GameManager.Instance.timeManager != null)
            {
                DateTime dateTime = GameManager.Instance.timeManager.dateTime;
                if (dateTime != null)
                {
                    newBuilding.finishDayBuildingTime = dateTime.day + newBuilding.dayCost;
                }
            }

            nearestTile.isOccupied = true;
            nearestTile.buildingOnTile = newBuilding;

            int initialLevel = 1;
            BuiltBuildingInfo builtBuildingInfo = new BuiltBuildingInfo(newBuilding, initialLevel, nearestTile, buildingToPlace.buildingType);
            builtBuildings.Add(builtBuildingInfo);
            CollectColliders();

            buildingToPlace = null;
            _lastBuildingToPlace = null;

            if (uIBuilding != null) uIBuilding.SetActive(false);
            if (uIBuildingButton != null) uIBuildingButton.SetActive(true);

            UIUpdateAfterBuildOrCancelBuild();
        }
    }

    public void BuyBuilding()
    {
        if (building == null) return;

        if (steel >= building.steelCost && plank >= building.plankCost && npc >= building.npcCost)
        {
            steel -= building.steelCost;
            plank -= building.plankCost;
            npc -= building.npcCost;

            Cursor.visible = false;

            if (customCursor != null)
            {
                customCursor.gameObject.SetActive(true);
                SpriteRenderer cursorRenderer = customCursor.GetComponent<SpriteRenderer>();
                if (cursorRenderer != null)
                {
                    cursorRenderer.sprite = building.OriginalSprite;
                }
            }

            buildingToPlace = building;
            _lastBuildingToPlace = building;
            UpdateTileVisibilityForPlacement();

            if (uIBuilding != null) uIBuilding.SetActive(false);
            if (grid != null) grid.SetActive(true);
        }
    }

    public void DestroyBuilding(GameObject buildingObj)
    {
        if (buildingObj == null) return;

        BuiltBuildingInfo buildingInfo = builtBuildings.Find(b => b.building != null && b.building.gameObject == buildingObj);

        if (buildingInfo != null)
        {
            if (buildingInfo.tile != null)
            {
                buildingInfo.tile.isOccupied = false;
                buildingInfo.tile.buildingOnTile = null;
            }

            builtBuildings.Remove(buildingInfo);
            CollectColliders();
            Destroy(buildingObj);
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
        if (resourceHandlers != null && resourceHandlers.TryGetValue(itemId, out var updateResource))
        {
            updateResource(quantity);
        }
    }
    #endregion

    #region UI
    public void UIUpdateAfterBuildOrCancelBuild()
    {
        if (customCursor != null) customCursor.gameObject.SetActive(false);
        Cursor.visible = true;
        if (grid != null) grid.SetActive(false);
    }

    public void UpdateResoureDisplay()
    {
        // Zero-GC: Only allocate strings and trigger TMP mesh rebuilds when values actually change
        if (steelDisplay != null && steel != _lastSteel)
        {
            _lastSteel = steel;
            steelDisplay.text = steel.ToString();
        }

        if (plankDisplay != null && plank != _lastPlank)
        {
            _lastPlank = plank;
            plankDisplay.text = plank.ToString();
        }

        if (foodDisplay != null && food != _lastFood)
        {
            _lastFood = food;
            foodDisplay.text = food.ToString();
        }

        if (fuelDisplay != null && fuel != _lastFuel)
        {
            _lastFuel = fuel;
            fuelDisplay.text = fuel.ToString();
        }

        if (ammoDisplay != null && ammo != _lastAmmo)
        {
            _lastAmmo = ammo;
            ammoDisplay.text = ammo.ToString();
        }

        if (npcDisplay != null && npc != _lastNpc)
        {
            _lastNpc = npc;
            npcDisplay.text = npc.ToString();
        }
    }
    #endregion

    #region Colliders
    public void CollectColliders()
    {
        collidersToManage.Clear();

        if (tiles != null)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                Tile tile = tiles[i];
                if (tile != null)
                {
                    Collider2D tileCollider = tile.GetComponent<Collider2D>();
                    if (tileCollider != null)
                    {
                        collidersToManage.Add(tileCollider);
                    }
                }
            }
        }

        for (int i = 0; i < builtBuildings.Count; i++)
        {
            BuiltBuildingInfo builtBuilding = builtBuildings[i];
            if (builtBuilding != null && builtBuilding.building != null)
            {
                Collider2D buildingCollider = builtBuilding.building.GetComponent<Collider2D>();
                if (buildingCollider != null)
                {
                    collidersToManage.Add(buildingCollider);
                }
            }
        }
    }

    public void DisableColliders()
    {
        for (int i = 0; i < collidersToManage.Count; i++)
        {
            Collider2D collider = collidersToManage[i];
            if (collider != null)
            {
                collider.enabled = false;
            }
        }
    }

    public void EnableColliders()
    {
        for (int i = 0; i < collidersToManage.Count; i++)
        {
            Collider2D collider = collidersToManage[i];
            if (collider != null)
            {
                collider.enabled = true;
            }
        }
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