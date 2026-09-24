using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Tile : MonoBehaviour
{
    [FormerlySerializedAs("isOccupied")]
    [SerializeField] private bool _isOccupied;

    [FormerlySerializedAs("buildingType")]
    [SerializeField] private BuildingType _buildingType;

    [FormerlySerializedAs("greenColor")]
    [SerializeField] private Color _greenColor = Color.green;

    [FormerlySerializedAs("redColor")]
    [SerializeField] private Color _redColor = Color.red;

    [FormerlySerializedAs("rend")]
    [SerializeField] private SpriteRenderer _rend;

    [FormerlySerializedAs("buildingOnTile")]
    [SerializeField] private Building _buildingOnTile;

    private bool _lastOccupiedState;
    private bool _hasInitialized;

    public bool isOccupied
    {
        get => _isOccupied;
        set
        {
            _isOccupied = value;
            _lastOccupiedState = value;
            UpdateColor();
        }
    }

    public BuildingType buildingType
    {
        get => _buildingType;
        set => _buildingType = value;
    }

    public Color greenColor
    {
        get => _greenColor;
        set
        {
            _greenColor = value;
            UpdateColor();
        }
    }

    public Color redColor
    {
        get => _redColor;
        set
        {
            _redColor = value;
            UpdateColor();
        }
    }

    public SpriteRenderer rend
    {
        get => _rend;
        set => _rend = value;
    }

    public Building buildingOnTile
    {
        get => _buildingOnTile;
        set => _buildingOnTile = value;
    }

    private void Awake()
    {
        if (_rend == null)
        {
            _rend = GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        UpdateColor();
        _lastOccupiedState = _isOccupied;
        _hasInitialized = true;
    }

    protected void Update()
    {
        // Only update color if the state changed externally (e.g. from inspector or external code)
        if (_hasInitialized && _isOccupied != _lastOccupiedState)
        {
            _lastOccupiedState = _isOccupied;
            UpdateColor();
        }
    }

    public void UpdateColor()
    {
        if (_rend != null)
        {
            _rend.color = _isOccupied ? _redColor : _greenColor;
        }
    }
}
