using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Timline2 : MonoBehaviour
{
    public PlayableDirector timeline;
    public GameObject Camera1;
    public GameObject Camera2;
    public GameObject colliderObject; // Menambahkan referensi ke objek collider
    public GameObject exhaustionText;
    public GameObject Key1;
    public GameObject Key2;
    public GameObject livesText;
    public SC_FPSController playerScript;
    public AudioSource audioscource;
    public enemyAI1 enemyScript;
    public Collider Door1;
    public Collider Door2;

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
            exhaustionText.SetActive(false);
            // Mengatur kamera dan objek
            Camera2.SetActive(true);
            Camera1.SetActive(false);
            DisablePlayerMovement();
            Door1.enabled = false;
            Door2.enabled = false;
            
            // Memulai timeline dan menjalankan coroutine untuk kontrol
            timeline.Play();
            StartCoroutine(AllowSkipAfterDelay());
        }
    }

    void Update()
    {
        // Mengecek apakah tombol 'Spacebar' ditekan untuk melewatkan timeline
        if (canSkip && Input.GetKeyDown(KeyCode.E))
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
        }
    }

    void EnablePlayerMovement()
    {
        if (playerScript != null)
        {
            playerScript.enabled = true; // Aktifkan kembali script movement player
            audioscource.enabled = true;
            enemyScript.enabled = true;
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
        // Mengembalikan kamera ke Camera1 dan menghilangkan SkipButton
        Camera1.SetActive(true);
        Camera2.SetActive(false);
        Key1.SetActive(true);
        Key2.SetActive(true);
        livesText.SetActive(true);
        EnablePlayerMovement();
        Door1.enabled = true;
        Door2.enabled = true;

        // Debug untuk memastikan apakah colliderObject sudah terisi
        if (colliderObject != null)
        {
            Debug.Log("Destroying colliderObject: " + colliderObject.name); // Log nama collider sebelum menghancurkan
            Destroy(colliderObject);
        }
        else
        {
            Debug.Log("colliderObject is null!"); // Jika colliderObject ternyata null
        }
    }


}
