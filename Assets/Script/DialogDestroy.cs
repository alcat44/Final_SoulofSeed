using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogDestroy : MonoBehaviour
{
    public TextMeshProUGUI textComponent; // Text untuk dialog
    public GameObject intText; // GameObject untuk "Press E"
    public string[] lines; // Array untuk menampung dialog
    public float textSpeed;
    private int index;
    public bool interactable = false; // Untuk mengecek apakah pemain ada di dalam area trigger
    private bool isDialogActive = false; // Untuk mengecek apakah dialog sedang aktif

    public MonoBehaviour SC_FPSController;
    public Pause pauseScript;
    public MonoBehaviour door; // Tambahkan referensi untuk script player movement
    public AudioSource audioSource;
    public AudioSource audioSource2;
    public AudioClip LockedSound;
    public GameObject DialogBG;
    public GameObject trigger;

    // Gibberish Sound Properties
    public AudioClip[] SpeakNoises = new AudioClip[0]; // Array suara gibberish
    public AudioSource gibberishSource; // AudioSource untuk gibberish
    public float gibberishPitch = 1.0f; // Kecepatan playback untuk audio gibberish (1.0 = normal speed)
    private Coroutine gibberishCoroutine; // Coroutine untuk gibberish
    private bool isGibberishPlaying = false; // Flag untuk mengecek apakah gibberish sedang diputar

    void Start()
    {
        textComponent.text = string.Empty;
        intText.SetActive(false); // Nonaktifkan teks "Press E" pada awalnya
    }

    void Update()
    {
        // Hanya memungkinkan interaksi jika pemain berada di area trigger dan dialog belum aktif
        if (interactable && !isDialogActive && Input.GetKeyDown(KeyCode.E))
        {
            StartDialog();
        }

        // Saat dialog aktif, pemain dapat menekan E untuk melanjutkan ke baris berikutnya
        if (isDialogActive && Input.GetKeyDown(KeyCode.E))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
                StopGibberish(); // Stop gibberish saat dialog di-skip
            }
        }
    }

    void StartDialog()
    {
        index = 0;
        isDialogActive = true; // Tandai bahwa dialog aktif
        SC_FPSController.enabled = false; // Matikan script player movement
        if (pauseScript != null) pauseScript.enabled = false; 
        if (audioSource != null) audioSource.enabled = false; // Cek null untuk audioSource
        intText.SetActive(false);
        if (audioSource2 != null && LockedSound != null) // Cek null untuk audioSource2 dan LockedSound
        {
            audioSource2.PlayOneShot(LockedSound);
        }
        DialogBG.SetActive(true);
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        // Mengetik tiap karakter satu per satu
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;

            // Play gibberish sound when typing each character
            PlayRandomGibberish();

            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        isDialogActive = false; // Tandai bahwa dialog sudah selesai
        textComponent.text = string.Empty; // Kosongkan teks setelah dialog selesai
        SC_FPSController.enabled = true; // Aktifkan kembali script player movement
        if (pauseScript != null) pauseScript.enabled = true;
        if (audioSource != null) audioSource.enabled = true; // Cek null untuk audioSource
        DialogBG.SetActive(false);
        Destroy(trigger);
    }

    // Functions for Gibberish Sound

    // Play a random gibberish sound
    void PlayRandomGibberish()
    {
        if (SpeakNoises.Length > 0 && !isGibberishPlaying) // Cek jika suara tidak sedang diputar
        {
            // Pilih suara secara acak
            AudioClip randomClip = SpeakNoises[Random.Range(0, SpeakNoises.Length)];
            gibberishSource.pitch = gibberishPitch; // Set kecepatan playback
            gibberishSource.PlayOneShot(randomClip); // Mainkan suara
            isGibberishPlaying = true; // Tandai bahwa suara sedang diputar

            // Hentikan flag ketika clip selesai
            StartCoroutine(ResetGibberishPlaying(randomClip.length));
        }
    }

    // Coroutine untuk mereset flag isGibberishPlaying setelah clip selesai
    private IEnumerator ResetGibberishPlaying(float duration)
    {
        yield return new WaitForSeconds(duration);
        isGibberishPlaying = false; // Set flag menjadi false setelah audio selesai
    }

    // Fungsi untuk menghentikan audio gibberish
    void StopGibberish()
    {
        gibberishSource.Stop(); // Hentikan audio
        isGibberishPlaying = false; // Reset flag
    }

    // Ketika pemain berada di dalam area trigger
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            // Tampilkan "Press E" hanya jika dialog belum aktif
            if (!isDialogActive)
            {
                intText.SetActive(true);
                interactable = true;
                if (door != null) door.enabled = false; // Cek null untuk door
            }
        }                                                                           
    }

    // Ketika pemain keluar dari area trigger
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false); // Sembunyikan "Press E"
            interactable = false; // Pemain tidak bisa berinteraksi
            if (door != null) door.enabled = true; // Cek null untuk door
        }
    }

    public void ResetDialog()
    {
        // Reset dialog UI and interaction states
        textComponent.text = string.Empty; // Clear any active dialog text
        intText.SetActive(false); // Hide interaction prompt
        interactable = false; // Reset interaction flag
        isDialogActive = false; // Ensure dialog is not active
        DialogBG.SetActive(false); // Hide dialog background
        StopGibberish(); // Stop any gibberish audio
    }
}
