using UnityEngine;

public class SlimeProjectile : MonoBehaviour
{
    private Vector2 targetPosition;
    private float speed;

    private float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Initialize(Vector2 targetPos, float projectileSpeed)
    {
        targetPosition = targetPos;
        speed = projectileSpeed;
        Vector2 direction = targetPosition - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        // Move the projectile towards the target position
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ActionController player = other.GetComponent<ActionController>();
        if (player != null)
        {
            // Start the stuck state
            player.StartStuck();

            // Destroy the projectile
            Destroy(gameObject);
        }
    }
}
