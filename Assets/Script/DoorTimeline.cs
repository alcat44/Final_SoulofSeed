using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTimeline : MonoBehaviour
{
    public GameObject intText; // Interact text for player
    public bool interactable, toggle;
    public Animator doorAnim;
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;
    public GameObject Timeline;
    
    // Added colliders for the door
    public Collider doorCollider; 
    public Collider doorCollider2; 

    private bool isEnemyNearby = false; // To check if the enemy is already near the door
    private bool isActionInProgress = false; // Flag to prevent spam triggering

    void OnTriggerStay(Collider other)
    {
        // Player interaction (using "MainCamera" tag)
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(true);
            interactable = true;
        }

        // Enemy interaction (using "Enemy" tag)
        if (other.CompareTag("Enemy") && !isActionInProgress)
        {
            if (!toggle && !isEnemyNearby)
            {
                isEnemyNearby = true; // Set flag to prevent repeated triggers
                StartCoroutine(OpenDoor());
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Player exits trigger (using "MainCamera" tag)
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false);
            interactable = false;
        }

        // Enemy exits trigger (using "Enemy" tag)
        if (other.CompareTag("Enemy") && !isActionInProgress)
        {
            if (toggle && isEnemyNearby)
            {
                isEnemyNearby = false; // Reset flag when enemy leaves
                StartCoroutine(CloseDoor());
            }
        }
    }

    void Update()
    {
        // Player manual interaction (using Key "E")
        if (interactable && !isActionInProgress)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                toggle = !toggle;
                intText.SetActive(false); // Hide interaction text
                interactable = false; // Set interactable to false

                if (toggle)
                {
                    // Check if Timeline still exists before trying to activate it
                    if (Timeline != null)
                    {
                        Timeline.SetActive(true);
                    }
                    else
                    {
                        Debug.LogWarning("Timeline has been destroyed, cannot activate!");
                    }

                    StartCoroutine(OpenDoor());
                }
                else
                {
                    StartCoroutine(CloseDoor());
                }
            }
        }
    }

    IEnumerator OpenDoor()
    {
        isActionInProgress = true; // Set the flag to prevent action spam
        doorAnim.ResetTrigger("close");
        doorAnim.SetTrigger("open");
        audioSource.PlayOneShot(openSound);
        toggle = true;

        // Disable colliders during animation
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }
        if (doorCollider2 != null)
        {
            doorCollider2.enabled = false;
        }

        yield return new WaitForSeconds(doorAnim.GetCurrentAnimatorStateInfo(0).length); // Wait for the door to open

        // Enable colliders after animation
        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }
        if (doorCollider2 != null)
        {
            doorCollider2.enabled = true;
        }

        isActionInProgress = false; // Reset the flag after door fully opens
    }

    IEnumerator CloseDoor()
    {
        isActionInProgress = true; // Set the flag to prevent action spam
        doorAnim.ResetTrigger("open");
        doorAnim.SetTrigger("close");
        audioSource.PlayOneShot(closeSound);
        toggle = false;

        // Disable colliders during animation
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }
        if (doorCollider2 != null)
        {
            doorCollider2.enabled = false;
        }

        yield return new WaitForSeconds(doorAnim.GetCurrentAnimatorStateInfo(0).length); // Wait for the door to close

        // Enable colliders after animation
        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }
        if (doorCollider2 != null)
        {
            doorCollider2.enabled = true;
        }

        isActionInProgress = false; // Reset the flag after door fully closes
    }

    public void ResetInteraction()
    {
        intText.SetActive(false); // Make sure interaction text is off
        interactable = false;     // Reset interactable flag
    }
}
