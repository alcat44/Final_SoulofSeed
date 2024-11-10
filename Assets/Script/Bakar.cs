using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bakar : MonoBehaviour
{
    public GameObject intText;
    public bool interactable;
    public GameObject objectInHand; // Objek yang dipegang player
    public GameObject newObject;    // Objek baru yang akan muncul
    public Transform spawnPoint;    // Lokasi tempat objek baru akan muncul
    public Animator doorAnim;
    public AudioSource audioSource;
    public AudioClip openSound;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(true);
            interactable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false);
            interactable = false;
        }
    }

    private void Update()
    {
        if (interactable)
        {
            if (Input.GetKeyDown(KeyCode.E)) // Mengganti 'B' dengan 'E' untuk interaksi
            {
                // Jika player memegang objek, hilangkan objek tersebut
                if (objectInHand != null)
                {
                    Destroy(objectInHand); // Hapus objek yang sedang dipegang
                }

                // Munculkan objek baru di spawnPoint
                Instantiate(newObject, spawnPoint.position, spawnPoint.rotation);

                // Jalankan animasi dan suara
                doorAnim.SetTrigger("bakar");
                audioSource.PlayOneShot(openSound);

                // Nonaktifkan teks interaksi
                intText.SetActive(false);
                interactable = false;
            }
        }
    }
}
