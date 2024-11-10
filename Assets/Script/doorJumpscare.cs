using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorJumpscare : MonoBehaviour
{
    public GameObject intText;
    public bool interactable, toggle;
    public Animator doorAnim;
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;
    
    public Animator otherObjectAnim; // Animator untuk objek lain
    public AudioSource otherObjectAudioSource; // AudioSource untuk objek lain
    public AudioClip otherObjectSound; // Efek suara untuk objek lain
    public string animationTriggerName; // Nama trigger animasi untuk objek lain

    private bool hasOpenedOnce = false; // Menandakan apakah pintu sudah dibuka sekali

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("MainCamera"))
        {
            intText.SetActive(true);
            interactable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("MainCamera"))
        {
            intText.SetActive(false);
            interactable = false;
        }
    }

    void Update()
    {
        if(interactable)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                toggle = !toggle;
                if(toggle)
                {
                    doorAnim.ResetTrigger("close");
                    doorAnim.SetTrigger("open");
                    audioSource.PlayOneShot(openSound);

                    // Memicu animasi objek lain dan memainkan efek suara jika pintu terbuka untuk pertama kali
                    if (!hasOpenedOnce)
                    {
                        if (otherObjectAnim != null)
                        {
                            otherObjectAnim.SetTrigger(animationTriggerName);
                            if (otherObjectAudioSource != null && otherObjectSound != null)
                            {
                                otherObjectAudioSource.PlayOneShot(otherObjectSound);
                            }
                        }
                        hasOpenedOnce = true;
                    }
                }
                else
                {
                    doorAnim.ResetTrigger("open");
                    doorAnim.SetTrigger("close");
                    audioSource.PlayOneShot(closeSound);
                }
                intText.SetActive(false);
                interactable = false;
            }
        }
    }
}
