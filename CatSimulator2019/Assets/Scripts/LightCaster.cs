using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCaster : MonoBehaviour
{
    [Header("References to Sleep Spots")]
    [SerializeField] private LayerMask solidLayer; // The Layer to raycast agaisn't
    [SerializeField] private LayerMask zoneLayer; 
    [SerializeField] private SleepingZone[] zones;

    private float delay = 0f;

    private void Update()
    {
        
        delay -= Time.deltaTime;
        if (delay <= 0f)
        {
            CastLight();
            delay = 1f;
        }
    }

    private void CastLight()
    {
        RaycastHit hit;
        foreach (SleepingZone zone in zones)
        {
            Quaternion q_dir = transform.rotation.normalized;
            Vector3 dir = q_dir * Vector3.forward;
        }
        
    }
}
