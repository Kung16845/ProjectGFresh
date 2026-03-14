using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isOccupied;
    public BuildingType buildingType;
    public Color greenColor;
    public Color redColor;
    public SpriteRenderer rend;
    public Building buildingOnTile;
    private bool lastOccupied;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        UpdateColor();
    }

    private void OnEnable()
    {
        UpdateColor();
    }

    public void SetOccupied(bool occupied)
    {
        if (isOccupied == occupied) return;
        isOccupied = occupied;
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (rend == null) return;
        rend.color = isOccupied ? redColor : greenColor;
    }
}

