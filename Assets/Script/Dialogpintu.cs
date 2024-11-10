using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class Dialogpintu : MonoBehaviour
{
    public TextMeshProUGUI textComponent; // Text untuk dialog
    public GameObject intText; // GameObject untuk "Press E"
    public string[] lines; // Array untuk menampung dialog
    public float textSpeed;
    private int index, NumberDialog;
    public Image blackScreen; // Assign ini di Inspector dengan Image hitam di Canvas
    public float fadeDuration = 1f; // Durasi fade in dan fade out
    public bool interactable = false; // Untuk mengecek apakah pemain ada di dalam area trigger
    public bool isDialogActive = false; // Untuk mengecek apakah dialog sedang aktif

    public MonoBehaviour SC_FPSController; // Script player movement
    public Pause pauseScript;
    public PauseApart pauseScript2;
    public AudioSource audioSource;
    public GameObject DialogBG;
    public AudioSource doorSFX;
    public bool hasKey = false; // Tandai apakah pemain sudah memiliki kunci
    public bool hasTuru = false;
    private bool isTransitioning = false;

    // Gibberish sound related variables
    public AudioClip[] gibberishClips; // Array of gibberish sounds
    public AudioSource gibberishSource; // AudioSource for playing gibberish
    private bool isGibberishPlaying = false; // Flag to check if gibberish is playing

    void Start()
    {
        textComponent.text = string.Empty;
        intText.SetActive(false); // Nonaktifkan teks "Press E" pada awalnya
    }

    void Update()
    {
        if (hasTuru)
        {
            NumberDialog = 2; // Jika hasTuru true, dialog mulai dari index 1
        }
        else
        {
            NumberDialog = 0; // Jika hasTuru false, dialog mulai dari index 0
        }

        if (hasKey && interactable && !isTransitioning && Input.GetKeyDown(KeyCode.E))
        {
            // Set flag to true to prevent further interaction
            isTransitioning = true;

            // Disable further interactions and hide intText
            interactable = false;
            intText.SetActive(false);

            // Start the scene transition coroutine
            StartCoroutine(TransitionToScene("SampleScene"));
            return; // Return here to avoid executing dialog-related code during scene transition
        }

        // Logika dialog hanya berjalan jika scene transition tidak sedang terjadi
        if (!isTransitioning && interactable && !isDialogActive && Input.GetKeyDown(KeyCode.E))
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
                StopGibberish(); // Stop gibberish audio when skipping text
            }
        }
    }

    void StartDialog()
    {
        index = NumberDialog;
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

        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        // Mengetik tiap karakter satu per satu
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;

            // Play gibberish sound while typing text
            PlayGibberish();

            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index == NumberDialog)
        {
            index++;
            textComponent.text = string.Empty;
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

    // Function to play a random gibberish sound
    void PlayGibberish()
    {
        if (gibberishClips.Length > 0 && !isGibberishPlaying) // Ensure no gibberish is currently playing
        {
            // Pick a random gibberish clip
            AudioClip randomGibberish = gibberishClips[Random.Range(0, gibberishClips.Length)];
            gibberishSource.PlayOneShot(randomGibberish); // Play the sound
            isGibberishPlaying = true;

            // Reset the gibberish playing flag after the sound finishes
            StartCoroutine(ResetGibberishPlaying(randomGibberish.length));
        }
    }

    // Coroutine to reset the gibberish playing flag after the clip finishes
    IEnumerator ResetGibberishPlaying(float duration)
    {
        yield return new WaitForSeconds(duration);
        isGibberishPlaying = false;
    }

    // Function to stop the gibberish audio
    void StopGibberish()
    {
        gibberishSource.Stop(); // Stop the audio
        isGibberishPlaying = false; // Reset flag
    }

    // Ketika pemain berada di dalam area trigger pintu
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

    // Ketika pemain keluar dari area trigger pintu
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false); // Sembunyikan "Press E"
            interactable = false; // Pemain tidak bisa berinteraksi
        }
    }

    IEnumerator TransitionToScene(string sceneName)
    {
        if (doorSFX != null)
        {
            doorSFX.Play();
        }
        // Fade in layar hitam
        yield return StartCoroutine(FadeScreen(0f, 1f)); // Fade in (layar hitam muncul)
        
        // Tunggu 3 detik
        yield return new WaitForSeconds(4f);

        // Ganti scene
        SceneManager.LoadScene(sceneName);

        // Setelah scene berubah, fade out ke tampilan baru
        yield return StartCoroutine(FadeScreen(1f, 0f)); // Fade out (layar hitam menghilang)
    }

    IEnumerator FadeScreen(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = blackScreen.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            blackScreen.color = color;
            yield return null;
        }

        // Pastikan alpha tepat sesuai nilai akhir
        color.a = endAlpha;
        blackScreen.color = color;
    }
}
