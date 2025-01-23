using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OutpostupUI : MonoBehaviour
{
    public Outpostup outpostup;
    public Globalstat globalstat;
    public OutpostSystem outpostSystem;
    public GameObject SetupoutpostButton;
    public GameObject DisableButton;
    public TextMeshProUGUI statusOutpost;

    void Start()
    {
        outpostSystem = FindObjectOfType<OutpostSystem>();
        globalstat = FindObjectOfType<Globalstat>();
    }

    void Update()
    {
        // Check if there's an available slot and the outpost is not set up
        if (globalstat.OutpostLimit > outpostSystem.outpostRewards.Count && !outpostup.issetUpOutpost)
        {
            SetupoutpostButton.SetActive(true);
            DisableButton.SetActive(false);
            statusOutpost.text = "An outpost can be set up. Slots are available.";
        }
        // Check if the outpost is set up
        else if (outpostup.issetUpOutpost)
        {
            SetupoutpostButton.SetActive(false);
            DisableButton.SetActive(true);

            if (globalstat.OutpostLimit <= outpostSystem.outpostRewards.Count)
            {
                statusOutpost.text = "You can disable this outpost to free up a slot.";
            }
            else
            {
                statusOutpost.text = "This outpost is currently active.";
            }
        }
        // No available slot for a new outpost
        else
        {
            SetupoutpostButton.SetActive(false);
            DisableButton.SetActive(false);
            statusOutpost.text = "No slots available to set up a new outpost.";
        }
    }
}
