using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class tidur : MonoBehaviour
{
    public Dialogpintu dialogPintu;
    public GameObject intText;
    public GameObject exhaustionText;
    public GameObject kasur2;
    public bool interactable;
    public Collider laci, laci2, meja, lemari1, lemari2, laci4, kasur3;
    public PlayableDirector timeline;
    public Camera mainCamera;
    public Camera cutsceneCamera;
    public SC_FPSController playerScript;
    public MonoBehaviour kasur;
    public AudioSource audioscource;
    private bool cutscenePlaying = false;
    private bool canSkip = false;
    private bool hasPlayed = false; // Tambahkan variabel ini untuk mengecek apakah timeline sudah diputar

    void Start()
    {
        if (timeline == null)
        {
            timeline = GetComponent<PlayableDirector>();
        }

        timeline.stopped += OnTimelineStopped;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera") && !cutscenePlaying)
        {
            intText.SetActive(true);
            exhaustionText.SetActive(true);
            interactable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false);
            exhaustionText.SetActive(false);
            interactable = false;
        }
    }

    void Update()
    {
        if (interactable && !cutscenePlaying && !hasPlayed) // Cek apakah timeline belum diputar
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayTimeline();
            }
        }

        if (cutscenePlaying && canSkip && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SkipTimeline());
        }
    }

    void PlayTimeline()
    {
        if (!cutscenePlaying && !hasPlayed) // Pastikan timeline belum diputar
        {
            hasPlayed = true; // Set agar timeline tidak bisa diputar lagi
            cutscenePlaying = true;
            timeline.Play();
            DisablePlayerMovement();
            cutsceneCamera.gameObject.SetActive(true);
            mainCamera.gameObject.SetActive(false);
            intText.SetActive(false);
            exhaustionText.SetActive(false);
            StartCoroutine(AllowSkipAfterDelay());
        }
    }

    IEnumerator AllowSkipAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        canSkip = true; // Mengizinkan skip setelah 5 detik
    }

    void DisablePlayerMovement()
    {
        if (playerScript != null)
        {
            playerScript.enabled = false;
            audioscource.enabled = false;
        }
    }

    void EnablePlayerMovement()
    {
        if (playerScript != null)
        {
            playerScript.enabled = true;
            audioscource.enabled = true;
            laci.enabled = true;
            laci2.enabled = true;
            meja.enabled = true;
            lemari1.enabled = true;
            lemari2.enabled = true;
            laci4.enabled = true;
            kasur2.SetActive(true);
            kasur.enabled = true;
            kasur3.enabled = false;
        }
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        EndCutscene();
    }

    IEnumerator SkipTimeline()
    {
        if (timeline.state == PlayState.Playing) // Pastikan timeline sedang berjalan
        {
            timeline.time = timeline.duration;
            timeline.Evaluate();
            EndCutscene();
        }
        yield return null;
    }

    void EndCutscene()
    {
        cutsceneCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        EnablePlayerMovement();
        dialogPintu.hasTuru = true;
        cutscenePlaying = false;
        canSkip = false;
    }
}
