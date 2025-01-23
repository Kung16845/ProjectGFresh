using System.Collections.Generic;
using UnityEngine;

public class ZombieSlimer : Zombie
{
    private enum SlimerStates
    {
        Moving,
        Attacking,
        SwitchingLanes
    }

    [Header("Slimer Settings")]
    public float attackingDuration = 5f;         // Total time spent attacking
    public float attackInterval = 1f;            // Time between each projectile launch
    public GameObject slimeProjectilePrefab;     // Prefab of the slime projectile to spawn
    public float projectileSpeed = 5f;           // Speed of the projectile

    private SlimerStates slimerState;
    private float SlimeattackTimer = 0f;
    private float attackCooldown = 0f;

    // Reference to the engaging area position
    public Transform engagingPoint;

    // List of all lanes for lane switching
    private List<Lane> allLanes;
    private AnimationControllerGrunt animationControllerGrunt;

    // Reference to the player
    private Transform playerTransform;

    public void SetZombieCostumeId()
    {
        string mutationCode = GetMutationCode(mutationType);
        idZombieCoustume = $"30105{mutationCode}";
    }

    protected override void Start()
    {
        base.Start();
        SetZombieCostumeId();
        animationControllerGrunt = GetComponent<AnimationControllerGrunt>();
        rb2D = GetComponent<Rigidbody2D>();
        // Initialize variables
        slimerState = SlimerStates.Moving;
        // Get all lanes from LaneManager
        allLanes = LaneManager.Instance.allLanes;

        // Check if allLanes is set
        if (allLanes == null || allLanes.Count == 0)
        {
            Debug.LogError("Slimer: allLanes is not set!");
        }
        else
        {
            // Set currentLane if not already set
            if (currentLane == null)
            {
                currentLane = GetClosestLane();
                SetLane(currentLane);
            }

            // Set engagingPoint based on currentLane
            if (currentLane != null)
            {
                engagingPoint = currentLane.engagingArea;
            }
            else
            {
                Debug.LogError("Slimer: currentLane is not set!");
            }
        }

        // Find the player in the scene
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Slimer: Player not found in the scene!");
        }
    }

    private Lane GetClosestLane()
    {
        float closestDistance = Mathf.Infinity;
        Lane closestLane = null;
        Vector3 position = transform.position;

        foreach (Lane lane in allLanes)
        {
            float distance = Vector2.Distance(position, lane.engagingArea.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestLane = lane;
            }
        }

        return closestLane;
    }

    private void Update()
    {
        switch (slimerState)
        {
            case SlimerStates.Moving:
                HandleMovingState();
                break;
            case SlimerStates.Attacking:
                HandleAttackingState();
                break;
            case SlimerStates.SwitchingLanes:
                HandleSwitchingLanesState();
                break;
            default:
                break;
        }
    }

    private void HandleMovingState()
    {
        if (HasReachedEngagingPoint())
        {
            slimerState = SlimerStates.Attacking;
            SlimeattackTimer = attackingDuration;
            attackCooldown = 0f;
            rb2D.velocity = Vector2.zero; // Stop moving while attacking
        }
        else
        {
            animationControllerGrunt.IsWalk = true;
            animationControllerGrunt.IsAttack = false;
            if(canmove)
            {
                MoveTowards(engagingPoint.position);
            }
        }
    }

    private void HandleAttackingState()
    {
        animationControllerGrunt.IsAttack = true;
        SlimeattackTimer -= Time.deltaTime;
        attackCooldown -= Time.deltaTime;

        if (attackCooldown <= 0f)
        {
            animationControllerGrunt.Isreach = true;
            LaunchProjectile();
            attackCooldown = attackInterval;
        }

        if (SlimeattackTimer <= 0f)
        {
            animationControllerGrunt.Isreach = false;
            slimerState = SlimerStates.SwitchingLanes;
        }
    }

    private void HandleSwitchingLanesState()
    {
        SwitchToRandomLane();
    }

    private bool HasReachedEngagingPoint()
    {
        if (engagingPoint != null)
        {
            float distanceToEngagingPoint = Vector2.Distance(transform.position, engagingPoint.position);
            float thresholdDistance = 0.1f;
            return distanceToEngagingPoint <= thresholdDistance;
        }
        return false;
    }

    private void MoveTowards(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb2D.velocity = direction * currentSpeed;
         if (direction.x < 0)
            {
                transform.localScale = new Vector3(0.7f, 0.7f, 1); // Flip left
            }
            else if (direction.x > 0)
            {
                transform.localScale = new Vector3(-0.7f, 0.7f, 1); // Face right
            }
    }

    private void LaunchProjectile()
    {
        if (slimeProjectilePrefab != null && playerTransform != null)
        {
            GameObject projectile = Instantiate(slimeProjectilePrefab, transform.position, Quaternion.identity);
            SlimeProjectile slimeProjectileScript = projectile.GetComponent<SlimeProjectile>();
            if (slimeProjectileScript != null)
            {
                slimeProjectileScript.Initialize(playerTransform.position, projectileSpeed);
            }
        }
        else
        {
            Debug.LogWarning("Slimer: Cannot launch projectile because slimeProjectilePrefab or playerTransform is null.");
        }
    }

    private void SwitchToRandomLane()
    {
        if (allLanes != null && allLanes.Count > 1)
        {
            // Exclude current lane from selection
            List<Lane> otherLanes = new List<Lane>(allLanes);
            otherLanes.Remove(currentLane);

            // Select a random lane
            int randomIndex = Random.Range(0, otherLanes.Count);
            Lane newLane = otherLanes[randomIndex];

            // Update the current lane and engaging point
            SetLane(newLane);

            // Do not change the Slimer's position; it will start moving towards the new engaging point from its current position

            // Reset the state to moving
            slimerState = SlimerStates.Moving;
        }
        else
        {
            // No other lanes to switch to
            slimerState = SlimerStates.Moving;
        }
    }

    public override void SetLane(Lane lane)
    {
        base.SetLane(lane);
        engagingPoint = lane.engagingArea;
    }
}
