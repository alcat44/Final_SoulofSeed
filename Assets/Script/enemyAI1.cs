using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyAI1 : MonoBehaviour
{
    public NavMeshAgent ai;
    public List<Transform> destinations;
    public Animator aiAnim;
    public float walkSpeed, chaseSpeed, minIdleTime, maxIdleTime, idleTime, detectionDistance, catchDistance, searchDistance, minChaseTime, maxChaseTime, minSearchTime, maxSearchTime, jumpscareTime;
    public bool walking, chasing, searching;
    public Transform player;
    public Transform respawnPoint;
    public Vector3 respawnRotation;
    public Transform enemyRespawnPoint;
    Transform currentDest;
    Vector3 dest;
    public Vector3 rayCastOffset;
    public GameObject mainCamera;
    public GameObject jumpscareCamera;
    public GameObject Dialog;
    public GameObject exhaustionText;
    public GameObject intText;
    public GameObject hide;
    public GameObject stopHide;
    public Collider bedCollider; // Collider pintu
    public float aiDistance;
    public bool isDead;
    public static bool isChasing;
    public Image fadeImage;
    public float fadeDuration = 1f;

    public AudioManager Audio;
    private AudioSource audioSource;
    private string previousAnimationState;

    public GameOverManager gameOverManager;
    

    void Start()
    {
        walking = true;
        currentDest = destinations[Random.Range(0, destinations.Count)];
        ai.destination = currentDest.position;

        // Memastikan layar hitam di awal tidak terlihat (alpha = 0)
        SetImageAlpha(0);

        // Pastikan jumpscareCamera tidak aktif pada awalnya
        jumpscareCamera.SetActive(false);
        mainCamera.SetActive(true);  // Pastikan kamera utama aktif

        audioSource = GetComponent<AudioSource>();

    }

    void Update()
    {
        if (isDead) return;

        Vector3 direction = (player.position - transform.position).normalized;
        RaycastHit hit;
        aiDistance = Vector3.Distance(player.position, this.transform.position);

        Debug.DrawRay(transform.position, direction * detectionDistance, Color.red);
        if (Physics.Raycast(transform.position, direction, out hit, detectionDistance))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                // Cek apakah kita belum dalam kondisi searching atau chasing untuk mencegah pemutaran suara berulang kali
                if (!searching && !chasing)
                {
                    Debug.Log("Player detected!");
                    walking = false;
                    StopAllCoroutines();
                    StartCoroutine(searchRoutine());
                    searching = true;
                    Audio.SFXSource.Stop();  // Stop all previous sounds before starting search

                    // Memainkan suara pencarian
                    Audio.sfx6();
                    previousAnimationState = "search";
                }
            }
        }

        if (searching)
        {
            ai.speed = 0;
            aiAnim.ResetTrigger("walk");
            aiAnim.ResetTrigger("idle");
            aiAnim.ResetTrigger("sprint");
            aiAnim.SetTrigger("search");

            if (aiDistance <= searchDistance)
            {
                StopAllCoroutines();
                StartCoroutine(chaseRoutine());
                chasing = true;
                searching = false;
                Audio.bgm();  // Mainkan BGM chasing
            }
        }

        if (chasing)
        {
            // Mulai mengejar player
            dest = player.position;
            ai.destination = dest;
            ai.speed = chaseSpeed;

            aiAnim.ResetTrigger("walk");
            aiAnim.ResetTrigger("idle");
            aiAnim.ResetTrigger("search");
            aiAnim.SetTrigger("sprint");

            // Pastikan suara chasing hanya diputar sekali
            if (previousAnimationState != "sprint")
            {
                Audio.SFXSource.Stop();  // Pastikan suara lain berhenti sebelum memulai sprint SFX

                // Mainkan SFX untuk sprint
                Audio.sfx2();  // Suara berjalan/motorik
                Audio.sfx5();  // Suara tertawa atau efek lain selama chasing

                previousAnimationState = "sprint";
            }

            if (aiDistance <= catchDistance)
            {
                player.gameObject.SetActive(false);
                StopAllCoroutines();
                StartCoroutine(deathRoutine());
                chasing = false;
                Audio.Chasemusicbg.Stop();
            }
        }

        if (walking == true)
        {
            dest = currentDest.position;
            ai.destination = dest;
            ai.speed = walkSpeed;
            aiAnim.ResetTrigger("sprint");
            aiAnim.ResetTrigger("idle");
            aiAnim.ResetTrigger("search");
            aiAnim.SetTrigger("walk");

            // Memastikan hanya memutar suara langkah jika belum diputar
            if (previousAnimationState != "walk")
            {
                Audio.SFXSource.Stop();  // Hentikan suara sebelumnya jika ada
                Audio.sfx1();  // Memutar suara langkah kaki
                previousAnimationState = "walk";
            }

            // Jika sudah mencapai tujuan, AI berhenti dan masuk ke state idle
            if (ai.remainingDistance <= ai.stoppingDistance)
            {
                aiAnim.ResetTrigger("walk");
                aiAnim.SetTrigger("idle");
                ai.speed = 0;

                // Memastikan hanya memutar suara idle jika belum diputar
                if (previousAnimationState != "idle")
                {
                    Audio.SFXSource.Stop();  // Hentikan semua suara langkah
                    Audio.sfx4();  // Memutar suara tawa idle
                    previousAnimationState = "idle";
                }

                StartCoroutine(stayIdle());  // AI berhenti sejenak
                walking = false;
            }
        }
    }

    public void stopChase()
    {
        walking = true;
        chasing = false;
        isChasing = false;  // Set isChasing to false saat musuh berhenti mengejar
        StopAllCoroutines();
        currentDest = destinations[Random.Range(0, destinations.Count)];
        ai.destination = currentDest.position;
        Audio.Chasemusicbg.Stop();
    }


    IEnumerator stayIdle()
    {

        idleTime = Random.Range(minIdleTime, maxIdleTime);
        yield return new WaitForSeconds(idleTime);

        walking = true;
        currentDest = destinations[Random.Range(0, destinations.Count)];
        ai.destination = currentDest.position;
        Audio.Chasemusicbg.Stop();
    }


    IEnumerator searchRoutine()
    {
        yield return new WaitForSeconds(Random.Range(minSearchTime, maxSearchTime));
        searching = false;
        walking = true;
        currentDest = destinations[Random.Range(0, destinations.Count)];
        ai.destination = currentDest.position;
        Audio.Chasemusicbg.Stop();
    }

    IEnumerator chaseRoutine()
    {
        isChasing = true;  // Set isChasing to true saat musuh mulai mengejar
        yield return new WaitForSeconds(Random.Range(minChaseTime, maxChaseTime));
        stopChase();
        Audio.Chasemusicbg.Stop();
    }


   IEnumerator deathRoutine()
    {
        exhaustionText.SetActive(false);
        intText.SetActive(false);
        hide.SetActive(false);
        isDead = true;

        // Nonaktifkan kamera utama dan aktifkan kamera jumpscare
        mainCamera.SetActive(false);
        jumpscareCamera.SetActive(true);
        stopHide.SetActive(false);
        Audio.SFXSource.Stop();
        Debug.Log("Jumpscare triggered!");
        aiAnim.SetTrigger("jumpscare");
        Audio.sfx3();

        // Dinonaktifkan selama animasi
        if (bedCollider != null)
        {
            bedCollider.enabled = false;
        }

        yield return new WaitForSeconds(jumpscareTime);

        // Mulai fade-out (menuju hitam penuh)
        yield return StartCoroutine(FadeOut());
        Audio.Chasemusicbg.Stop();

        // Panggil PlayerRespawned di sini, agar hanya dihitung setelah jumpscare selesai
        gameOverManager.PlayerRespawned();

        // Cek apakah game over, jika ya, hentikan eksekusi deathRoutine
        if (gameOverManager.isGameOver)
        {
            yield break;  // Stop jika sudah game over
        }

        // Respawn player dan musuh hanya jika belum game over
        Debug.Log("Respawning player...");
        player.position = respawnPoint.position;
        transform.rotation = Quaternion.Euler(respawnRotation);
        player.gameObject.SetActive(true);

        Door[] doors = FindObjectsOfType<Door>(); // Find all instances of Door script
        foreach (Door door in doors)
        {
            door.ResetInteraction(); // Call reset on each door
        }

        DropArea dropArea = FindObjectOfType<DropArea>(); // Ini akan langsung mencari DropArea yang ada di scene
        if (dropArea != null)
        {
            dropArea.ResetInteraction(); // Memanggil fungsi ResetInteraction di script DropArea
            dropArea.ResetDialog();
        }

        Doorpickable[] doorPickables = FindObjectsOfType<Doorpickable>(); // Temukan semua Doorpickable
        foreach (Doorpickable doorPickable in doorPickables)
        {
            doorPickable.ResetInteraction(); // Panggil reset pada setiap Doorpickable
        }

        DialogText[] allDialogs = FindObjectsOfType<DialogText>();
        foreach (DialogText dialog in allDialogs)
        {
            dialog.ResetDialog();
        }

        DialogNormal[] allDialogsNormal = FindObjectsOfType<DialogNormal>();
        foreach (DialogNormal dialogNormal in allDialogsNormal)
        {
            dialogNormal.ResetDialog();
        }

        DialogDestroy[] allDialogsDestroy = FindObjectsOfType<DialogDestroy>();
        foreach (DialogDestroy dialogDestroy in allDialogsDestroy)
        {
            dialogDestroy.ResetDialog();
        }

        PintuGembok[] allPintuGembok = FindObjectsOfType<PintuGembok>();
        foreach (PintuGembok pintuGembok in allPintuGembok)
        {
            pintuGembok.ResetDialog();
        }

        PintuGembok2[] allPintuGembok2 = FindObjectsOfType<PintuGembok2>();
        foreach (PintuGembok2 pintuGembok2 in allPintuGembok2)
        {
            pintuGembok2.ResetDialog();
        }

        // Reset hiding setelah respawn
        hidingPlace[] hidingPlaces = FindObjectsOfType<hidingPlace>();
        foreach (hidingPlace hideSpot in hidingPlaces)
        {
            hideSpot.ResetHiding();
        }

        // Reset gerakan player
        SC_FPSController playerController = player.GetComponent<SC_FPSController>();
        if (playerController != null)
        {
            playerController.ResetPlayerMovement();  // Reset script gerakan player
        }

        Debug.Log("Respawning enemy...");
        transform.position = enemyRespawnPoint.position;

        // Nonaktifkan kamera jumpscare dan aktifkan kembali kamera utama
        jumpscareCamera.SetActive(false);
        mainCamera.SetActive(true);

        // Set parameter isRespawning agar musuh kembali ke state awal
        aiAnim.SetBool("isRespawning", true);

        // Jeda kecil untuk memastikan animator kembali ke state awal
        yield return new WaitForSeconds(0.2f);

        // Reset state respawning
        aiAnim.SetBool("isRespawning", false);

        // Reset state AI ke keadaan awal
        ResetAIState();
        Audio.SFXSource.Stop();

        // Mulai fade-in (layar hitam perlahan menghilang)
        yield return StartCoroutine(FadeIn());

        isDead = false;  // Izinkan Update() berjalan lagi
    }



    void ResetAIState()
    {
        Dialog.SetActive(true);
        walking = true;
        chasing = false;
        searching = false;

        aiAnim.ResetTrigger("jumpscare");
        aiAnim.ResetTrigger("sprint");
        aiAnim.ResetTrigger("search");
        aiAnim.ResetTrigger("walk");
        aiAnim.ResetTrigger("idle");

        aiAnim.CrossFade("Walk", 0.1f);

        currentDest = destinations[Random.Range(0, destinations.Count)];
        ai.destination = currentDest.position;
        ai.speed = walkSpeed;
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 0f;
        fadeImage.color = color;
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 1f;
        fadeImage.color = color;
    }

    void SetImageAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }

}