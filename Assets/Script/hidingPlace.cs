using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hidingPlace : MonoBehaviour
{
    public GameObject hideText, stopHideText;
    public GameObject normalPlayer, hidingPlayer;
    public enemyAI1 monsterScript;
    public Transform monsterTransform;
    public GameObject kakek; // GameObject for "kakek"
    bool interactable, hiding;
    public float loseDistance;

    // Tambahan untuk SFX
    public AudioSource audioSource; // AudioSource untuk memutar SFX
    public AudioClip hideSFX; // SFX saat player hide
    public AudioClip unhideSFX; // SFX saat player unhide

    // Tambahan untuk delay
    private bool canExitHiding = false; // Menentukan apakah player sudah bisa keluar dari hiding
    
    // Cache SC_FPSController dynamically
    private SC_FPSController playerScript;

    void Start()
    {
        interactable = false;
        hiding = false;

        // Dynamically find the SC_FPSController on the normal player
        playerScript = normalPlayer.GetComponent<SC_FPSController>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera") && !hiding) // Hanya tampilkan jika tidak sedang hiding
        {
            hideText.SetActive(true);
            interactable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            hideText.SetActive(false);
            interactable = false;
        }
    }

    void Update()
    {
        if (interactable && !hiding) // Hanya bisa hide jika tidak sedang hiding
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                hideText.SetActive(false); // Pastikan hideText dimatikan
                hidingPlayer.SetActive(true);
                float distance = Vector3.Distance(monsterTransform.position, normalPlayer.transform.position);
                if (distance > loseDistance)
                {
                    if (monsterScript.chasing)
                    {
                        monsterScript.stopChase();
                    }
                }
                stopHideText.SetActive(true);
                hiding = true;
                normalPlayer.SetActive(false);
                interactable = false;

                // Mainkan SFX hide
                if (audioSource != null && hideSFX != null)
                {
                    audioSource.PlayOneShot(hideSFX);
                }

                // Reset exhaustion text during hiding, if player is not exhausted
                if (playerScript != null && playerScript.exhaustionText != null)
                {
                    playerScript.exhaustionText.SetActive(false);
                }

                // Activate "kakek" if it's assigned
                if (kakek != null)
                {
                    kakek.SetActive(true);
                }

                // Mulai coroutine untuk delay sebelum bisa keluar dari hiding
                StartCoroutine(AllowExitAfterDelay(1f)); // 1 detik delay
            }
        }

        if (hiding && canExitHiding) // Cek jika player sedang hiding dan bisa keluar
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                stopHideText.SetActive(false);
                normalPlayer.SetActive(true);
                hidingPlayer.SetActive(false);
                hiding = false;
                canExitHiding = false; // Reset agar tidak bisa keluar sebelum delay saat hiding berikutnya

                // Mainkan SFX unhide
                if (audioSource != null && unhideSFX != null)
                {
                    audioSource.PlayOneShot(unhideSFX);
                }

                // Handle exhaustion state after exiting hiding
                if (playerScript != null)
                {
                    playerScript.ResetPlayerMovement();
                }

                // Deactivate "kakek" if it's assigned
                if (kakek != null)
                {
                    kakek.SetActive(false);
                }
            }
        }
    }

    IEnumerator AllowExitAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canExitHiding = true; // Setelah delay, player bisa keluar dari hiding
    }

    public void ResetHiding()
    {
        // Reset flags
        interactable = false;
        hiding = false;
        canExitHiding = false; // Reset agar tidak bisa keluar jika hiding direset

        // Ensure player exits hiding mode
        normalPlayer.SetActive(true);
        hidingPlayer.SetActive(false);

        // Disable all UI texts related to hiding
        hideText.SetActive(false);
        stopHideText.SetActive(false);

        // Deactivate "kakek" if it's assigned
        if (kakek != null)
        {
            kakek.SetActive(false);
        }
    }
}
