using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        [TextArea(15, 20)]
        public string description; // Text to display for this tutorial step
        public GameObject highlightObject; // Object to highlight (optional)
        public Transform panelTextPosition;
        public Transform Buttonposition; // Transform to set the position of the paneltext (optional)
    }

    public List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    public TextMeshProUGUI tutorialText; // UI Text to show tutorial descriptions
    public GameObject paneltext;
    public GameObject overlayPanel; // Optional: Panel to dim the background
    public GameObject uIBacktoMainScene;
    public MainSpawner mainSpawner;
    public TimeManager  timeManager;
    private int currentStepIndex = 0;

    // New variable to control if tutorial can proceed
    public GameObject Confirmbutton;
    private CheckUsingDDA checkUsingDDA;
    public bool isturorialnight = true;
    public bool activetutorial;
    private bool canProceed = false;
    public bool tutorialfinished = false;

    // Timeout duration for showing the confirm button
    public float buttonTimeout = 5f; 
    void Awake()
    {
        timeManager = FindObjectOfType<TimeManager>();
        checkUsingDDA = FindObjectOfType<CheckUsingDDA>();
        if(checkUsingDDA.ActiveTutorial)
        {
            timeManager.currentTickSeconedIncrease = 0;
            timeManager.currentTimeBetweenTricks = 0;
            isturorialnight = true;
        }
        else
            isturorialnight = false;
    }
    void Start()
    {   
        mainSpawner = FindObjectOfType<MainSpawner>();
        if(isturorialnight)
        {
            if (tutorialSteps.Count > 0)
            {
                ShowTutorialStep();
            }
            else
            {
                Debug.LogWarning("No tutorial steps defined!");
                EndTutorial();
            }
        }
        else
            this.gameObject.SetActive(false);
    }

    void Update()
    {
        if (canProceed) // Check if allowed to proceed
        {
            NextTutorialStep();
        }
    }

    private void ShowTutorialStep()
    {
        if (currentStepIndex < tutorialSteps.Count)
        {
            TutorialStep step = tutorialSteps[currentStepIndex];

            if (tutorialText != null)
            {
                paneltext.gameObject.SetActive(true);
                tutorialText.text = step.description;

                // Adjust the panel position if specified
                if (step.panelTextPosition != null)
                {
                    paneltext.transform.position = step.panelTextPosition.position;
                }

                if (Confirmbutton != null)
                {
                    Confirmbutton.SetActive(false); // Hide the button initially
                }

                if (step.Buttonposition != null)
                {
                    Confirmbutton.transform.position = step.Buttonposition.position;
                }
            }

            if (overlayPanel != null)
                overlayPanel.SetActive(true);

            // Highlight object if specified
            if (step.highlightObject != null)
                step.highlightObject.SetActive(true);

            // Reset canProceed for the new step
            canProceed = false;

            // Start coroutine to show confirm button after a timeout
            StartCoroutine(ShowConfirmButtonAfterTimeout());
        }
    }

    private IEnumerator ShowConfirmButtonAfterTimeout()
    {
        yield return new WaitForSeconds(buttonTimeout);

        if (Confirmbutton != null)
            Confirmbutton.SetActive(true); // Show the button after the timeout
    }

    private void NextTutorialStep()
    {
        // Stop any ongoing button timeout coroutine
        StopCoroutine(ShowConfirmButtonAfterTimeout());

        // Deactivate current highlight object if any
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
        checkUsingDDA.ActiveTutorial = false;
        timeManager.currentTickSeconedIncrease = 1;
        timeManager.currentTimeBetweenTricks = 0;
        mainSpawner.StartNextDeck();
        mainSpawner.startDelayText.gameObject.SetActive(false);
        if (Confirmbutton != null)
            Confirmbutton.SetActive(false);

        if (tutorialText != null)
        {
            paneltext.gameObject.SetActive(false);
            tutorialText.text = ""; // Clear tutorial text
        }

        if (overlayPanel != null)
            overlayPanel.SetActive(false);

        uIBacktoMainScene.gameObject.SetActive(true);
        Debug.Log("Tutorial finished.");
    }

    public void SetCanproceed()
    {
        canProceed = true;
    }
}
