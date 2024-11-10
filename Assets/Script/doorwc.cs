using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorwc : MonoBehaviour
{
    public GameObject intText; // Interact text for player
    public bool interactable, toggle;
    public Animator doorAnim;
    public AudioSource audioSource;
    public AudioSource firstInteractionAudioSource; // AudioSource tambahan
    public AudioClip openSound;
    public AudioClip closeSound;
    public Collider doorCollider; // Collider pintu
    public Collider doorCollider2; // Collider pintu kedua
    public Collider keyCollider; // Collider for the key

    private bool isEnemyNearby = false; // To check if the enemy is already near the door
    private bool hasActivatedOnce = false; // To ensure the mechanic only happens once
    private bool hasPlayedFirstInteractionSound = false; // Flag untuk memutar suara hanya sekali

    // Tambahkan dua GameObject untuk perubahan status
    public GameObject gameObject1; // Akan menyala saat pintu dibuka pertama kali
    public GameObject gameObject2; // Akan mati saat pintu dibuka pertama kali

    void OnTriggerStay(Collider other)
    {
        // Player interaction (using "MainCamera" tag)
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(true);
            interactable = true;
        }

        // Enemy interaction (using "Enemy" tag)
        if (other.CompareTag("Enemy"))
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
        if (other.CompareTag("Enemy"))
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
        if (interactable)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                toggle = !toggle;
                interactable = false;   // Set interactable to false when pressing E
                intText.SetActive(false); // Hide interaction text when pressing E

                if (toggle)
                {
                    StartCoroutine(OpenDoor());
                }
                else
                {
                    StartCoroutine(CloseDoor());
                }

                if (!hasActivatedOnce)
                {
                    hasActivatedOnce = true; // Set to true to ensure first action only once
                }
            }
        }
    }

    IEnumerator OpenDoor()
    {
        doorAnim.ResetTrigger("close");
        doorAnim.SetTrigger("open");
        audioSource.PlayOneShot(openSound);

        // Disable the door colliders during the animation
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }
        if (doorCollider2 != null)
        {
            doorCollider2.enabled = false;
        }

        // Activate the key collider when the door opens
        if (keyCollider != null)
        {
            keyCollider.enabled = true;
        }

        // Efek khusus untuk interaksi pertama kali
        if (!hasActivatedOnce)
        {
            // Hanya dijalankan sekali saat interaksi pertama
            gameObject1.SetActive(true);
            gameObject2.SetActive(false);

            if (!hasPlayedFirstInteractionSound)
            {
                firstInteractionAudioSource.Play(); // Play sound only once
                hasPlayedFirstInteractionSound = true; // Set to true to prevent replay
            }

            // Tunggu 7 detik sebelum mengubah kembali kondisi GameObject1 dan GameObject2
            yield return new WaitForSeconds(7);

            // Membalik kondisi GameObject1 dan GameObject2
            gameObject1.SetActive(false);
            gameObject2.SetActive(true);
        }

        // Wait until the animation is finished
        yield return new WaitForSeconds(doorAnim.GetCurrentAnimatorStateInfo(0).length);

        // Enable the door colliders after the animation
        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }
        if (doorCollider2 != null)
        {
            doorCollider2.enabled = true;
        }

        toggle = true;
    }

    IEnumerator CloseDoor()
    {
        doorAnim.ResetTrigger("open");
        doorAnim.SetTrigger("close");
        audioSource.PlayOneShot(closeSound);

        // Disable the door colliders during the animation
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }
        if (doorCollider2 != null)
        {
            doorCollider2.enabled = false;
        }

        // Deactivate the key collider when the door closes
        if (keyCollider != null)
        {
            keyCollider.enabled = false;
        }

        // Wait until the animation is finished
        yield return new WaitForSeconds(doorAnim.GetCurrentAnimatorStateInfo(0).length);

        // Enable the door colliders after the animation
        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }
        if (doorCollider2 != null)
        {
            doorCollider2.enabled = true;
        }

        toggle = false;
    }

    public void ResetInteraction()
    {
        intText.SetActive(false); // Make sure interaction text is off
        interactable = false;     // Reset interactable flag
    }
}
