using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public UIController uiController;  // Reference to UIController
    public Pause pauseScript;
    public SC_FPSController playerScript;
    public AudioSource audioSource;
    public GameObject livesText;
    public int respawnCount = 0;  // Set to public so it can be accessed by UIController
    public int maxRespawns = 3;
    public bool isGameOver = false;

    public AudioClip gameOverClip;
    public AudioSource GameOver;

    void Start()
    {
        // Call UpdateLivesText immediately when the game starts
        if (uiController != null)
        {
            uiController.UpdateLivesText(maxRespawns);  // Set text to max lives on start
        }
    }

    public void PlayerRespawned()
    {
        respawnCount++;
        if (respawnCount > maxRespawns && !isGameOver)  // Change condition here
        {
            isGameOver = true;
            TriggerGameOver();
        }
        else
        {
            if (uiController != null)
            {
                uiController.UpdateLivesText(maxRespawns - respawnCount);  // Call to update the text after respawn
            }
        }
    }

    public void TriggerGameOver()
    {
        livesText.SetActive(false);
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;  // Pause game
        playerScript.enabled = false;
        audioSource.enabled = false;

        if (gameOverClip != null)
        {
            GameOver.clip = gameOverClip;
            GameOver.Play();  // Mainkan audio game over
        }

        if (pauseScript != null)
        {
            pauseScript.enabled = false;
        }

        // Unlock and show the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Game Over");
    }

    // Fungsi untuk restart game (memuat ulang scene saat ini)
    public void RestartGame()
    {
        Time.timeScale = 1;  // Kembalikan waktu ke normal
        SceneManager.LoadScene("SampleScene");  // Muat ulang scene "SampleScene"
    }

    // Fungsi untuk kembali ke main menu
    public void GoToMainMenu()
    {
        Time.timeScale = 1;  // Kembalikan waktu ke normal
        SceneManager.LoadScene("MenuAwal");  // Pindah ke scene "MenuAwal"
    }
}
