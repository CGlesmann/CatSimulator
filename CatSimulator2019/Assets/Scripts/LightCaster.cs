using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCaster : MonoBehaviour
{
    [Header("References to Sleep Spots")]
    [SerializeField] private LayerMask solidLayer; // The Layer to raycast agaisn't
    [SerializeField] private SleepingZone[] zones;

    private float delay = 0f;

    private void Update()
    {
        
        delay -= Time.deltaTime;
        if (delay <= 0f)
        {
            CastLight();
            delay = 0f; //for debugging
        }
    }

    private void CastLight()
    {
        RaycastHit hit;
        foreach (SleepingZone zone in zones)
        {
            Quaternion q_dir = transform.rotation.normalized;
            Vector3 dir = q_dir * Vector3.back;

            int layerMask = 1 << 12;
            if (Physics.Raycast(zone.transform.position, dir, out hit, 1000.0f, layerMask)) {
                // Debug.DrawRay(zone.transform.position, dir, Color.red);
                // Debug.DrawRay(zone.transform.position, dir * hit.distance, Color.yellow);
                Debug.Log("Did Hit");
            }
            else {
                // Debug.DrawRay(zone.transform.position, dir, Color.green);
                // Debug.DrawRay(zone.transform.position, dir*1000, Color.white);
                Debug.Log("Did not Hit");
            }
        }
        
    }
}
