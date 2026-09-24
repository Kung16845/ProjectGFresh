using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatPanelUI : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI itemNameText;
    public Transform statContainer;
    public ItemStatDisplay itemStatDisplay;

    public void DeleteItem()
    {
        if (itemStatDisplay != null)
        {
            itemStatDisplay.DeletethisItem();
        }
        else
        {
            Debug.LogWarning("ItemStatDisplay reference is missing.");
        }
    }

    public void Destroythisobject()
    {
        Destroy(gameObject);
    }
}
