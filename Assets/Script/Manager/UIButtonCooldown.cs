using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonCooldown : MonoBehaviour
{
    public Button targetButton; // ปุ่มที่ต้องการควบคุม
    public float cooldownTime = 3f; // ระยะเวลาที่ปิดใช้งาน (วินาที)

    public void OnButtonClick()
    {
        // เรียกเมื่อปุ่มถูกกด
        Debug.Log("Button clicked!");

        // ปิดการใช้งานปุ่ม
        StartCoroutine(DisableButtonTemporarily());
    }

    private IEnumerator DisableButtonTemporarily()
    {
        targetButton.interactable = false; // ปิดการใช้งานปุ่ม
        yield return new WaitForSeconds(cooldownTime); // รอเวลาตามที่กำหนด
        targetButton.interactable = true; // เปิดใช้งานปุ่มอีกครั้ง
    }
}
