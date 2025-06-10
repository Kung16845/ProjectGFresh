using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BuildingType
{
    Large,
    Medium,
    Small
}
public class Building : MonoBehaviour
{
    public string nameBuild;
    public string detailBuild;
    public int steelCost;
    public int plankCost;
    public int npcCost;
    public int dayCost;
    public int finishDayBuildingTime;
    public BuildingType buildingType;
    public bool isBuilding;
    public TimeManager timeManager;
    public DateTime dateTime;
    public SpriteRenderer spriteRenderer;
    public Sprite OriginalSprite;
    public Sprite Constructhreshold;
    public BuildManager buildManager;
    public bool isfinsih;
    private void Start()
    {
        timeManager = GameManager.Instance.timeManager;
        buildManager = GameManager.Instance.buildManager;
        dateTime = timeManager.dateTime;
        spriteRenderer = GetComponent<SpriteRenderer>();
        isBuilding = true;
        isfinsih = false;
    }
    private void Update()
    {   
        WaitBuilding();
    }
    public void WaitBuilding()
    {
        // Debug.Log("WaitBuilding");

        if (dateTime.day >= finishDayBuildingTime && isBuilding)
        {
            isBuilding = false;
            buildManager.npc += npcCost;
            spriteRenderer.sprite = OriginalSprite;
            isfinsih = true;
            return;
        }
        else if (dateTime.day < finishDayBuildingTime)
        {
            // Debug.Log("Is Building");
            spriteRenderer.sprite = Constructhreshold;
        }
    }
    
    
}
