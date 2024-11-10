using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MENU : MonoBehaviour
{
    public GameObject loadingscreen, menuobj, settingsobj;
    public string sceneName;
    public Canvas menuCanvas;

    public void playGame()
    {
        menuCanvas.gameObject.SetActive(false);
        SceneManager.LoadScene(sceneName);
        //loadingscreen.SetActive(true);
        
    }
    public void quitGame()
    {
        Debug.Log("quit game");
        Application.Quit();
    }
    public void settingTosMenu()
    {
        menuobj.SetActive(false);
        settingsobj.SetActive(true);
    }
    public void backToMenu()
    {
        settingsobj.SetActive(false);
        menuobj.SetActive(true);
    }

}