using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBuilding : MonoBehaviour
{
    public Building building;
    public BuildManager buildManager;
    public Image image;
    public TextMeshProUGUI textNameBuild;
    public TextMeshProUGUI textDescriveBuild;
    public TextMeshProUGUI textPlankCost;
    public TextMeshProUGUI textSteelCost;
    public TextMeshProUGUI textNpcCost;
    public TextMeshProUGUI textDayCost;

    private void Awake()
    {
        if (buildManager == null)
        {
            buildManager = BuildManager.Instance;
        }
    }

    public void SetDataBuild()
    {
        if (building == null) return;

        if (textNameBuild != null) textNameBuild.text = building.nameBuild;
        if (textDescriveBuild != null) textDescriveBuild.text = building.detailBuild;
        if (textPlankCost != null) textPlankCost.text = building.plankCost.ToString();
        if (textSteelCost != null) textSteelCost.text = building.steelCost.ToString();
        if (textNpcCost != null) textNpcCost.text = building.npcCost.ToString();
        if (textDayCost != null) textDayCost.text = building.dayCost.ToString();

        if (image != null)
        {
            if (building.OriginalSprite != null)
            {
                image.sprite = building.OriginalSprite;
            }
            else
            {
                SpriteRenderer spriteRend = building.GetComponent<SpriteRenderer>();
                if (spriteRend != null)
                {
                    image.sprite = spriteRend.sprite;
                }
            }
        }

        if (buildManager == null)
        {
            buildManager = BuildManager.Instance;
        }

        if (buildManager != null)
        {
            buildManager.building = building;
        }
    }

    public void DisableColliders()
    {
        if (buildManager != null)
        {
            buildManager.DisableColliders();
        }
    }

    public void EnableColliders()
    {
        if (buildManager != null)
        {
            buildManager.EnableColliders();
        }
    }
}
