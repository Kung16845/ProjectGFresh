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
    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }
    protected void Update()
    {
        if (isOccupied)
        {
            rend.color = redColor;
        }
        else
        {
            rend.color = greenColor;
        }
    }

   
}

