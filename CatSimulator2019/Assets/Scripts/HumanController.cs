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

    [Header("Human Searching Variables")]
    public LayerMask playerLayer;
    [SerializeField] private Vector3 searchOffset = Vector3.zero;
    [SerializeField] private Vector3 searchRange = Vector3.zero;

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

    private void Update()
    {
        // Checking for player
        Debug.Log(LookForPlayer() ? "Found Player" : "Can't See Player");
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

        // Rotating the Human
        transform.rotation = GetRotation();

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
        }
    }

    /// <summary>
    /// Gets a rotation based on motion vector
    /// </summary>
    /// <returns></returns>
    private Quaternion GetRotation()
    {
        if (motion != Vector3.zero)
        {
            if (Mathf.Abs(motion.x) > Mathf.Abs(motion.z))
            {
                // Left
                if (motion.x <= 0f) { return Quaternion.Euler(0f, 90f, 0f); }
                // Right
                if (motion.x > 0f) { return Quaternion.Euler(0f, -90f, 0f); }
            }
            else
            {
                // Forward
                if (motion.z <= 0f) { return Quaternion.Euler(0f, 0f, 0f); }
                // Backward
                if (motion.z > 0f) { return Quaternion.Euler(0f, 180f, 0f); }
            }
        }

        return transform.localRotation;
    }

    /// <summary>
    /// Returns if the player is in range
    /// </summary>
    /// <returns></returns>
    private bool LookForPlayer()
    {
        return (Physics.BoxCast(transform.position + searchOffset, searchRange / 2f, motion.normalized, transform.rotation, 0f, playerLayer));
    }

    /// <summary>
    /// Drawing the Seeking Range
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position + searchOffset, searchRange);
    }

}
