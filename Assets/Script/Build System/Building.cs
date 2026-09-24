using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum BuildingType
{
    Large,
    Medium,
    Small
}

public class Building : MonoBehaviour
{
    [Header("Building Data")]
    [FormerlySerializedAs("nameBuild")]
    [SerializeField] private string _nameBuild;

    [FormerlySerializedAs("detailBuild")]
    [SerializeField] private string _detailBuild;

    [FormerlySerializedAs("steelCost")]
    [SerializeField] private int _steelCost;

    [FormerlySerializedAs("plankCost")]
    [SerializeField] private int _plankCost;

    [FormerlySerializedAs("npcCost")]
    [SerializeField] private int _npcCost;

    [FormerlySerializedAs("dayCost")]
    [SerializeField] private int _dayCost;

    [FormerlySerializedAs("finishDayBuildingTime")]
    [SerializeField] private int _finishDayBuildingTime;

    [FormerlySerializedAs("buildingType")]
    [SerializeField] private BuildingType _buildingType;

    [Header("Status")]
    [FormerlySerializedAs("isBuilding")]
    [SerializeField] private bool _isBuilding = true;

    [FormerlySerializedAs("isfinsih")]
    [SerializeField] private bool _isfinsih = false;

    [Header("Visuals")]
    [FormerlySerializedAs("spriteRenderer")]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [FormerlySerializedAs("OriginalSprite")]
    [SerializeField] private Sprite _originalSprite;

    [FormerlySerializedAs("Constructhreshold")]
    [SerializeField] private Sprite _constructhreshold;

    [Header("External References")]
    [FormerlySerializedAs("timeManager")]
    [SerializeField] private TimeManager _timeManager;

    [FormerlySerializedAs("buildManager")]
    [SerializeField] private BuildManager _buildManager;

    public DateTime dateTime;

    // Public Properties maintaining backward compatibility
    public string nameBuild { get => _nameBuild; set => _nameBuild = value; }
    public string detailBuild { get => _detailBuild; set => _detailBuild = value; }
    public int steelCost { get => _steelCost; set => _steelCost = value; }
    public int plankCost { get => _plankCost; set => _plankCost = value; }
    public int npcCost { get => _npcCost; set => _npcCost = value; }
    public int dayCost { get => _dayCost; set => _dayCost = value; }
    public int finishDayBuildingTime { get => _finishDayBuildingTime; set => _finishDayBuildingTime = value; }
    public BuildingType buildingType { get => _buildingType; set => _buildingType = value; }
    public bool isBuilding { get => _isBuilding; set => _isBuilding = value; }
    public bool isfinsih { get => _isfinsih; set => _isfinsih = value; }
    public bool isFinished => _isfinsih;
    public SpriteRenderer spriteRenderer { get => _spriteRenderer; set => _spriteRenderer = value; }
    public Sprite OriginalSprite { get => _originalSprite; set => _originalSprite = value; }
    public Sprite Constructhreshold { get => _constructhreshold; set => _constructhreshold = value; }
    public TimeManager timeManager { get => _timeManager; set => _timeManager = value; }
    public BuildManager buildManager { get => _buildManager; set => _buildManager = value; }

    private void Awake()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            if (_timeManager == null)
            {
                _timeManager = GameManager.Instance.timeManager;
            }
            if (_buildManager == null)
            {
                _buildManager = GameManager.Instance.buildManager;
            }
        }

        if (_timeManager != null)
        {
            dateTime = _timeManager.dateTime;
        }

        _isBuilding = true;
        _isfinsih = false;

        // Set initial construction sprite
        if (_spriteRenderer != null && _constructhreshold != null)
        {
            _spriteRenderer.sprite = _constructhreshold;
        }
    }

    private void Update()
    {
        if (_isBuilding)
        {
            WaitBuilding();
        }
    }

    public void WaitBuilding()
    {
        if (dateTime == null)
        {
            if (_timeManager != null)
            {
                dateTime = _timeManager.dateTime;
            }
            if (dateTime == null) return;
        }

        if (dateTime.day >= _finishDayBuildingTime && _isBuilding)
        {
            _isBuilding = false;
            _isfinsih = true;

            if (_buildManager != null)
            {
                _buildManager.npc += _npcCost;
            }

            if (_spriteRenderer != null && _originalSprite != null)
            {
                _spriteRenderer.sprite = _originalSprite;
            }
        }
        else if (dateTime.day < _finishDayBuildingTime && _isBuilding)
        {
            if (_spriteRenderer != null && _spriteRenderer.sprite != _constructhreshold && _constructhreshold != null)
            {
                _spriteRenderer.sprite = _constructhreshold;
            }
        }
    }
}
