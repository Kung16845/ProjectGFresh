using UnityEngine;
using UnityEngine.UI;

public class ButtonChoiceEx : MonoBehaviour
{
    public Button buttonCreateExpendition;
    public ExpenditionManager expenditionManager;
    public Transform transformParent;
    public int indexEXButton;

    private void Start()
    {
        if (expenditionManager == null)
        {
            expenditionManager = ExpenditionManager.Instance != null ? ExpenditionManager.Instance : FindFirstObjectByType<ExpenditionManager>();
        }

        if (expenditionManager != null && transformParent != null)
        {
            expenditionManager.transformsUIEx = transformParent;
        }

        if (buttonCreateExpendition != null)
        {
            if (indexEXButton == 1)
            {
                buttonCreateExpendition.onClick.RemoveAllListeners();
            }
            else if (indexEXButton == 2)
            {
                buttonCreateExpendition.onClick.RemoveAllListeners();
            }
        }
    }
}
