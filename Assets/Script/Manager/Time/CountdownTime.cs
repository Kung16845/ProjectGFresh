using System.Collections;
using UnityEngine;

public class CountdownTime : MonoBehaviour
{
    public float timeScale;
    public float ratio;
    public float timeInSeconds;
    public float timeCount;
    private bool isCountdownComplete = false;

    private static readonly WaitForSeconds OneSecondWait = new WaitForSeconds(1f);
    private Coroutine _countdownCoroutine;

    private void Awake()
    {
        ratio = timeScale / 1000f;
        timeInSeconds = ratio * 60f;
        timeCount = timeInSeconds;
    }

    private void Start()
    {
        _countdownCoroutine = StartCoroutine(Countdown());
    }

    private void OnDisable()
    {
        if (_countdownCoroutine != null)
        {
            StopCoroutine(_countdownCoroutine);
            _countdownCoroutine = null;
        }
    }

    public IEnumerator Countdown()
    {
        while (timeCount > 0f)
        {
            yield return OneSecondWait;
            timeCount -= 1f;
        }

        isCountdownComplete = true;
        Destroy(gameObject);
    }

    public bool IsCountdownComplete()
    {
        return isCountdownComplete;
    }
}
