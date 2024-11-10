using UnityEngine;
using UnityEngine.Video;

public class TriggerSuara : MonoBehaviour
{
    public AudioSource audioSource; // Assign this in the Inspector

    private Collider triggerCollider; // Reference to the trigger collider

    private void Start()
    {
        // Ensure the audio source is stopped at the start
        audioSource.Stop();

        // Get the collider component attached to this game object
        triggerCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            audioSource.Play(); // Play audio if stopped or paused

            // Disable the collider to prevent re-triggering
            triggerCollider.enabled = false;
        }
    }
}
