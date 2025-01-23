using System.Collections.Generic;
using UnityEngine;

public class ZombieReeker : Zombie
{
    private enum ReekerState
    {
        Moving,
        Spitting,
        SwitchingLanes
    }

    [Header("Reeker Settings")]
    public float spittingDuration = 5f;    // Total time spent spitting acid
    public float spittingInterval = 1f;    // Time between each acid spit
    public GameObject reekerAcidPoolPrefab; // Prefab of the AcidPool to spawn (renamed to avoid conflicts)

    // Acid Pool Stats
    [Header("Reeker Acid Pool Stats")]
    public float reekerAcidBarrierDamage = 5f;
    public float reekerAcidZombieDamage = 5f;
    public float reekerAcidDuration = 5f;
    public float reekerAcidRadius = 1f;
    public float reekerAcidInterval = 1f;

    private ReekerState ReekercurrentState;
    private float spittingTimer = 0f;
    private float spittingCooldown = 0f;

    // Reference to the engaging area position
    public Transform engagingPoint;
    public AnimationControllerGrunt animationControllerGrunt;

    // List of all lanes for lane switching
    private List<Lane> allLanes;
    public void SetZombieCostumeId()
    {
        string mutationCode = GetMutationCode(mutationType);
        idZombieCoustume = $"30106{mutationCode}";
        Debug.Log($"Zombie Costume ID set to: {idZombieCoustume}");
    }
    protected override void Start()
    {
        base.Start();
        SetZombieCostumeId();
        animationControllerGrunt = GetComponent<AnimationControllerGrunt>();
        rb2D = GetComponent<Rigidbody2D>();
        // Initialize variables
        currentSpeed = maxSpeed;
        ReekercurrentState = ReekerState.Moving;
        countTimer = attackTimer; // Initialize attack timer
        // Get all lanes from LaneManager
        allLanes = LaneManager.Instance.allLanes;

        // Check if allLanes is set
        if (allLanes == null || allLanes.Count == 0)
        {
            Debug.LogError("Reeker: allLanes is not set!");
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
                Debug.LogError("Reeker: currentLane is not set!");
            }
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
        switch (ReekercurrentState)
        {
            case ReekerState.Moving:
                HandleMovingState();
                break;
            case ReekerState.Spitting:
                HandleSpittingState();
                break;
            case ReekerState.SwitchingLanes:
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
            ReekercurrentState = ReekerState.Spitting;
            spittingTimer = spittingDuration;
            spittingCooldown = 0f;
            rb2D.velocity = Vector2.zero; // Stop moving while spitting
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

    private void HandleSpittingState()
    {
        animationControllerGrunt.IsAttack = true;
        spittingTimer -= Time.deltaTime;
        spittingCooldown -= Time.deltaTime;

        if (spittingCooldown <= 0f)
        {
            animationControllerGrunt.Isreach = true;
            SpitAcid();
            spittingCooldown = spittingInterval;
        }

        if (spittingTimer <= 0f)
        {
            animationControllerGrunt.Isreach = false;
            ReekercurrentState = ReekerState.SwitchingLanes;
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

    private void SpitAcid()
    {
        if (reekerAcidPoolPrefab != null && currentLane != null && currentLane.attackPoint != null)
        {
            GameObject acidPool = Instantiate(reekerAcidPoolPrefab, currentLane.attackPoint.position, Quaternion.identity);
            AcidPool acidPoolScript = acidPool.GetComponent<AcidPool>();
            if (acidPoolScript != null)
            {
                acidPoolScript.Initialize(
                barrierDamage: reekerAcidBarrierDamage,
                zombieDamage: reekerAcidZombieDamage,
                duration: reekerAcidDuration,
                radius: reekerAcidRadius,
                interval: reekerAcidInterval
            );
            }
        }
        else
        {
            Debug.LogWarning("Reeker: Cannot spit acid because reekerAcidPoolPrefab, currentLane, or attackPoint is null.");
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

            // Do not change the Reeker's position; it will start moving towards the new engaging point from its current position

            // Reset the state to moving
            ReekercurrentState= ReekerState.Moving;
        }
        else
        {
            // No other lanes to switch to
            ReekercurrentState = ReekerState.Moving;
        }
    }

    public override void SetLane(Lane lane)
    {
        base.SetLane(lane);
        engagingPoint = lane.engagingArea;
    }
}
