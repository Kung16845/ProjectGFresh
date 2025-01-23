using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;
    public Vector2 deadZoneSize = new Vector2(2f, 2f);  // Width and height of the dead zone
    public float followSpeed = 5f;  // Speed at which the camera follows the player

    private Vector2 deadZoneOffset;

    private void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform not assigned!");
        }

        // Calculate the offset from the center of the dead zone
        deadZoneOffset = deadZoneSize / 2f;
    }

    private void LateUpdate()
    {
        if (playerTransform == null) return;

        Vector3 cameraPosition = transform.position;
        Vector3 playerPosition = playerTransform.position;

        // Calculate the difference between the camera's position and the player's position
        Vector3 difference = playerPosition - cameraPosition;

        // Check if the player is outside the dead zone horizontally
        if (Mathf.Abs(difference.x) > deadZoneOffset.x)
        {
            cameraPosition.x = Mathf.Lerp(cameraPosition.x, playerPosition.x, followSpeed * Time.deltaTime);
        }

        // Check if the player is outside the dead zone vertically
        if (Mathf.Abs(difference.y) > deadZoneOffset.y)
        {
            cameraPosition.y = Mathf.Lerp(cameraPosition.y, playerPosition.y, followSpeed * Time.deltaTime);
        }

        // Update the camera's position
        transform.position = new Vector3(cameraPosition.x, cameraPosition.y, cameraPosition.z);
    }

    private void OnDrawGizmos()
    {
        // Draw the dead zone in the Scene view for visualization
        Gizmos.color = Color.red;
        Vector3 deadZoneCenter = new Vector3(transform.position.x, transform.position.y, 0);
        Gizmos.DrawWireCube(deadZoneCenter, new Vector3(deadZoneSize.x, deadZoneSize.y, 0));
    }
}
