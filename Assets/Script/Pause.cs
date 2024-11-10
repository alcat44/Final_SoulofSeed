using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject Pausemenu;
    public GameObject howToPlayCanvas;
    public string sceneName;
    public bool toggle;
    public SC_FPSController playerScript;
    public SC_FPSController playerScript1;
    public SC_FPSController playerScript2;
    public SC_FPSController playerScript3;
    public SC_FPSController playerScript4;
    public SC_FPSController playerScript5;
    public AudioSource audioSource;
    public AudioManager audioManager;  // Reference to AudioManager

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            toggle = !toggle;

            if (toggle == false)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void resumeGame()
    {
        toggle = false;
        ResumeGame();
    }

    public void PauseGame()
    {
        Pausemenu.SetActive(true);
        Time.timeScale = 0;
        playerScript.enabled = false;
        playerScript1.enabled = false;
        playerScript2.enabled = false;
        playerScript3.enabled = false;
        playerScript4.enabled = false;
        playerScript5.enabled = false;
        audioSource.enabled = false;
        DisableAudioManagerSources();  // Disable AudioSources in AudioManager
        Cursor.visible = true;  // Show cursor
        Cursor.lockState = CursorLockMode.None;  // Free cursor
    }

    public void ResumeGame()
    {
        Pausemenu.SetActive(false);
        Time.timeScale = 1;
        playerScript.enabled = true;
        playerScript1.enabled = true;
        playerScript2.enabled = true;
        playerScript3.enabled = true;
        playerScript4.enabled = true;
        playerScript5.enabled = true;
        audioSource.enabled = true;
        EnableAudioManagerSources();  // Re-enable AudioSources in AudioManager
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;  // Lock cursor
    }

    public void Mainmenu()
    {
        SceneManager.LoadScene(sceneName);
        playerScript.enabled = true;
        playerScript1.enabled = true;
        playerScript2.enabled = true;
        playerScript3.enabled = true;
        playerScript4.enabled = true;
        playerScript5.enabled = true;
        audioSource.enabled = true;
        EnableAudioManagerSources();  // Re-enable AudioSources in AudioManager
        Time.timeScale = 1;
    }

    public void Exit()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void ShowHowToPlay()
    {
        howToPlayCanvas.SetActive(true);
        Pausemenu.SetActive(false);
    }

    public void HideHowToPlay()
    {
        howToPlayCanvas.SetActive(false);
        Pausemenu.SetActive(true);
    }

    private void DisableAudioManagerSources()
    {
        audioManager.Chasemusicbg.Pause();
        audioManager.SFXSource.Pause();
        audioManager.SFXLaugh.Pause();
    }

    private void EnableAudioManagerSources()
    {
        audioManager.Chasemusicbg.UnPause();
        audioManager.SFXSource.UnPause();
        audioManager.SFXLaugh.UnPause();
    }
}
