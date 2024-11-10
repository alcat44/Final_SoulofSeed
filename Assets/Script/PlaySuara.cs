using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySuara : MonoBehaviour
{
    public GameObject intText2;          // Teks interaksi
    public AudioClip audioClip;          // AudioClip utama (misalnya Radio)
    public AudioClip turnOn;             // AudioClip yang diputar saat audio dihidupkan
    public AudioClip turnOff;            // AudioClip yang diputar saat audio dimatikan
    public AudioSource RadioSource;      // AudioSource untuk memutar AudioClip
    public AudioSource SFXSource;        // AudioSource untuk memutar turnOn dan turnOff

    private bool isInRange = false;      // Cek apakah MainCamera dalam jarak dengan object
    private bool isPlaying = false;      // Cek status apakah audio sedang dimainkan

    void Start()
    {
        // Pastikan intText2 nonaktif di awal
        if (intText2 != null)
        {
            intText2.SetActive(false);
        }

        // Tambahkan AudioSource ke object ini
        RadioSource.clip = audioClip;    // Set AudioClip untuk RadioSource
    }

    void Update()
    {
        // Ketika MainCamera dalam jarak (isInRange == true) dan tombol E ditekan
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleAudio();   // Panggil fungsi untuk toggle audio
        }

        // Cek jika audio utama selesai diputar
        if (!RadioSource.isPlaying && isPlaying)
        {
            // Jika audio utama selesai, mainkan turnOff dan set radio ke status mati
            SFXSource.PlayOneShot(turnOff);
            isPlaying = false;  // Radio sudah dalam kondisi mati
        }
    }

    // Fungsi untuk toggle AudioClip
    void ToggleAudio()
    {
        if (isPlaying)
        {
            // Mainkan suara turnOff dan kemudian matikan audio
            SFXSource.PlayOneShot(turnOff);
            RadioSource.Stop();      // Hentikan audio utama
            isPlaying = false;       // Set status ke false
        }
        else
        {
            // Mainkan suara turnOn dan kemudian hidupkan audio utama
            SFXSource.PlayOneShot(turnOn);
            RadioSource.PlayDelayed(turnOn.length); // Tunggu sampai turnOn selesai sebelum mainkan audio utama
            isPlaying = true;        // Set status ke true
        }
    }

    // Ketika MainCamera masuk ke dalam area trigger (Collider object)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            isInRange = true;   // Set bahwa player berada dalam jarak

            // Tampilkan teks interaksi jika intText2 ada
            if (intText2 != null)
            {
                intText2.SetActive(true);
            }
        }
    }

    // Ketika MainCamera keluar dari area trigger (Collider object)
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            isInRange = false;  // Set bahwa player berada di luar jarak

            // Sembunyikan teks interaksi jika intText2 ada
            if (intText2 != null)
            {
                intText2.SetActive(false);
            }
        }
    }
}
