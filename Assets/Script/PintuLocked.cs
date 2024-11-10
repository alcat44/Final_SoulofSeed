using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PintuLocked : MonoBehaviour
{
    public DoorUtama doorScript; // Reference to the Door script
    public Collider doorCollider; // Collider to be deactivated after closing the door
    public Collider doorCollider2;
    public GameObject Gembok;
    public GameObject trigger;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (doorScript.toggle == true) // Check if the door is currently open
            {
                // Close the door by triggering the animation and sound
                doorScript.doorAnim.ResetTrigger("open");
                doorScript.doorAnim.SetTrigger("close");
                doorScript.audioSource.PlayOneShot(doorScript.closeSound);

                // Disable the collider to prevent further interaction
                doorCollider.enabled = false;
                doorCollider2.enabled = false;
                Gembok.SetActive(true);

                // Update the door's toggle state to closed
                doorScript.toggle = false;
                trigger.SetActive(false);
            }
        }
    }
}
