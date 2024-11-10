using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseApart : MonoBehaviour
{
    public GameObject Pausemenu;
    public GameObject howToPlayCanvas;
    public string sceneName;
    public bool toggle;
    public SC_FPSController playerScript;
    public AudioSource audioSource;
    
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
        audioSource.enabled = false;
        Cursor.visible = true;  // Show cursor
        Cursor.lockState = CursorLockMode.None;  // Free cursor
    }

    public void ResumeGame()
    {
        Pausemenu.SetActive(false);
        Time.timeScale = 1;
        playerScript.enabled = true;
        audioSource.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;  // Lock cursor
    }

    public void Mainmenu()
    {
        SceneManager.LoadScene(sceneName);
        playerScript.enabled = true;
        audioSource.enabled = true;
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
}
