using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSystem : MonoBehaviour
{
    private int mainSceneIndex = 0;
    public int currentSceneIndex;
    public Animator transitionAnim;
    public TimeManager timeManager;
    private bool isSceneLoading = false;
    public SaveObjectActiveMainScene saveObjectActiveMainScene;
    public SaveDataDDA saveDataDDA;
    public GameObject uICompleteNightbeforeEndTIme;
    public MainSpawner mainSpawner;
    public TutorialManager tutorialManager;
    private void Start()
    {
        
        timeManager = FindObjectOfType<TimeManager>();
        timeManager.sceneSystem1 = this;
        timeManager.dateTime.sceneSystem = this;
        saveObjectActiveMainScene = FindObjectOfType<SaveObjectActiveMainScene>();
        saveDataDDA = FindObjectOfType<SaveDataDDA>();
        tutorialManager = FindObjectOfType<TutorialManager>();
        mainSpawner = FindObjectOfType<MainSpawner>();
    }
    private void Update()
    {
        if (uICompleteNightbeforeEndTIme== null) return;
        if(!mainSpawner.isCompleteSpawned) return;
        
        Zombie[] zombies = FindObjectsOfType<Zombie>();
        if (zombies.Length == 0)
        {
            Debug.Log("ZombieOutReturn?");
            uICompleteNightbeforeEndTIme.gameObject.SetActive(true);
        }
    }
    public void SwitchScene(int sceneIndex)
    {

        StartCoroutine(LoadScene(sceneIndex));
    }
    IEnumerator LoadScene(int sceneIndex)
    {
        transitionAnim.SetTrigger("EndScene");
        yield return new WaitForSeconds(3.0f);
        if (sceneIndex == mainSceneIndex)
        {
            Debug.LogWarning("Main scene is already loaded. Use ReturnToMainScene instead.");
            yield break;
        }

        // Hide all root GameObjects in the main scene (Scene index 0)
        saveObjectActiveMainScene.objectActiveStates.Clear();
        Scene mainScene = SceneManager.GetSceneByBuildIndex(mainSceneIndex);
        if (mainScene.IsValid() && mainScene.isLoaded)
        {
            foreach (GameObject go in mainScene.GetRootGameObjects())
            {
                saveObjectActiveMainScene.objectActiveStates[go] = go.activeSelf;
                go.SetActive(false); // Temporarily hide main scene objects
            }
        }

        // Load the new scene additively
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);

        currentSceneIndex = sceneIndex;
    }

    public void ReturnToMainScene()
    {
        // Unload all additive scenes
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.buildIndex != mainSceneIndex && scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }

        // Show all root GameObjects in the main scene
        Scene mainScene = SceneManager.GetSceneByBuildIndex(mainSceneIndex);
        if (mainScene.IsValid() && mainScene.isLoaded)
        {
            foreach (GameObject go in mainScene.GetRootGameObjects())
            {
                // go.SetActive(true); // Restore main scene objects
                if (saveObjectActiveMainScene.objectActiveStates.TryGetValue(go, out bool wasActive))
                {
                    go.SetActive(wasActive); // Restore the previous active state
                }
            }
        }
        if(tutorialManager == null)
        {
            saveDataDDA.AddDataDDAAndSave();
            Debug.Log("SaveData");
        }
        // timeManager.dateTime.day++;
        timeManager.dateTime.SetTimeStartDay();
    }
    public void ReturnToMainSceneFromExpenditionScene()
    {
        // Unload all additive scenes
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.buildIndex != mainSceneIndex && scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }

        // Show all root GameObjects in the main scene
        Scene mainScene = SceneManager.GetSceneByBuildIndex(mainSceneIndex);
        if (mainScene.IsValid() && mainScene.isLoaded)
        {
            foreach (GameObject go in mainScene.GetRootGameObjects())
            {
                // go.SetActive(true); // Restore main scene objects
                if (saveObjectActiveMainScene.objectActiveStates.TryGetValue(go, out bool wasActive))
                {
                    go.SetActive(wasActive); // Restore the previous active state
                }
            }
        }
        if(tutorialManager == null)
        {
            saveDataDDA.AddDataDDAAndSave();
            Debug.Log("SaveData");
        }
        timeManager.TimeStop();
    }
    private void OnEnable()
    {
        timeManager = FindObjectOfType<TimeManager>();
        timeManager.sceneSystem1 = this;
        timeManager.dateTime.sceneSystem = this;
    }
}
