using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject livesText;  // The UI text object for displaying lives

    // Function to update livesText based on remaining lives
    public void UpdateLivesText(int remainingLives)
    {
        if (livesText != null)
        {
            livesText.GetComponent<TextMeshProUGUI>().text = remainingLives.ToString();
        }
    }
}
