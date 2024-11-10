using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PintuGembok2 : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    public GameObject intText;
    public GameObject DialogBG;
    public MonoBehaviour SC_FPSController;
    public Pause pauseScript;
    public AudioSource audioSource;
    public AudioSource audioSource2;
    public AudioClip openSound;
    public AudioClip lockSound;
    public Collider doorCollider; // Collider pintu
    public Collider doorCollider2; // Collider pintu
    public GameObject Kunci2;

    // Reference to the PickUpController script
    public PickUpController pickUpController;

    // Gibberish variables
    public AudioClip[] SpeakNoises = new AudioClip[0]; // Array suara gibberish
    public AudioSource gibberishSource; // AudioSource untuk gibberish
    public float gibberishPitch = 1.0f; // Kecepatan playback untuk audio gibberish (1.0 = normal speed)
    private bool isGibberishPlaying = false; // Flag untuk mengecek apakah gibberish sedang diputar
    private Coroutine gibberishCoroutine; // Coroutine untuk gibberish

    private int index, NumberDialog;
    private bool isDialogueActive = false;
    public bool interactable = false;
    public bool hasKeyTouched = false; // Flag jika kunci sudah menyentuh pintu

    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = string.Empty;
        intText.SetActive(false); // Nonaktifkan teks interaksi di awal
    }

    // Update is called once per frame
    void Update()
    {
        // Ubah nomor dialog jika kunci sudah menyentuh doorCollider
        if (hasKeyTouched)
        {
            NumberDialog = 2; // Dialog akan dimulai dari index 2 jika kunci menyentuh
        }
        // Check if the object is picked using the isPicked bool from PickUpController
        else if (pickUpController != null && pickUpController.isPicked)
        {
            NumberDialog = 4; // Set dialog to 4 if isPicked is true
        }
        else
        {
            NumberDialog = 0; // Jika hasTuru false, dialog mulai dari index 0
        }

        // Jika interaksi diizinkan dan pemain menekan E
        if (interactable && !isDialogueActive && Input.GetKeyDown(KeyCode.E))
        {
            StartDialog();
        }

        // Saat dialog aktif dan pemain menekan E untuk lanjut
        if (isDialogueActive && Input.GetKeyDown(KeyCode.E))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
                StopGibberish(); // Stop gibberish immediately when skipping
            }
        }
    }

    void StartDialog()
    {
        index = NumberDialog;
        isDialogueActive = true; // Tandai dialog sedang aktif
        SC_FPSController.enabled = false; // Nonaktifkan movement player
        if (pauseScript != null) pauseScript.enabled = false;
        audioSource.enabled = false;
        intText.SetActive(false); // Matikan intText saat dialog aktif
        DialogBG.SetActive(true);

        // Jika dialog dimulai dari NumberDialog == 0, putar suara lockSound
        if (NumberDialog == 0)
        {
            audioSource2.PlayOneShot(lockSound);
        }

        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        textComponent.text = string.Empty; // Kosongkan teks
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;

            // Play gibberish sound while typing
            PlayRandomGibberish();

            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index == NumberDialog) // Lanjutkan ke dialog berikutnya jika masih ada
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
        isDialogueActive = false; // Tandai dialog selesai
        textComponent.text = string.Empty;
        SC_FPSController.enabled = true; // Aktifkan kembali kontrol pemain
        if (pauseScript != null) pauseScript.enabled = true;
        audioSource.enabled = true;
        DialogBG.SetActive(false); // Sembunyikan background dialog
        StopGibberish(); // Stop any gibberish audio at the end
        // Setelah dialog selesai, jika NumberDialog == 2, hancurkan objek ini
        if (NumberDialog == 2)
        {
            gameObject.SetActive(false); // Hancurkan objek setelah dialog yang dimulai dari NumberDialog == 2 selesai
        }
    }

    // Gibberish functions
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

    // Deteksi jika doorCollider bersentuhan dengan objek bertag "Kunci"
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Kunci2"))
        {
            hasKeyTouched = true; // Kunci menyentuh doorCollider
            Destroy(Kunci2); // Hancurkan objek Kunci
            doorCollider.enabled = true; // Aktifkan collider pintu
            doorCollider2.enabled = true; // Aktifkan collider pintu
            audioSource2.PlayOneShot(openSound);
        }
    }

    // Deteksi saat pemain mendekati pintu
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            if (!isDialogueActive)
            {
                intText.SetActive(true); // Tampilkan teks interaksi
                interactable = true; // Pemain bisa berinteraksi
            }
        }
    }

    // Deteksi saat pemain menjauh dari pintu
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false); // Matikan teks interaksi
            interactable = false; // Pemain tidak bisa berinteraksi
        }
    }

    public void ResetDialog()
    {
        textComponent.text = string.Empty; // Clear any active dialog text
        intText.SetActive(false); // Hide interaction prompt
        interactable = false; // Reset interaction flag
        isDialogueActive = false; // Ensure dialog is not active
        DialogBG.SetActive(false); // Hide dialog background
        StopGibberish(); // Stop any gibberish audio
    }
}

