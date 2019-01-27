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
    private GameObject attachedWall = null; // The Current Wall the cat is attached to
    private Vector3 motion; // Current Momentum of the cat
    private Rigidbody rb; // Refernce to the rigidbody

    [Header("Cat Sleeping Variables")]
    public float stamina = 75f;
    public float staminaMax = 75f;
    public float comfort = 75f, comfortMax = 75f;
    public float irritation = 0f, irritationMax = 75f;

    [SerializeField] private float sleepInc = 25f; // Amount of stamina per second from sleeping
    [SerializeField] private GameObject previousZone = null; // The Previous Sleep zone the player used
    private bool canMove = true;
    private bool sleeping = false; // Tracks when the cat is sleeping
    private bool inSleepingZone = false;

    private Animator anim;

    [Header("Cat Sound References")]
    [SerializeField] private AudioSource sfxPlayer = null;
    [SerializeField] private AudioClip walkSound = null;
    [SerializeField] private AudioClip jumpSound = null;
    [SerializeField] private AudioClip landSound = null;

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

        // Getting the Animator Referene
        anim = GetComponent<Animator>();
        anim.SetBool("moving", false);
    }

    /// <summary>
    /// Jumping and updating rotation
    /// </summary>
    private void Update()
    {
        if (!sleeping && canMove)
        {
            #region Jumping
            if (onGround)
            {
                // Checking for jumping
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    sfxPlayer.clip = jumpSound;
                    sfxPlayer.Play();

                    // Applying the jump force
                    rb.AddForce(new Vector3(0f, catJumpSpeed, 0f), ForceMode.Impulse);

                    // Tagging as inAir
                    onGround = false;

                    anim.SetBool("jumping", true);
                    anim.SetBool("moving", false);

                    stamina = Mathf.Clamp(stamina - jumpStaminaUse, 0f, 100f);
                }

                if (stamina <= 0f)
                {
                    GetInSleepingPosition();
                }

                

            }
            #endregion

            // Updating the Animatior
            if (!onGround)
            {
                if (rb.velocity.y > 0f)
                {
                    Debug.Log("Flying");
                    anim.SetBool("jumping", true);
                    anim.SetBool("falling", false);
                }
                else
                {
                    Debug.Log("Falling");
                    anim.SetBool("jumping", false);
                    anim.SetBool("falling", true);
                }
            }
            else
            {
                anim.SetBool("jumping", false);
                anim.SetBool("falling", false);
            }

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
                        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                        return;
                    }

                    // Right
                    if (motion.x > 0f)
                    {
                        // Setting the Rotation
                        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        return;
                    }
                }
                else
                {
                    // Forward
                    if (motion.z > 0f)
                    {
                        // Setting the Rotation
                        transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                        return;
                    }

                    // Backward
                    if (motion.z <= 0f)
                    {
                        // Setting the Rotation
                        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
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
            stamina = Mathf.Clamp(stamina + (sleepInc * mult * Time.deltaTime), 1f, staminaMax);

            // Adding Score
            GameManager.manager.IncreaseScore(10f * mult);

            if (stamina >= staminaMax)
            {
                // Exiting Sleep
                StopSleeping();

                // Checking Multiplier
                if (inSleepingZone)
                    GameManager.manager.IncreaseMultiplier();
                else
                    GameManager.manager.ResetMultiplier();

                inSleepingZone = false;
            }
        }
    }

    /// <summary>
    /// Movement
    /// </summary>
    private void FixedUpdate()
    {
        if (!sleeping && canMove)
        {
            // Getting Horizontal Movement
            motion = GetMotion();

            transform.position += motion * Time.deltaTime;

            // Taking away Stamina
            if (motion != Vector3.zero)
            {
                stamina = Mathf.Clamp(stamina - (runStaminaUse * Time.deltaTime), 0f, 100f);
                anim.SetBool("moving", true);

                // Playing the sound if nothing else is
                if (!sfxPlayer.isPlaying && onGround)
                {
                    sfxPlayer.clip = walkSound;
                    sfxPlayer.Play();
                }
            } else
            {
                anim.SetBool("moving", false);
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

        // Checking to see if we hit a wall
        if (obj.tag == "Wall")
        {
            motion = new Vector3(0f, motion.y, 0f);
            attachedWall = obj;
        }

        if (obj.tag == "Human Head")
        {
            Debug.Log("Hit the Head");

            // Checking for downward motion 
            if (rb.velocity.y < 0f)
            {
                Debug.Log("Bouncing");
                // Bouncing the cat
                rb.AddForce(new Vector3(0f, catJumpSpeed, 0f), ForceMode.Impulse);
            }
        }

        if (stamina <= 0f)
        {
            // Start Sleeping
            GetInSleepingPosition();
        }

        onGround = true;
        anim.SetBool("jumping", false);

        sfxPlayer.clip = landSound;
        sfxPlayer.Play();
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
    /// Bouncing off the human head
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;

        // Checking for bouncing off the human head
        if (obj.tag == "Human Head")
        {
            if (rb.velocity.y < 0f)
            {
                Debug.Log("Bouncing");
                // Bouncing the cat
                rb.AddForce(new Vector3(0f, catJumpSpeed * 2f, 0f), ForceMode.Impulse);
            }
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
            if (!sleeping)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    GetInSleepingPosition(obj);
                    inSleepingZone = true;
                }
            }
        }
    }

    /// <summary>
    /// Gets Input on a scale of 0 or 1
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMotion()
    {
        // Declaring Variables
        float x = 0f, z = 0f;

        // Setting X
        if (Input.GetKey(KeyCode.A)) { x -= 1f; }
        if (Input.GetKey(KeyCode.D)) { x += 1f; }

        // Setting Z
        if (Input.GetKey(KeyCode.W)) { z += 1f; }
        if (Input.GetKey(KeyCode.S)) { z -= 1f; }

        // Returning the result
        return (new Vector3(x, motion.y, z) * catMoveSpeed);
    }

    public void GetInSleepingPosition(GameObject zone = null)
    {
        if (previousZone == null || (zone != previousZone))
        {
            //Setting the Previous Zone
            previousZone = zone;

            // Setting Sleep
            canMove = false;
            anim.SetBool("moving", false);
            anim.SetBool("sleeping", true);
        }
    }

    /// <summary>
    /// Entering Sleep
    /// </summary>
    private void StartSleeping()
    {
        sleeping = true;
    }

    /// <summary>
    /// Exiting Sleep
    /// </summary>
    public void StopSleeping()
    {
        Debug.Log("Waking Upp");
        sleeping = false;
        anim.SetBool("sleeping", false);
    }

    public void WakeUp()
    {
        canMove = true;
    }
}
