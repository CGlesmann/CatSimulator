using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepingZone : MonoBehaviour
{
    // The Amount of levels
    private const int levelCount = 3;

    [Header("Sleeping Zone Color Control")]
    [SerializeField] private Material baseMat = null;
    [SerializeField] private Material[] colors = new Material[levelCount]; // Stores materials for each colorLevel
    private int colorLevel = 0; // Tracks current color level

    [Header("Scoring Levels")]
    [SerializeField] private float[] colorValues = new float[levelCount];
    [SerializeField] private float changeDelay = 20f;
    private float changeTimer = 0f;

    [Header("Animation Variables")]
    [SerializeField] private float animDelay = 0.5f;
    private float animTimer = 0f;

    private bool colored = false;
    private MeshRenderer rend;

    private void Awake()
    {
        // Setting the Timer
        changeTimer = changeDelay;
        colorLevel = 0;

        rend = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        // Updating the Material
        if (animTimer > 0f)
        {
            animTimer -= Time.deltaTime;
            if (animTimer <= 0)
            {
                if (colored)
                {
                    rend.material = baseMat;
                    colored = false;
                } else {
                    rend.material = colors[colorLevel];
                    colored = true;
                }

                animTimer = animDelay;
            }
        }
    }
}
