using System.Collections.Generic;
using UnityEngine;

public enum CardEvent
{
    None,
    Fog,
    Supplydrop,
    PrisonBreak,
    Blackout,
    Rain,
    Swarm,
    Infectednest,
    HuntingFestival
}

[System.Serializable]
public class EventDuration
{
    public CardEvent cardEvent;
    public int minDuration;
    public int maxDuration;
    public int GetRandomDuration()
    {
        if (minDuration == maxDuration)
            return minDuration;
        else
            return Random.Range(minDuration, maxDuration + 1); // +1 because max is exclusive
    }
}

public class EventCard : MonoBehaviour
{
    public int DurationLeft;
    public Globalstat globalstat;

    [Header("Event Settings")]
    public CardEvent cardEvent = CardEvent.None;

    [Header("Event Durations")]
    public List<EventDuration> eventDurations = new List<EventDuration>();

    void Start()
    {
        ActiveEvent();
        SetEventDuration();
    }

    // Switch-case for triggering events
    void ActiveEvent()
    {
        switch (cardEvent)
        {
            case CardEvent.None:
                HandleNone();
                break;
            case CardEvent.Fog:
                HandleFog();
                break;
            case CardEvent.Supplydrop:
                HandleSupplyDrop();
                break;
            case CardEvent.PrisonBreak:
                HandlePrisonBreak();
                break;
            case CardEvent.Blackout:
                HandleBlackout();
                break;
            case CardEvent.Rain:
                HandleRain();
                break;
            case CardEvent.Swarm:
                HandleSwarm();
                break;
            case CardEvent.Infectednest:
                HandleInfectedNest();
                break;
            case CardEvent.HuntingFestival:
                HandleHuntingFestival();
                break;
            default:
                Debug.LogWarning("Unknown event!");
                break;
        }
    }

    // Individual handlers for each event
    void HandleNone()
    {
        Debug.Log("No event is active.");
    }

    void HandleFog()
    {
        Debug.Log("Fog event activated.");
        globalstat.expiditionspeed = 1.5f;
    }

    void HandleSupplyDrop()
    {
        Debug.Log("Supply drop event activated.");
        // Add your Supply Drop logic here
    }

    void HandlePrisonBreak()
    {
        Debug.Log("Prison break event activated.");
        globalstat.expiditionrisk = 80f;
        // Add your Prison Break logic here
    }

    void HandleBlackout()
    {
        Debug.Log("Blackout event activated.");
        // Add your Blackout logic here
    }

    void HandleRain()
    {
        Debug.Log("Rain event activated.");
        globalstat.expiditionspeed = 1.25f;
        globalstat.expiditionrisk = 1.1f;
        // Add your Rain logic here
    }

    void HandleSwarm()
    {
        Debug.Log("Swarm event activated.");
        // Add your Swarm logic here
    }

    void HandleInfectedNest()
    {
        Debug.Log("Infected nest event activated.");
        // Add your Infected Nest logic here
    }

    void HandleHuntingFestival()
    {
        Debug.Log("Hunting festival event activated.");
        // Add your Hunting Festival logic here
    }

    void SetEventDuration()
    {
        EventDuration duration = eventDurations.Find(ed => ed.cardEvent == cardEvent);
        if (duration != null)
        {
            DurationLeft = duration.GetRandomDuration();
        }
        else
        {
            // Default duration if event is not found
            DurationLeft = 0;
        }
    }
}
