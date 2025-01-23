using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public float targetSize = 10f; // Desired camera size when the player enters
    public Vector3 targetPosition; // The center position you want the camera to move to
    public float transitionSpeed = 2f; // Speed of the camera transition

    private Camera mainCamera;
    public bool playerInZone;

    private float originalSize; // Store the original camera size
    private Vector3 originalPosition; // Store the original camera position

    void Start()
    {
        mainCamera = Camera.main;
        originalSize = mainCamera.orthographicSize;
        originalPosition = mainCamera.transform.position;
    }

    void Update()
    {
        if (playerInZone)
        {
            // Smoothly interpolate camera size and position to the target
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetSize, transitionSpeed * Time.deltaTime);
            // mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, transitionSpeed * Time.deltaTime);
        }
        else
        {
            // Smoothly return camera size and position to the original
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, originalSize, transitionSpeed * Time.deltaTime);
            // mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, originalPosition, transitionSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
        }
    }
}
