using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public static DontDestroy Instance { get; private set; }
    private static bool instanceExists = false;
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
