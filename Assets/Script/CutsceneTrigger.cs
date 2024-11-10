using System.Collections;
using UnityEngine;
using TMPro;

public class CutsceneTrigger : MonoBehaviour
{
    public AudioSource scareSound;
    public Collider collision; // Collider yang digunakan sebagai trigger jumpscare
    public GameObject Jumpscare;
    public GameObject cutsceneCamera; // Kamera untuk cutscene
    public TextMeshProUGUI conversationText; // TextMeshPro untuk percakapan
    public MonoBehaviour playerMovementScript; // Script movement player

    void Start()
    {
        collision.enabled = false; // Pastikan collider dinonaktifkan pada awalnya
        cutsceneCamera.SetActive(false); // Pastikan kamera cutscene tidak aktif di awal
        conversationText.gameObject.SetActive(false); // Pastikan teks percakapan tidak aktif di awal
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Jumpscare.SetActive(true);
            collision.enabled = false; // Nonaktifkan collider setelah trigger
            PlayScareSound();
            DisablePlayerMovement(); // Nonaktifkan movement player
            StartCoroutine(CutsceneCoroutine()); // Mulai cutscene dengan penundaan
        }
    }

    void PlayScareSound()
    {
        if (scareSound != null)
        {
            scareSound.Play();
        }
    }

    void DisablePlayerMovement()
    {
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false; // Nonaktifkan script movement player
        }
    }

    void EnablePlayerMovement()
    {
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true; // Aktifkan kembali script movement player
        }
    }

    // Fungsi untuk mengaktifkan collider jumpscare
    public void ActivateJumpscareTrigger()
    {
        collision.enabled = true;
    }

    IEnumerator CutsceneCoroutine()
    {
        yield return new WaitForSeconds(2.0f); // Penundaan 2 detik sebelum cutscene dimulai

        cutsceneCamera.SetActive(true); // Aktifkan kamera cutscene

        // Nonaktifkan kamera lain jika diperlukan
        Camera mainCamera = Camera.main;
        if (mainCamera != null && mainCamera.gameObject != cutsceneCamera)
        {
            mainCamera.gameObject.SetActive(false);
        }

        conversationText.gameObject.SetActive(true); // Aktifkan teks percakapan

        // Tampilkan teks percakapan selama cutscene
        conversationText.text = "Percakapan dimulai...";
        yield return new WaitForSeconds(3.0f); // Tampilkan teks pertama selama 3 detik

        conversationText.text = "Percakapan berlanjut...";
        yield return new WaitForSeconds(3.0f); // Tampilkan teks kedua selama 3 detik

        conversationText.text = "Percakapan berakhir...";
        yield return new WaitForSeconds(4.0f); // Tampilkan teks ketiga selama 4 detik

        conversationText.gameObject.SetActive(false); // Nonaktifkan teks percakapan
        cutsceneCamera.SetActive(false); // Nonaktifkan kamera cutscene

        // Aktifkan kembali kamera utama jika diperlukan
        if (mainCamera != null && mainCamera.gameObject != cutsceneCamera)
        {
            mainCamera.gameObject.SetActive(true);
        }

        Jumpscare.SetActive(false); // Nonaktifkan jumpscare setelah cutscene selesai
        EnablePlayerMovement(); // Aktifkan kembali movement player
    }
}
