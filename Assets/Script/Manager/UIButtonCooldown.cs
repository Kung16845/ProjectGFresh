using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonCooldown : MonoBehaviour
{
    public Button targetButton;
    public float cooldownTime = 3f;

    private WaitForSeconds _waitInstruction;

    private void Awake()
    {
        if (targetButton == null)
        {
            targetButton = GetComponent<Button>();
        }
        _waitInstruction = new WaitForSeconds(cooldownTime);
    }

    public void OnButtonClick()
    {
        if (targetButton == null) return;
        StartCoroutine(DisableButtonTemporarily());
    }

    private IEnumerator DisableButtonTemporarily()
    {
        targetButton.interactable = false;
        yield return _waitInstruction;
        if (targetButton != null)
        {
            targetButton.interactable = true;
        }
    }
}
