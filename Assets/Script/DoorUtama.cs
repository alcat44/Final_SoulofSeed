using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables; // Required for Timeline
using UnityEngine.SceneManagement; // Required for Scene Management

public class DoorUtama : MonoBehaviour
{
    public GameObject intText; // Interact text for player
    public GameObject exhaustionText;
    public bool interactable, toggle;
    public Animator doorAnim;
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    private bool isEnemyNearby = false; // To check if the enemy is already near the door
    private bool hasOpenedOnce = false; // Flag to check if the door has been opened by the player
    public bool interactionReset = false; // New flag to track interaction reset

    // Tambahkan referensi ke DropArea
    public DropArea dropArea; // DropArea script untuk menghentikan countdown

    // Timeline and Camera references
    public PlayableDirector timeline; // Reference to the timeline
    public Camera mainCamera; // Reference to the main camera
    public Camera timelineCamera; // Reference to the timeline camera
    public SC_FPSController playerScript;
    public AudioSource audioscource;

    void Start()
    {
        // Daftarkan listener untuk event stopped dari timeline
        if (timeline != null)
        {
            timeline.stopped += OnTimelineStopped;
        }
    }

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
        if (toggle && isEnemyNearby)
        {
            isEnemyNearby = false; // Reset flag when enemy leaves
            StartCoroutine(CloseDoor()); // CloseDoor digunakan pada kondisi ini
        }
    }

    void Update()
    {
        // Player manual interaction (using Key "E")
        if (interactable)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!hasOpenedOnce) // Only allow opening door on the first interaction
                {
                    toggle = true; // Set toggle to true only to open the door
                    StartCoroutine(OpenDoor());
                    hasOpenedOnce = true; // Prevent further interactions after the first open
                }
                else if (interactionReset) // Allow interaction after reset
                {
                    toggle = true; // Open the door again after reset
                    StartCoroutine(OpenDoor());
                }
            }
        }
    }

    IEnumerator OpenDoor()
    {
        doorAnim.ResetTrigger("close");
        doorAnim.SetTrigger("open");
        audioSource.PlayOneShot(openSound);
        toggle = true;

        // Jika interactionReset true, stop countdown dan play timeline
        if (interactionReset)
        {
            if (dropArea != null)
            {
                dropArea.StopCountdown(); // Hentikan countdown saat pintu dibuka kedua kali
            }
            intText.SetActive(false); // Sembunyikan teks "Press E"
            interactable = false;
            PlayTimeline(); // Play timeline pada interaksi kedua setelah reset
        }

        yield return null; // Tidak perlu menunggu penuh
    }

    IEnumerator CloseDoor()
    {
        // Biarkan fungsi ini tidak dipanggil oleh interaksi player
        doorAnim.ResetTrigger("open");
        doorAnim.SetTrigger("close");
        audioSource.PlayOneShot(closeSound);
        toggle = false;
        yield return null; // Tidak perlu menunggu penuh
    }

    public void ResetInteraction()
    {
        intText.SetActive(false); // Pastikan teks interaksi dimatikan
        interactable = false; // Reset flag interaksi
        hasOpenedOnce = false; // Izinkan interaksi kembali
        interactionReset = true; // Set interaction reset flag to true
    }

    void PlayTimeline()
    {
        // Switch camera to timeline camera
        mainCamera.gameObject.SetActive(false);
        timelineCamera.gameObject.SetActive(true);
        playerScript.enabled = false;
        audioscource.enabled = false;
        exhaustionText.SetActive(false);
        // Play the timeline
        timeline.Play();
    }

    void OnTimelineStopped(PlayableDirector director)
    {
        // When the timeline is finished, unlock and show the cursor
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Make the cursor visible

        // Then load the "MenuAwal" scene
        SceneManager.LoadScene("MenuAwal");
    }
}
