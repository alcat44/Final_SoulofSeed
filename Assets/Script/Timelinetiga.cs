using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Timelinetiga : MonoBehaviour
{
    public PlayableDirector timeline;
    public GameObject Camera1;
    public GameObject Camera2;
    public GameObject Objective1;
    public GameObject Objective2;
    public GameObject colliderObject; // Referensi ke objek collider
    public SC_FPSController playerScript;
    public AudioSource audioscource; // Tambahkan audioclip kedua
    public SoundEffectsPlayer1 Audio;
    public UnityEngine.AI.NavMeshAgent navmeshserem;
    public GameObject ondel1;
    public GameObject Jumpscare;
    public Collider postJumpscareTrigger;

    private bool canSkip = false; // Menyimpan status apakah bisa skip atau tidak

    void Start()
    {
        // Pastikan timeline diinisialisasi dengan benar
        if (timeline == null)
        {
            timeline = GetComponent<PlayableDirector>();
        }

        // Menghubungkan event untuk menangani akhir timeline
        timeline.stopped += OnTimelineStopped;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Mengatur kamera dan objek
            Camera2.SetActive(true);
            Camera1.SetActive(false);
            Objective1.SetActive(false);
            Objective2.SetActive(false);
            ondel1.SetActive(true);
            Jumpscare.SetActive(true);

            DisablePlayerMovement();

            // Memulai timeline dan menjalankan coroutine untuk kontrol
            timeline.Play();
            StartCoroutine(AllowSkipAfterDelay());
        }
    }

    void Update()
    {
        // Mengecek apakah tombol 'Spacebar' ditekan untuk melewatkan timeline
        if (canSkip && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(SkipTimeline());
        }
    }

    IEnumerator AllowSkipAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        canSkip = true;
    }

    void DisablePlayerMovement()
    {
        if (playerScript != null)
        {
            playerScript.enabled = false; // Nonaktifkan script movement player
            audioscource.enabled = false;
            navmeshserem.enabled = false; // Nonaktifkan NavMeshAgent
        }
    }

    void EnablePlayerMovement()
    {
        if (playerScript != null)
        {
            Audio.Chasemusicbg.enabled = true;
            Audio.SFXSource.enabled = true;
            Audio.SFXLaugh.enabled = true;
            playerScript.enabled = true; // Aktifkan kembali script movement player
            audioscource.enabled = true; // Aktifkan audioclip kedua
            navmeshserem.enabled = true; // Aktifkan NavMeshAgent
        }
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        EndCutscene();
    }

    IEnumerator SkipTimeline()
    {
        // Memastikan bahwa timeline dimulai dan ada perubahan waktu
        if (timeline.playableGraph.IsPlaying())
        {
            timeline.time = timeline.duration;
            timeline.Evaluate(); // Memastikan evaluasi timeline
            EndCutscene();
        }
        yield return null;
    }

    private void EndCutscene()
    {
        // Mengembalikan kamera ke Camera1 dan menghilangkan objek ondel
        Camera1.SetActive(true);
        Camera2.SetActive(false);
        Objective1.SetActive(true);
        Objective2.SetActive(true);
        EnablePlayerMovement();

        // Menghancurkan objek collider
        if (colliderObject != null)
        {
            Destroy(colliderObject);
        }

        // Memulai post-jumpscare dengan penundaan
        StartCoroutine(PostJumpscare());
    }

    IEnumerator PostJumpscare()
    {
        yield return new WaitForSeconds(2.0f); // Penundaan 2 detik sebelum jumpscare selesai
        postJumpscareTrigger.enabled = true; // Dihapus karena sudah diaktifkan sebelumnya
    }
}
