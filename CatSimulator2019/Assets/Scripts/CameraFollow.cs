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
        cameraOffset = (target.position - cameraOffset);
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            // Updating the Camera Position
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, transform.position.x + cameraOffset.x, camSpeed),
                                             Mathf.Lerp(transform.position.y, transform.position.y + cameraOffset.y, camSpeed),
                                             Mathf.Lerp(transform.position.z, transform.position.z + cameraOffset.z, camSpeed));
        }
    }
}
