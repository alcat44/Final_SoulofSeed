using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.AI; // Jika enemy menggunakan NavMeshAgent

public class Dialogrespawn : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    public GameObject Dialog;
    public GameObject DialogBG;
    public GameObject player;
    public GameObject pauseMenuObject;
    public Collider bedCollider; // Collider pintu
    public AudioSource audioSource;
    public PlayableDirector timeline;

    public Camera playerCamera;
    public Camera timelineCamera;

    public GameObject enemy; // Tambahkan reference ke enemy
    private NavMeshAgent enemyNavMeshAgent; // Jika enemy menggunakan NavMeshAgent
    public MonoBehaviour enemyAI1;
    
    public bool isDialogueActive = false;
    private bool hasPlayedTimeline = false;
    private bool canSkip = false;  // To allow skipping the timeline after a delay
    private bool timelinePlaying = false;

    private int index;
    private SC_FPSController scFpsController;
    private Pause pauseScript;

    // Gibberish Sound Properties
    public AudioClip[] SpeakNoises = new AudioClip[0]; // Array suara gibberish
    public AudioSource gibberishSource; // AudioSource untuk gibberish
    public float gibberishPitch = 1.0f; // Kecepatan playback untuk audio gibberish (1.0 = normal speed)
    private Coroutine gibberishCoroutine; // Coroutine untuk gibberish
    private bool isGibberishPlaying = false; // Flag untuk mengecek apakah gibberish sedang diputar

    void Start()
    {
        scFpsController = player.GetComponent<SC_FPSController>();
        pauseScript = pauseMenuObject.GetComponent<Pause>();

        if (timeline != null)
        {
            timeline.stopped += OnTimelineStopped;
        }

        if (timelineCamera != null)
        {
            timelineCamera.gameObject.SetActive(false);
        }

        // Ambil referensi ke NavMeshAgent enemy jika ada
        if (enemy != null)
        {
            enemyNavMeshAgent = enemy.GetComponent<NavMeshAgent>();
        }
    }

    void Update()
    {
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
                StopGibberish(); // Stop audio gibberish jika tombol E ditekan untuk skip
            }
        }

        // Allow skipping the timeline after 5 seconds
        if (timelinePlaying && canSkip && Input.GetKeyDown(KeyCode.E))
        {
            SkipTimeline();
        }
    }

    void PlayTimeline()
    {
        if (pauseScript != null) pauseScript.enabled = true;
        DialogBG.SetActive(false);

        if (enemyAI1 != null)
        {
            enemyAI1.enabled = false;
        }

          // Hentikan gerakan musuh
        if (enemyNavMeshAgent != null)
        {
            enemyNavMeshAgent.isStopped = true; // Hentikan NavMeshAgent
        }

        if (timelineCamera != null && playerCamera != null)
        {
            playerCamera.gameObject.SetActive(false);
            timelineCamera.gameObject.SetActive(true);
            if (scFpsController != null) scFpsController.enabled = false;
            if (audioSource != null) audioSource.enabled = false;
        }

        if (timeline != null)
        {
            timeline.Play();
            timelinePlaying = true;
            StartCoroutine(AllowSkipAfterDelay());  // Allow skipping after 5 seconds
        }
    }

    // Coroutine to allow skipping after 5 seconds
    IEnumerator AllowSkipAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        canSkip = true;  // Allow skipping after 5 seconds
    }

    void SkipTimeline()
    {
        if (timeline.state == PlayState.Playing)
        {
            timeline.time = timeline.duration;
            timeline.Evaluate();
            EndTimeline();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("MainCamera")) && !isDialogueActive)
        {

            if (!hasPlayedTimeline)
            {
                PlayTimeline();
                hasPlayedTimeline = true;
            }
            else if (!timeline.playableGraph.IsValid())
            {
                StartDialogue();
                isDialogueActive = true;
            }
        }
    }

    void OnTimelineStopped(PlayableDirector director)
    {
        EndTimeline();
    }

    void EndTimeline()
    {
        if (timelineCamera != null && playerCamera != null)
        {
            timelineCamera.gameObject.SetActive(false);
            playerCamera.gameObject.SetActive(true);
            if (scFpsController != null) scFpsController.enabled = true;
            if (audioSource != null) audioSource.enabled = true;
        }

        if (enemyAI1 != null)
        {
            enemyAI1.enabled = true;
        }

        // Lanjutkan gerakan musuh
        if (enemyNavMeshAgent != null)
        {
            enemyNavMeshAgent.isStopped = false; // Lanjutkan NavMeshAgent
        }

        timelinePlaying = false;
        canSkip = false;  // Reset the ability to skip
        Dialog.SetActive(false);

        if (bedCollider != null)
        {
            bedCollider.enabled = true;
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
        DialogBG.SetActive(true);
        Dialog.SetActive(true);

        if (scFpsController != null) scFpsController.enabled = false;
        if (pauseScript != null) pauseScript.enabled = false;
        if (audioSource != null) audioSource.enabled = false;

         // Hentikan gerakan musuh
        if (enemyNavMeshAgent != null)
        {
            enemyNavMeshAgent.isStopped = true; // Hentikan NavMeshAgent
        }

        if (enemyAI1 != null)
        {
            enemyAI1.enabled = false;
        }
    }

    IEnumerator TypeLine()
    {
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
        textComponent.text = string.Empty;
        DialogBG.SetActive(false);
        Dialog.SetActive(false);
        isDialogueActive = false;

        if (scFpsController != null) scFpsController.enabled = true;
        if (pauseScript != null) pauseScript.enabled = true;
        if (audioSource != null) audioSource.enabled = true;
        if (bedCollider != null)
        {
            bedCollider.enabled = true;
        }

        // Lanjutkan gerakan musuh
        if (enemyNavMeshAgent != null)
        {
            enemyNavMeshAgent.isStopped = false; // Lanjutkan NavMeshAgent
        }

        if (enemyAI1 != null)
        {
            enemyAI1.enabled = true;
        }
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
