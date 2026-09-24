using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSystem : MonoBehaviour
{   
    public static SceneSystem Instance { get; private set; }

    [Header("Scene Configuration")]
    [SerializeField] private int mainSceneIndex = 0;
    public int currentSceneIndex;
    public Animator transitionAnim;
    public TimeManager timeManager;
    public SaveObjectActiveMainScene saveObjectActiveMainScene;
    public SaveDataDDA saveDataDDA;
    public GameObject uICompleteNightbeforeEndTIme;
    public MainSpawner mainSpawner;
    public TutorialManager tutorialManager;

    private bool _isSceneLoading = false;
    private float _zombieCheckTimer = 0f;
    private const float ZombieCheckInterval = 0.5f;
    private static readonly WaitForSeconds WaitSceneTransition = new WaitForSeconds(3.0f);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (timeManager == null) timeManager = FindFirstObjectByType<TimeManager>();
        if (saveObjectActiveMainScene == null) saveObjectActiveMainScene = FindFirstObjectByType<SaveObjectActiveMainScene>();
        if (saveDataDDA == null) saveDataDDA = FindFirstObjectByType<SaveDataDDA>();
        if (tutorialManager == null) tutorialManager = FindFirstObjectByType<TutorialManager>();
        if (mainSpawner == null) mainSpawner = FindFirstObjectByType<MainSpawner>();
    }

    private void Update()
    {
        if (uICompleteNightbeforeEndTIme == null) return;
        if (mainSpawner == null || !mainSpawner.isCompleteSpawned) return;

        _zombieCheckTimer += Time.deltaTime;
        if (_zombieCheckTimer >= ZombieCheckInterval)
        {
            _zombieCheckTimer = 0f;

            // Zero-GC, early-exit check instead of full-hierarchy array allocation
            Zombie anyZombie = FindFirstObjectByType<Zombie>();
            if (anyZombie == null)
            {
                uICompleteNightbeforeEndTIme.SetActive(true);
            }
        }
    }

    public void SwitchScene(int sceneIndex)
    {
        if (_isSceneLoading) return;
        StartCoroutine(LoadScene(sceneIndex));
    }

    private IEnumerator LoadScene(int sceneIndex)
    {
        _isSceneLoading = true;

        if (transitionAnim != null)
        {
            transitionAnim.SetTrigger("EndScene");
        }

        yield return WaitSceneTransition;

        if (sceneIndex == mainSceneIndex)
        {
            Debug.LogWarning("Main scene is already loaded. Use ReturnToMainScene instead.");
            _isSceneLoading = false;
            yield break;
        }

        // Hide all root GameObjects in the main scene (Scene index 0)
        if (saveObjectActiveMainScene != null)
        {
            saveObjectActiveMainScene.objectActiveStates.Clear();
            Scene mainScene = SceneManager.GetSceneByBuildIndex(mainSceneIndex);
            if (mainScene.IsValid() && mainScene.isLoaded)
            {
                GameObject[] rootObjects = mainScene.GetRootGameObjects();
                for (int i = 0; i < rootObjects.Length; i++)
                {
                    GameObject go = rootObjects[i];
                    if (go != null)
                    {
                        saveObjectActiveMainScene.objectActiveStates[go] = go.activeSelf;
                        go.SetActive(false);
                    }
                }
            }
        }

        // Load the new scene additively
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
        currentSceneIndex = sceneIndex;
        _isSceneLoading = false;
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
        if (mainScene.IsValid() && mainScene.isLoaded && saveObjectActiveMainScene != null)
        {
            GameObject[] rootObjects = mainScene.GetRootGameObjects();
            for (int i = 0; i < rootObjects.Length; i++)
            {
                GameObject go = rootObjects[i];
                if (go != null && saveObjectActiveMainScene.objectActiveStates.TryGetValue(go, out bool wasActive))
                {
                    go.SetActive(wasActive);
                }
            }
        }

        if (tutorialManager == null && saveDataDDA != null)
        {
            saveDataDDA.AddDataDDAAndSave();
        }

        if (timeManager != null && timeManager.dateTime != null)
        {
            timeManager.dateTime.SetTimeStartDay();
        }
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
        if (mainScene.IsValid() && mainScene.isLoaded && saveObjectActiveMainScene != null)
        {
            GameObject[] rootObjects = mainScene.GetRootGameObjects();
            for (int i = 0; i < rootObjects.Length; i++)
            {
                GameObject go = rootObjects[i];
                if (go != null && saveObjectActiveMainScene.objectActiveStates.TryGetValue(go, out bool wasActive))
                {
                    go.SetActive(wasActive);
                }
            }
        }

        if (tutorialManager == null && saveDataDDA != null)
        {
            saveDataDDA.AddDataDDAAndSave();
        }

        if (timeManager != null)
        {
            timeManager.TimeStop();
        }
    }
}
