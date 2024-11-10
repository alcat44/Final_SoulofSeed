using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KursiGoyang : MonoBehaviour
{
    public AudioSource phoneRingSound;  // Assign audio source with the ringing sound
    public GameObject kakek;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has entered the trigger
        if (other.CompareTag("Player"))
        {
            // Play the ringing sound
            if (phoneRingSound != null && !phoneRingSound.isPlaying)
            {
                phoneRingSound.Play();
            }

            if (kakek != null)
            {
                kakek.SetActive(false);
            }

            // Disable the trigger so it won't ring again
            GetComponent<Collider>().enabled = false;
        }
    }
}
