using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        [TextArea(15, 20)]
        public string description;
        public GameObject highlightObject;
        public Transform panelTextPosition;
        public Transform Buttonposition;
    }

    public List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    public TextMeshProUGUI tutorialText;
    public GameObject paneltext;
    public GameObject overlayPanel;
    public GameObject uIBacktoMainScene;
    public MainSpawner mainSpawner;
    public TimeManager timeManager;
    private int currentStepIndex = 0;

    public GameObject Confirmbutton;
    private CheckUsingDDA checkUsingDDA;
    public bool isturorialnight = true;
    public bool activetutorial;
    private bool canProceed = false;
    public bool tutorialfinished = false;

    public float buttonTimeout = 5f;

    private Coroutine _confirmButtonCoroutine;
    private WaitForSeconds _buttonWait;

    private void Awake()
    {
        _buttonWait = new WaitForSeconds(buttonTimeout);

        if (timeManager == null)
        {
            timeManager = TimeManager.Instance != null ? TimeManager.Instance : FindFirstObjectByType<TimeManager>();
        }

        checkUsingDDA = FindFirstObjectByType<CheckUsingDDA>();
        if (checkUsingDDA != null && checkUsingDDA.ActiveTutorial)
        {
            if (timeManager != null)
            {
                timeManager.currentTickSeconedIncrease = 0;
                timeManager.currentTimeBetweenTricks = 0f;
            }
            isturorialnight = true;
        }
        else
        {
            isturorialnight = false;
        }
    }

    private void Start()
    {
        if (mainSpawner == null)
        {
            mainSpawner = FindFirstObjectByType<MainSpawner>();
        }

        if (isturorialnight)
        {
            if (tutorialSteps.Count > 0)
            {
                ShowTutorialStep();
            }
            else
            {
                Debug.LogWarning("[TutorialManager] No tutorial steps defined!");
                EndTutorial();
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (canProceed)
        {
            canProceed = false;
            NextTutorialStep();
        }
    }

    private void ShowTutorialStep()
    {
        if (currentStepIndex < tutorialSteps.Count)
        {
            TutorialStep step = tutorialSteps[currentStepIndex];

            if (tutorialText != null && paneltext != null)
            {
                paneltext.SetActive(true);
                tutorialText.text = step.description;

                if (step.panelTextPosition != null)
                {
                    paneltext.transform.position = step.panelTextPosition.position;
                }

                if (Confirmbutton != null)
                {
                    Confirmbutton.SetActive(false);
                }

                if (step.Buttonposition != null && Confirmbutton != null)
                {
                    Confirmbutton.transform.position = step.Buttonposition.position;
                }
            }

            if (overlayPanel != null)
            {
                overlayPanel.SetActive(true);
            }

            if (step.highlightObject != null)
            {
                step.highlightObject.SetActive(true);
            }

            canProceed = false;

            if (_confirmButtonCoroutine != null)
            {
                StopCoroutine(_confirmButtonCoroutine);
            }
            _confirmButtonCoroutine = StartCoroutine(ShowConfirmButtonAfterTimeout());
        }
    }

    private IEnumerator ShowConfirmButtonAfterTimeout()
    {
        yield return _buttonWait;

        if (Confirmbutton != null)
        {
            Confirmbutton.SetActive(true);
        }
        _confirmButtonCoroutine = null;
    }

    private void NextTutorialStep()
    {
        if (_confirmButtonCoroutine != null)
        {
            StopCoroutine(_confirmButtonCoroutine);
            _confirmButtonCoroutine = null;
        }

        if (currentStepIndex < tutorialSteps.Count && tutorialSteps[currentStepIndex].highlightObject != null)
        {
            tutorialSteps[currentStepIndex].highlightObject.SetActive(false);
        }

        currentStepIndex++;

        if (currentStepIndex >= tutorialSteps.Count)
        {
            EndTutorial();
        }
        else
        {
            ShowTutorialStep();
        }
    }

    private void EndTutorial()
    {
        tutorialfinished = true;

        if (checkUsingDDA != null)
        {
            checkUsingDDA.ActiveTutorial = false;
        }

        if (timeManager != null)
        {
            timeManager.currentTickSeconedIncrease = 1;
            timeManager.currentTimeBetweenTricks = 0f;
        }

        if (mainSpawner != null)
        {
            mainSpawner.StartNextDeck();
            if (mainSpawner.startDelayText != null)
            {
                mainSpawner.startDelayText.gameObject.SetActive(false);
            }
        }

        if (Confirmbutton != null)
        {
            Confirmbutton.SetActive(false);
        }

        if (tutorialText != null)
        {
            tutorialText.text = "";
        }

        if (paneltext != null)
        {
            paneltext.SetActive(false);
        }

        if (overlayPanel != null)
        {
            overlayPanel.SetActive(false);
        }

        if (uIBacktoMainScene != null)
        {
            uIBacktoMainScene.SetActive(true);
        }

        Debug.Log("[TutorialManager] Tutorial finished.");
    }

    public void SetCanproceed()
    {
        canProceed = true;
    }
}
