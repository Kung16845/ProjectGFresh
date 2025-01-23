using UnityEngine;

public class TradeManager : MonoBehaviour
{
    public static TradeManager Instance { get; private set; }

    private TradesystemScript activeTrade;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public TradesystemScript GetActiveTrade()
    {
        return activeTrade;
    }

    public void ClearActiveTrade()
    {
        activeTrade = null;
    }
}
