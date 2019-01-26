using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HumanController : MonoBehaviour
{
    [Header("Human Movement Variables")]
    [SerializeField] private float humanWalkSpeed = 5f;
    [SerializeField] private float acceptanceRange = 0.1f;
    [SerializeField] private GameObject[] wayPoints = null;

    private Rigidbody rb; // Reference to the current Rigidbody
    private Vector3 motion; // The Current human velocity
    private int currentWayPoint = 0; // Tracks the Current Waypoint

    /// <summary>
    /// Human Setup
    /// </summary>
    private void Awake()
    {
        // Getting the Rigidbody reference
        rb = GetComponent<Rigidbody>();

        // Setting Currnet WayPoint to 0
        currentWayPoint = 0;
    }

    /// <summary>
    /// Human Movement
    /// </summary>
    private void FixedUpdate()
    {
        // Setting the Motion
        motion = (wayPoints[currentWayPoint].transform.position - transform.position).normalized * humanWalkSpeed;
        motion = new Vector3(motion.x, 0f, motion.z);
        transform.position += motion * Time.deltaTime;

        // Checking current goal
        CheckForNextWaypoint();
    }

    /// <summary>
    /// Checks Distane to next waypoint
    /// Changes waypoint if goal is reached
    /// </summary>
    private void CheckForNextWaypoint()
    {
        // Checking the Distance
        if (Vector3.Distance(transform.position, wayPoints[currentWayPoint].transform.position) <= acceptanceRange)
        {
            // Changing Waypoint
            if (++currentWayPoint >= wayPoints.Length)
                currentWayPoint = 0;

            // Resetting Motion
            motion = Vector3.zero;

            // Rotating the Human
            transform.rotation = GetRotation();
        }
    }

    private Quaternion GetRotation()
    {
       return Quaternion.Euler(0f, 0f, 0f);
    }

}
