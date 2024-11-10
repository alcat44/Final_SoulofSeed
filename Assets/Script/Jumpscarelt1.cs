using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumpscarelt1 : MonoBehaviour
{
    public GameObject jumpscare;
    public AudioSource scareSound;
    public Collider collision;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jumpscare.SetActive(true);
            if (scareSound != null)
            {
                scareSound.Play();
            }
            collision.enabled = false;
        }
    }
}
