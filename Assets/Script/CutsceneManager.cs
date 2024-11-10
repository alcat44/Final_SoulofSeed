using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    public TMP_Text cutsceneText;
    public Canvas cutsceneCanvas;
    public Canvas mainmenuCanvas;
    public string[] cutsceneLines;
    public float textDisplayTime = 3.0f;
    public string sceneName;

    private bool isSkipping = false; // Menandai apakah cutscene sedang diskip

    private void Awake()
    {
        StartCoroutine(PlayCutscene());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isSkipping = true;
        }
    }

    private IEnumerator PlayCutscene()
    {
        cutsceneCanvas.gameObject.SetActive(true);

        foreach (string line in cutsceneLines)
        {
            cutsceneText.text = line;
            float elapsedTime = 0f;

            // Tampilkan teks selama waktu tertentu atau hingga tombol Spacebar ditekan
            while (elapsedTime < textDisplayTime)
            {
                if (isSkipping)
                {
                    break;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (isSkipping)
            {
                break;
            }
        }

        cutsceneCanvas.gameObject.SetActive(false);
        StartGame();
    }

    private void StartGame()
    {
        SceneManager.LoadScene(sceneName);
        // Masukkan logika untuk memulai game di sini.
        Debug.Log("Game Dimulai!");
    }
}
