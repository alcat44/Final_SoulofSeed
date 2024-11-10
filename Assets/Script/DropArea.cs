using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.UI; 

public class DropArea : MonoBehaviour
{
    public GameObject objectToSpawn; // Objek yang akan muncul di area trigger
    public float spawnDelay = 1f; // Waktu tunda sebelum objek muncul
    public float activeDuration = 10f; // Durasi objek aktif sebelum dinonaktifkan

    public TextMeshProUGUI textComponent; // Komponen teks untuk dialog
    public string[] lines; // Baris-baris teks dialog
    public float textSpeed; // Kecepatan pengetikan teks
    public GameObject DialogBG; // Background dialog
    public MonoBehaviour SC_FPSController; // Kontrol player
    public SC_FPSController playerScript;
    public Pause pauseScript;
    public PintuLocked pintuLockedScript;
    public DoorUtama doorScript;
    public AudioSource audioSource;

    public PlayableDirector timeline; // Timeline yang akan dipanggil setelah 2 trigger
    public GameObject player; // Player object yang akan disembunyikan selama timeline berjalan
    public Camera playerCamera; // Kamera pemain
    public Camera cutsceneCamera; // Kamera cutscene

    private int index, NumberDialog; // Index baris dialog saat ini
    private bool isDialogueActive = false; // Apakah dialog sedang aktif
    private int triggerCount = 0; // Variabel untuk menghitung jumlah trigger
    private bool hasObjectSpawned = false; // Flag untuk memastikan objek hanya muncul sekali

    public GameObject intText; // UI "Press E" untuk interaksi
    public bool interactable; // Apakah pemain dapat berinteraksi
    public Collider Object;
    
    public GameObject objectToDisable1;
    public GameObject objectToDisable2;
    public GameObject bapakDisable;
    public GameObject bapakAble;
    public GameObject explosion;

    public TextMeshProUGUI countdownText; // Tambahkan referensi untuk countdown text
    public GameOverManager gameOverManager; // Referensi untuk GameOverManager script

    public Image fadeImage; // Tambahkan referensi untuk Image di Canvas yang akan digunakan untuk fade in
    public float fadeDuration = 1f; // Durasi animasi fade in

    private Coroutine countdownCoroutine; // Tambahkan ini untuk menyimpan referensi ke coroutine

    public AudioClip destroySound;
    public AudioClip countdownClip; // Tambahkan AudioClip untuk countdown
    public AudioSource audioSourceKarpet;
    public AudioSource audioCountdown;

    public AudioClip[] SpeakNoises = new AudioClip[0]; // Array suara gibberish
    public AudioSource gibberishSource; // AudioSource untuk gibberish
    public float gibberishPitch = 1.0f; // Kecepatan playback untuk audio gibberish (1.0 = normal speed)
    private bool isGibberishPlaying = false; // Flag untuk mengecek apakah gibberish sedang diputar

    
    private void Start()
    {
        // Pastikan objek yang akan muncul dimulai dalam keadaan tidak aktif
        if (objectToSpawn != null)
        {
            objectToSpawn.SetActive(false);
        }

        if (timeline != null)
        {
            timeline.gameObject.SetActive(false); // Pastikan timeline dimulai dalam keadaan tidak aktif
            timeline.stopped += OnTimelineStopped; // Berlangganan event ketika timeline berhenti
        }

        // Pastikan kamera cutscene dimulai dalam keadaan tidak aktif
        if (cutsceneCamera != null)
        {
            cutsceneCamera.gameObject.SetActive(false);
        }

        textComponent.text = string.Empty;
        DialogBG.SetActive(false); // Nonaktifkan background dialog di awal
        intText.SetActive(false); // Nonaktifkan teks "Press E" di awal

        playerScript = SC_FPSController as SC_FPSController;
    }

    void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("MainCamera"))
        {
            if (!isDialogueActive)
            {
                intText.SetActive(true);
                interactable = true;
            }
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false);
            interactable = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickupable") || other.CompareTag("Pickupable2"))
        {
            triggerCount++; // Tambahkan jumlah trigger setiap kali objek mengenai trigger

            if (other.CompareTag("Pickupable"))
            {
                NumberDialog = 0; // Set NumberDialog untuk Pickupable
            }
            else if (other.CompareTag("Pickupable2"))
            {
                NumberDialog = 3; // Set NumberDialog untuk Pickupable2
            }

            if (!hasObjectSpawned)
            {
                hasObjectSpawned = true; // Set flag agar objek tidak muncul lagi
                StartCoroutine(ActivateAndDeactivateObject());
            }

            if (destroySound != null && audioSourceKarpet != null)
            {
                audioSourceKarpet.PlayOneShot(destroySound);
            }


            Destroy(other.gameObject); // Hapus objek yang dibuang

            if (triggerCount >= 2 && timeline != null)
            {
                // Aktifkan timeline
                timeline.gameObject.SetActive(true);
                bapakDisable.SetActive(false);
                intText.SetActive(false);
                player.SetActive(false);

                // Reset exhaustion text during hiding, if player is not exhausted
                if (playerScript != null && playerScript.exhaustionText != null)
                {
                    playerScript.exhaustionText.SetActive(false);
                }

                timeline.Play();

                // Beralih ke kamera cutscene
                SwitchToCutsceneCamera();
            }
            else if (triggerCount == 1) // Mulai dialog jika triggerCount == 1
            {
                StartDialog(); // Mulai dialog dari NumberDialog yang sudah ditentukan
            }
        }
    }


    private void Update()
    {
        // Menangani input untuk melanjutkan dialog
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
            }
        }

        if (enemyAI1.isChasing)
        {
            Object.enabled = false; // Nonaktifkan collider
        }
        else
        {
            Object.enabled = true; // Aktifkan collider
        }

    }


    void StartDialog()
    {
        index = NumberDialog; // Mulai dialog dari nilai NumberDialog
        isDialogueActive = true; // Tandai dialog sedang aktif
        SC_FPSController.enabled = false; // Nonaktifkan movement player
        if (pauseScript != null) pauseScript.enabled = false; 
        audioSource.enabled = false;
        intText.SetActive(false); // Sembunyikan teks "Press E"
        DialogBG.SetActive(true); // Tampilkan background dialog

        StartCoroutine(TypeLine()); // Mulai dialog pertama
    }

    void NextLine()
    {
        if (index < lines.Length - 1 && index < NumberDialog + 2) 
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            StopGibberish(); // Stop audio gibberish ketika dialog selesai
            EndDialog();
        }
    }

    
    IEnumerator TypeLine()
    {
        textComponent.text = ""; // Kosongkan teks sebelumnya

        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;

            // Memutar suara gibberish secara acak dari array SpeakNoises
            PlayRandomGibberish();

            yield return new WaitForSeconds(textSpeed); // Tunggu sebelum karakter berikutnya muncul
        }

        // Setelah selesai, tandai line sebagai selesai
    }

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

    private IEnumerator ResetGibberishPlaying(float duration)
    {
        yield return new WaitForSeconds(duration);
        isGibberishPlaying = false; // Set flag menjadi false setelah audio selesai
    }

    void StopGibberish()
    {
        gibberishSource.Stop(); // Hentikan audio
        isGibberishPlaying = false; // Reset flag
    }



    void EndDialog()
    {
        isDialogueActive = false; // Tandai dialog selesai
        textComponent.text = string.Empty;
        SC_FPSController.enabled = true; // Aktifkan kembali kontrol pemain
        if (pauseScript != null) pauseScript.enabled = true;
        audioSource.enabled = true;
        DialogBG.SetActive(false); // Sembunyikan background dialog

        // Kembalikan kamera ke player jika ada timeline
        if (timeline != null)
        {
            SwitchToPlayerCamera();
        }
    }

    private IEnumerator ActivateAndDeactivateObject()
    {
        // Tunggu beberapa detik sebelum objek muncul
        yield return new WaitForSeconds(spawnDelay);

        // Aktifkan objek
        if (objectToSpawn != null)
        {
            objectToSpawn.SetActive(true);
            // Set objek muncul di posisi area trigger
            objectToSpawn.transform.position = transform.position;
            objectToSpawn.transform.rotation = transform.rotation;

            // Tunggu beberapa detik sebelum menonaktifkan objek
            yield return new WaitForSeconds(activeDuration);

            // Nonaktifkan objek
            objectToSpawn.SetActive(false);

            // Reset flag untuk memungkinkan penggunaan kembali jika diperlukan
            hasObjectSpawned = false;
        }
    }

    // Modifikasi fungsi OnTimelineStopped
    private void OnTimelineStopped(PlayableDirector director)
    {
        // Kembalikan ke kamera player
        SwitchToPlayerCamera();

        // Nonaktifkan objek yang perlu dinonaktifkan
        if (objectToDisable1 != null)
        {
            objectToDisable1.SetActive(false);
        }

        if (objectToDisable2 != null)
        {
            objectToDisable2.SetActive(false);
        }

        // Setelah timeline selesai, nonaktifkan Gembok dan aktifkan kembali collider pintu
        if (pintuLockedScript != null)
        {
            pintuLockedScript.Gembok.SetActive(false);  // Nonaktifkan Gembok
            pintuLockedScript.doorCollider.enabled = true;  // Aktifkan doorCollider 1
            pintuLockedScript.doorCollider2.enabled = true;  // Aktifkan doorCollider 2
        }

        // Memastikan DoorUtama bisa berinteraksi kembali
        if (doorScript != null)
        {
            doorScript.ResetInteraction();  // Panggil fungsi ResetInteraction dari DoorUtama
        }

        bapakAble.SetActive(true);
        player.SetActive(true);

        if (playerScript != null)
        {
            playerScript.ResetPlayerMovement();
        }

        countdownCoroutine = StartCoroutine(StartCountdown(20));
    }

     // Coroutine untuk countdown
    private IEnumerator StartCountdown(int countdownTime)
    {
        int currentTime = countdownTime;

        while (currentTime > 0)
        {
            countdownText.text = currentTime.ToString(); // Update teks countdown
            audioCountdown.PlayOneShot(countdownClip);
            yield return new WaitForSeconds(1f); // Tunggu 1 detik
            currentTime--;
        }

        countdownText.text = "";
        StartCoroutine(FadeIn());
    }

      // Fungsi untuk menghentikan countdown
    public void StopCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine); // Hentikan countdown jika coroutine sedang berjalan
            countdownText.text = ""; // Bersihkan teks countdown
        }
    }

     // Fungsi untuk memulai fade in
    private IEnumerator FadeIn()
    {
        explosion.SetActive(true);

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true); // Aktifkan image fade in
            Color fadeColor = fadeImage.color;
            fadeColor.a = 0f;
            fadeImage.color = fadeColor;

            float timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                fadeColor.a = Mathf.Lerp(0f, 1f, timer / fadeDuration); // Gradually increase alpha
                fadeImage.color = fadeColor;
                yield return null;
            }

            fadeColor.a = 1f;
            fadeImage.color = fadeColor;

            // Setelah fade in selesai, panggil fungsi game over
            gameOverManager.TriggerGameOver();
        }
    }

    // Fungsi untuk beralih ke kamera cutscene
    private void SwitchToCutsceneCamera()
    {
        if (cutsceneCamera != null && playerCamera != null)
        {
            playerCamera.gameObject.SetActive(false); // Nonaktifkan kamera player
            cutsceneCamera.gameObject.SetActive(true); // Aktifkan kamera cutscene
        }
    }

    // Fungsi untuk mengembalikan ke kamera player
    private void SwitchToPlayerCamera()
    {
        if (cutsceneCamera != null && playerCamera != null)
        {
            cutsceneCamera.gameObject.SetActive(false); // Nonaktifkan kamera cutscene
            playerCamera.gameObject.SetActive(true); // Aktifkan kamera player
        }
    }

    public void ResetInteraction()
    {
        intText.SetActive(false);
        Object.enabled = true; // Pastikan collider aktif kembali
        interactable = false;  // Reset flag interaksi
        Debug.Log("Interaction reset after player respawn.");
        
        // Force enable collider after respawn, ignoring enemy chase state
        enemyAI1.isChasing = false;
    }

    public void ResetDialog()
    {
        // Reset dialog UI and interaction states
        textComponent.text = string.Empty; // Clear any active dialog text
        intText.SetActive(false); // Hide interaction prompt
        interactable = false; // Reset interaction flag
        isDialogueActive = false; // Ensure dialog is not active
        DialogBG.SetActive(false); // Hide dialog background
        StopGibberish(); // Stop any gibberish audio
    }
}
