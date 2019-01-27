using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GFXDestroyer : MonoBehaviour
{
    [SerializeField] private AudioClip playSound;
    private AudioSource player;

    private void Awake()
    {
        player = GetComponent<AudioSource>();
        player.clip = playSound;
        player.Play();
    }

    public void DestroyGFX()
    {
        GameObject.Destroy(gameObject);
    }
}
