using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GFXDestroyer : MonoBehaviour
{
    public void DestroyGFX()
    {
        GameObject.Destroy(gameObject);
    }
}
