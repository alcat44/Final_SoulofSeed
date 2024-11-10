using UnityEngine;

public class HowToPlay : MonoBehaviour
{
    public GameObject howToPlayCanvas;

    // Method untuk menampilkan canvas
    public void ShowHowToPlay()
    {
        howToPlayCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    // Method untuk menyembunyikan canvas
    public void HideHowToPlay()
    {
        howToPlayCanvas.SetActive(false);
        Time.timeScale = 1;
    }
}
