using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public float currentHp;
    public float maxHp;
    public SceneSystem sceneSystem;
    public DDAdataCollector dDAdataCollector;
    private void Start() 
    {
        currentHp = maxHp;
        sceneSystem = FindAnyObjectByType<SceneSystem>();
        dDAdataCollector = FindAnyObjectByType<DDAdataCollector>();
    }
    public void BarrierTakeDamage(float damage)
    {
        currentHp -= damage;
        DDAdataCollector.Instance.OnBarrierDamage(damage); // Notify damage

        if (currentHp <= 0)
        {
            // Time.timeScale = 0;
            dDAdataCollector.valueFail += 1.0f;
            sceneSystem.ReturnToMainScene();
            sceneSystem.timeManager.dateTime.SetTimeStartDay();
            sceneSystem.timeManager.dateTime.day++;
        }
    }

    public void BarrierHealDamage(float heal)
    {
        currentHp += heal;
        if (currentHp > maxHp)
            currentHp = maxHp;
    }

}
