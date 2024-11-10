using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogKunci : MonoBehaviour
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
    public PauseApart pauseScript2;
    public MonoBehaviour door; // Tambahkan referensi untuk script player movement
    public AudioSource audioSource;
    public GameObject DialogBG;
    public GameObject Kunci;

    public Dialogpintu dialogPintu; // Referensi ke script Dialogpintu untuk memberi tahu pintu bahwa kunci diambil

    // Penambahan untuk Gibberish Sound
    public AudioClip[] SpeakNoises = new AudioClip[0]; // Array suara gibberish
    public AudioClip kunciClip; // Tambahkan AudioClip untuk suara kunci
    public AudioSource gibberishSource; // AudioSource untuk gibberish
    public float gibberishPitch = 1.0f; // Kecepatan playback untuk audio gibberish (1.0 = normal speed)
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
                StopGibberish(); // Hentikan suara gibberish saat skip teks
            }
        }
    }

    void StartDialog()
    {
        index = 0;
        isDialogActive = true; // Tandai bahwa dialog aktif
        if (SC_FPSController != null)
        {
            SC_FPSController.enabled = false; // Matikan script player movement
        }
        
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
        if (audioSource != null)
        {
            audioSource.enabled = false;
        }
        intText.SetActive(false);
        DialogBG.SetActive(true);
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        // Mengetik tiap karakter satu per satu
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;

            // Mainkan suara gibberish saat karakter diketik
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
            EndDialog(); // Move to a separate method to end the dialog
        }
    }

    void EndDialog()
    {
        isDialogActive = false; // Tandai bahwa dialog sudah selesai
        textComponent.text = string.Empty; // Kosongkan teks setelah dialog selesai
        if (SC_FPSController != null)
        {
            SC_FPSController.enabled = true; // Aktifkan kembali script player movement
        }
        
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
        if (audioSource != null)
        {
            audioSource.enabled = true;
        }
        DialogBG.SetActive(false);
        
        // Kunci diambil, mainkan suara kunci
        if (kunciClip != null && gibberishSource != null)
        {
            gibberishSource.PlayOneShot(kunciClip); // Mainkan suara kunci
        }

        dialogPintu.hasKey = true;
        Destroy(Kunci); // Hancurkan kunci
        if (door != null)
        {
            door.enabled = true; // Aktifkan pintu
        }
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
        if (gibberishSource != null)
        {
            gibberishSource.Stop(); // Hentikan audio
        }
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
                if (door != null)
                {
                    door.enabled = false; // Disable door interaction when in trigger
                }
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
            if (door != null)
            {
                door.enabled = true; // Enable door interaction when out of trigger
            }
        }
    }
}
