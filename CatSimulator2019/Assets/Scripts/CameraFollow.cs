using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Follow Variables")]
    [SerializeField] private Transform target = null;
    [SerializeField] private float camSpeed = 0.1f;
    private Vector3 cameraOffset = Vector3.zero;

    private void Awake()
    {
        // Getting the current offset
        cameraOffset = (transform.position - target.position);
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            // Updating the Camera Position
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, target.position.x + cameraOffset.x, camSpeed),
                                             transform.position.y,
                                             Mathf.Lerp(transform.position.z, target.position.z + cameraOffset.z, camSpeed));
        }
    }
}
