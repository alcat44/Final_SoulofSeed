using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameFinished : MonoBehaviour
{
    public GameObject boss;
    //public AudioSource audioboss;
    public Canvas cutsceneCanvas;
     public TMP_Text cutsceneText;
     public string[] cutsceneLines;
    public float textDisplayTime = 3.0f;
    public string sceneName;
    

    private IEnumerator PlayCutscene()
    {
        boss.SetActive(false);
        //audioboss.enabled = false;
        Time.timeScale = 0;
        cutsceneCanvas.gameObject.SetActive(true);

        foreach (string line in cutsceneLines)
        {
            cutsceneText.text = line;
            yield return new WaitForSecondsRealtime(textDisplayTime);
        }

        cutsceneCanvas.gameObject.SetActive(false);
        SceneManager.LoadScene(sceneName);;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Bisa Keluar");
            StartCoroutine(PlayCutscene());
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
