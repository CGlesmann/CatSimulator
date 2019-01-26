using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CatController : MonoBehaviour
{
    // Static Reference
    public static CatController cat = null;

    [Header("Cat Movement Variables")]
    [SerializeField] private float catMoveSpeed = 50f;
    [SerializeField] private float catJumpSpeed = 200f;

    [Header("Stamina Usage Variables")]
    [Tooltip("Stamina Used by moving per second")] [SerializeField] private float runStaminaUse = 5f; // Per Second
    [Tooltip("Stamina Used by Jumping per Action")] [SerializeField] private float jumpStaminaUse = 10f; // Per Action

    private bool onGround = false; // Tracks if the cat is grounded
    private GameObject attachedWall = null; // The Current Wall he cat is attached to
    private Vector3 motion; // Current Momentum of the cat
    private Rigidbody rb; // Refernce to the rigidbody

    [Header("Cat Sleeping Variables")]
    [Range(1f, 100f)] public float stamina = 100f;
    [Range(1f, 100f)] public float comfort = 100f;
    [Range(1f, 100f)] public float irritation = 0f;

    [SerializeField] private float sleepInc = 25f; // Amount of stamina per second from sleeping
    private bool sleeping = false; // Tracks when the cat is sleeping

    /// <summary>
    /// Getting References / Setting Default Values
    /// </summary>
    private void Awake()
    {
        // Setting the Static Reference
        if (cat == null)
            cat = this;
        else
            GameObject.Destroy(gameObject);

        // Getting the RigidBody Reference
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Jumping and updating rotation
    /// </summary>
    private void Update()
    {
        if (!sleeping)
        {
            #region Jumping
            if (onGround)
            {
                // Checking for jumping
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // Applying the jump force
                    rb.AddForce(new Vector3(0f, catJumpSpeed, 0f));

                    // Tagging as inAir
                    onGround = false;

                    stamina = Mathf.Clamp(stamina - jumpStaminaUse, 0f, 100f);
                    if (stamina <= 0f)
                    {
                        // Start Sleeping
                        StartSleeping();
                    }
                }
            }
            #endregion

            #region Rotating
            // Rotating the Cat
            if (motion != Vector3.zero)
            {
                if (Mathf.Abs(motion.x) > Mathf.Abs(motion.z))
                {
                    // Left
                    if (motion.x <= 0f)
                    {
                        // Setting the Rotation
                        transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                        return;
                    }

                    // Right
                    if (motion.x > 0f)
                    {
                        // Setting the Rotation
                        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                        return;
                    }
                }
                else
                {
                    // Forward
                    if (motion.z > 0f)
                    {
                        // Setting the Rotation
                        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        return;
                    }

                    // Backward
                    if (motion.z <= 0f)
                    {
                        // Setting the Rotation
                        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                        return;
                    }
                }
            }
            #endregion
        }

        if (sleeping)
        {
            // Recovering Stamina
            float mult = (Input.GetKeyDown(KeyCode.E)) ? 7f : 1f;
            stamina = Mathf.Clamp(stamina + (sleepInc * mult * Time.deltaTime), 1f, 100f);

            // Adding Score
            GameManager.manager.playerScore += (10f * mult);

            if (stamina >= 100f)
            {
                // Exiting Sleep
                StopSleeping();
            }
        }
    }

    /// <summary>
    /// Movement
    /// </summary>
    private void FixedUpdate()
    {
        if (!sleeping)
        {
            // Getting Horizontal Movement
            motion = new Vector3(Input.GetAxis("Horizontal"), motion.y, Input.GetAxis("Vertical")) * catMoveSpeed;

            transform.position += motion * Time.deltaTime;

            // Taking away Stamina
            if (motion != Vector3.zero)
            {
                stamina = Mathf.Clamp(stamina - (runStaminaUse * Time.deltaTime), 0f, 100f);
                if (stamina <= 0f)
                {
                    // Start Sleeping
                    StartSleeping();
                }
            }
        }
        else
            motion = Vector3.zero;
    }

    /// <summary>
    /// Checking for collision with ground/walls
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // Getting the collided gameObject
        GameObject obj = collision.gameObject;

        // Checking to see if we hit ground
        if (obj.tag == "Ground")
        {
            onGround = true;
        }

        // Checking to see if we hit a wall
        if (obj.tag == "Wall")
        {
            motion = new Vector3(0f, motion.y, 0f);
            attachedWall = obj;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.tag == "Wall")
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Wall Jumping");

                Vector3 jumpMotion = (attachedWall.transform.position - transform.position).normalized;
                rb.AddForce(new Vector3(jumpMotion.x, catJumpSpeed, jumpMotion.z));

                attachedWall = null;
            }
        }
    }

    /// <summary>
    /// Detaching from the wall
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision)
    {
        // Getting the GameObject
        GameObject obj = collision.gameObject;

        if (obj.tag == "Wall")
        {
            attachedWall = null;
        }
    }

    /// <summary>
    /// Checking for a sleep zone
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        // Getting the GameObject
        GameObject obj = other.gameObject;

        // Checking for a sleep zone
        if (obj.tag == "SleepingSpot")
        { 
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartSleeping();
            }
        }
    }

    /// <summary>
    /// Entering Sleep
    /// </summary>
    private void StartSleeping()
    {
        // Setting Sleep
        sleeping = true;
    }

    /// <summary>
    /// Exiting Sleep
    /// </summary>
    private void StopSleeping()
    {
        sleeping = false;
    }

}
