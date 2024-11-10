using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogMasuk : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    public GameObject Dialog;
    public GameObject DialogBG;
    public GameObject player;  // Reference ke Player GameObject
    public GameObject pauseMenuObject;  // Reference ke GameObject Pause
    public AudioSource audioSource;
    
    // Untuk gibberish sound
    public AudioClip[] SpeakNoises = new AudioClip[0]; // Array suara gibberish
    public AudioSource gibberishSource; // AudioSource untuk gibberish
    public float gibberishPitch = 1.0f; // Kecepatan playback untuk audio gibberish (1.0 = normal speed)
    private Coroutine gibberishCoroutine; // Coroutine untuk gibberish
    private bool isGibberishPlaying = false; // Flag untuk mengecek apakah gibberish sedang diputar

    public bool isDialogueActive = false;  // Public flag untuk pengecekan status dialog
    private bool canPressE = false;        // Flag untuk pengecekan apakah tombol E dapat ditekan

    private int index;
    private SC_FPSController scFpsController;
    private Pause pauseScript;
    private PauseApart pauseScript2;

    void Start()
    {
        scFpsController = player.GetComponent<SC_FPSController>();  // Ambil komponen SC_FPSController dari player
        pauseScript = pauseMenuObject.GetComponent<Pause>();  // Ambil komponen Pause dari pauseMenuObject
        pauseScript2 = pauseMenuObject.GetComponent<PauseApart>();

        textComponent.text = string.Empty;
        DialogBG.SetActive(false);  // Pastikan dialog background tidak aktif di awal
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("MainCamera")) && !isDialogueActive)
        {
            StartDialogue();
            isDialogueActive = true;
        }
    }

    void Update()
    {
        if (isDialogueActive && canPressE && Input.GetKeyDown(KeyCode.E)) // Hanya bisa menekan E jika flag canPressE true
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
                StopGibberish(); // Stop gibberish ketika dialog di-skip
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        canPressE = false; // Tombol E tidak bisa ditekan selama penundaan
        StartCoroutine(StartDialogueAfterDelay()); // Memulai dialog setelah penundaan 1 detik
    }

    IEnumerator StartDialogueAfterDelay()
    {
        if (scFpsController != null) scFpsController.enabled = false;  // Nonaktifkan kontrol player
        if (pauseScript != null) pauseScript.enabled = false;          // Nonaktifkan pause menu
        if (pauseScript2 != null) pauseScript2.enabled = false;
        if (audioSource != null) audioSource.enabled = false;          // Nonaktifkan audio source jika ada
        
        yield return new WaitForSeconds(1f); // Menunggu selama 1 detik

        StartCoroutine(TypeLine()); // Memulai pengetikan dialog setelah penundaan
        DialogBG.SetActive(true);   // Tampilkan background dialog
        Dialog.SetActive(true);     // Tampilkan dialog object
        
        canPressE = true; // Setelah 1 detik, tombol E baru bisa ditekan
    }

    IEnumerator TypeLine()
    {
        textComponent.text = string.Empty;
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            
            // Memutar suara gibberish secara acak dari array SpeakNoises
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
        textComponent.text = string.Empty;
        DialogBG.SetActive(false);  // Sembunyikan background dialog
        Dialog.SetActive(false);    // Sembunyikan dialog object
        isDialogueActive = false;   // Reset flag setelah dialog selesai
        canPressE = false;          // Setelah dialog selesai, set tombol E tidak bisa ditekan

        if (scFpsController != null) scFpsController.enabled = true;  // Aktifkan kontrol player
        if (pauseScript != null) pauseScript.enabled = true;          // Aktifkan pause menu
        if (pauseScript2 != null) pauseScript2.enabled = true; 
        if (audioSource != null) audioSource.enabled = true;          // Aktifkan audio source jika ada
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
}
