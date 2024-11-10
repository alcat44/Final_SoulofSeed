using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideText : MonoBehaviour
{
    public GameObject intText2;  // Teks interaksi
    public Collider door;        // Collider pintu pertama
    public Collider door2;       // Collider pintu kedua
    public MonoBehaviour doorScript1; // MonoBehaviour script for door 1
    public MonoBehaviour doorScript2; // MonoBehaviour script for door 2

    void Awake()
    {
        // Nonaktifkan intText2 di awal untuk memastikan tidak muncul secara acak
        if (intText2 != null)
        {
            intText2.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {

            if (door != null)
            {
                door.enabled = false;  // Nonaktifkan collider pintu pertama
                if (doorScript1 != null)
                {
                    doorScript1.enabled = false; // Disable the MonoBehaviour script for door 1
                    // Call ResetInteraction if the door script has that function
                    Door doorScript = doorScript1 as Door;
                    if (doorScript != null)
                    {
                        doorScript.ResetInteraction();
                    }
                }
            }

            if (door2 != null)
            {
                door2.enabled = false; // Nonaktifkan collider pintu kedua
                if (doorScript2 != null)
                {
                    doorScript2.enabled = false; // Disable the MonoBehaviour script for door 2
                    // Call ResetInteraction if the door script has that function
                    Door doorScript = doorScript2 as Door;
                    if (doorScript != null)
                    {
                        doorScript.ResetInteraction();
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            // Aktifkan kembali collider pintu dan MonoBehaviour script ketika player keluar dari area
            if (door != null)
            {
                door.enabled = true; // Aktifkan kembali collider pintu pertama
                if (doorScript1 != null)
                {
                    doorScript1.enabled = true; // Enable the MonoBehaviour script for door 1
                }
            }

            if (door2 != null)
            {
                door2.enabled = true; // Aktifkan kembali collider pintu kedua
                if (doorScript2 != null)
                {
                    doorScript2.enabled = true; // Enable the MonoBehaviour script for door 2
                }
            }
        }
    }
}
