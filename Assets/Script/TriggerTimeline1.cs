using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class TriggerTimeline1 : MonoBehaviour
{
    public PlayableDirector timelineDirector; // Referensi ke PlayableDirector yang menjalankan Timeline
    public Camera timelineCamera;             // Kamera yang digunakan oleh Timeline
    public Camera playerCamera;               // Kamera untuk Player
    public GameObject Objective1;
    public GameObject Objective2;
    public AudioSource audiosource;           // Perbaiki nama variabel ini
    public SC_FPSController playerScript;     // Referensi ke script player

    void Awake()
    {
        // Nonaktifkan kamera player saat memulai, aktifkan kamera timeline
        playerCamera.gameObject.SetActive(false);
        timelineCamera.gameObject.SetActive(true);

        // Nonaktifkan Objective1 dan Objective2 sebelum game benar-benar dimulai
        Objective1.SetActive(false);
        Objective2.SetActive(false);
    }

    void Start()
    {
        audiosource.enabled = false;
        playerScript.enabled = false;

        // Mulai memainkan timeline
        timelineDirector.Play();

        // Pasang callback untuk beralih ke kamera player saat timeline selesai
        timelineDirector.stopped += OnTimelineStopped;
    }

    void OnTimelineStopped(PlayableDirector director)
    {
        // Ketika timeline selesai, nonaktifkan kamera timeline dan aktifkan kamera player
        timelineCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        Objective1.SetActive(true);
        Objective2.SetActive(true);
        audiosource.enabled = true;
        playerScript.enabled = true;

        // Lepas callback untuk menghindari pemanggilan yang tidak perlu di masa depan
        timelineDirector.stopped -= OnTimelineStopped;
    }
}
