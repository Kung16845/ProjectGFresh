using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
public class NpcMovement : MonoBehaviour
{
    public List<Transform> listWayPointWalk;
    public Transform currentWayPoint;
    private Vector3 lastPosition;
    public NavMeshAgent agent;
    public AnimationController animationController;
    public bool isMoving = false;
    public float timeCount = 0;
    public float maxTimeCount;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<AnimationController>();
        transform.rotation = Quaternion.Euler(0, 0, 0);
        isMoving = true;
        lastPosition = transform.position;
        agent.avoidancePriority = Random.Range(0, 100); // กำหนดค่าความสำคัญแบบสุ่ม
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance; // ปิดการหลีกเลี่ยงสิ่งกีดขวาง

    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);

        if (transform.position != lastPosition)
        {
            // ถ้าตำแหน่งเปลี่ยน ทำอะไรบางอย่าง
            animationController.iswalk = true;
            CheckHorizontalMovement();
            // อัปเดตตำแหน่งล่าสุด
            lastPosition = transform.position;
        }
        else 
        {
            animationController.iswalk = false;
        }
        if (isMoving)
        {
            WalkWaypoint();

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                isMoving = WalkContinue();
            }

        }
        else if (!isMoving)
        {
            timeCount += Time.deltaTime;
          
            if (timeCount >= maxTimeCount)
            {

                timeCount = 0;
                WalkWaypoint();
            }
        }

    }
     void CheckHorizontalMovement()
    {
        if (transform.position.x > lastPosition.x)
        {
            // Debug.Log("Moved Right!");
            transform.localScale = new Vector3(0.6f, 0.6f, 1);
        }
        else if (transform.position.x < lastPosition.x)
        {
            // Debug.Log("Moved Left!");
            transform.localScale = new Vector3(-0.6f, 0.6f, 1);
        }
    }
    public void WalkWaypoint()
    {
        currentWayPoint = listWayPointWalk.ElementAt(Random.Range(0, listWayPointWalk.Count));
        agent.SetDestination(currentWayPoint.position);
        // 
    }
    public bool WalkContinue()
    {
        float randomValue = Random.Range(0f, 100f);
        maxTimeCount = Random.Range(5, 16);
        return randomValue >= 50.00f;
    }
}
