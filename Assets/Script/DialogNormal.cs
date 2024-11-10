using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogNormal : MonoBehaviour
{
    public TextMeshProUGUI textComponent; // Text untuk dialog
    public GameObject intText; // GameObject untuk "Press E"
    public string[] lines; // Array untuk menampung dialog
    public float textSpeed; // Kecepatan teks
    private int index;
    public bool interactable = false; // Untuk mengecek apakah pemain ada di dalam area trigger
    private bool isDialogActive = false; // Untuk mengecek apakah dialog sedang aktif

    public MonoBehaviour SC_FPSController;
    public Pause pauseScript;
    public PauseApart pauseScript2;
    public AudioSource audioSource;
    public GameObject DialogBG;

    public AudioClip[] SpeakNoises = new AudioClip[0]; // Array suara gibberish
    public AudioSource gibberishSource; // AudioSource untuk gibberish

    public float gibberishPitch = 1.0f; // Kecepatan playback untuk audio gibberish (1.0 = normal speed)

    private Coroutine gibberishCoroutine; // Coroutine untuk gibberish
    private bool lineFinished = false; // Flag untuk mengecek apakah line sudah selesai
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
            if (lineFinished)
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index]; // Tampilkan teks lengkap
                StopGibberish(); // Stop audio gibberish
                lineFinished = true; // Tandai line sebagai selesai
            }
        }
    }

    void StartDialog()
    {
        index = 0;
        isDialogActive = true; // Tandai bahwa dialog aktif
        SC_FPSController.enabled = false; // Matikan script player movement
        audioSource.enabled = false;
        intText.SetActive(false);
        DialogBG.SetActive(true);

        // Nonaktifkan pauseScript jika tidak null
        if (pauseScript != null)
        {
            pauseScript.enabled = false;
        }

        // Nonaktifkan pauseScript2 jika tidak null
        if (pauseScript2 != null)
        {
            pauseScript2.enabled = false;
        }

        lineFinished = false; // Reset flag
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        lineFinished = false; // Reset flag
        textComponent.text = ""; // Kosongkan teks sebelumnya

        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;

            // Memutar suara gibberish secara acak dari array SpeakNoises
            PlayRandomGibberish();

            yield return new WaitForSeconds(textSpeed); // Tunggu sebelum karakter berikutnya muncul
        }

        // Setelah selesai, tandai line sebagai selesai
        lineFinished = true;
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialog();
        }
    }

    void EndDialog()
    {
        isDialogActive = false; // Tandai bahwa dialog sudah selesai
        textComponent.text = string.Empty; // Kosongkan teks setelah dialog selesai
        SC_FPSController.enabled = true; // Aktifkan kembali script player movement
        audioSource.enabled = true;

        // Aktifkan kembali pauseScript jika tidak null
        if (pauseScript != null)
        {
            pauseScript.enabled = true;
        }

        // Aktifkan kembali pauseScript2 jika tidak null
        if (pauseScript2 != null)
        {
            pauseScript2.enabled = true;
        }

        DialogBG.SetActive(false);
    }

    // Fungsi untuk memutar suara gibberish secara acak
    void PlayRandomGibberish()
    {
        if (SpeakNoises.Length > 0 && !isGibberishPlaying) // Cek jika suara tidak sedang diputar
        {
            // Pilih suara secara acak
            AudioClip randomClip = SpeakNoises[Random.Range(0, SpeakNoises.Length)];
            gibberishSource.pitch = gibberishPitch; // Set kecepatan playback
            gibberishSource.PlayOneShot(randomClip); // Mainkan suara
            isGibberishPlaying = true; // Tandai bahwa suara sedang diputar

            // Hentikan mark ketika clip selesai
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
