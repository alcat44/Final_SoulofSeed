using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Dropdown graphicsDrop, resoDrop;
    public Slider volumeSlider;
    public Toggle chromaticToggle, vignetteToggle, grainToggle;
    public bool inGame;
    public GameObject chromaticCam, vignetteCam, grainCam;

    void Start()
    {
        if(PlayerPrefs.GetInt("settingsSaved", 0) == 0)
        {
            PlayerPrefs.SetInt("graphics", 0);
            PlayerPrefs.SetInt("resolution", 0);
            PlayerPrefs.SetFloat("mastervolume", 1.0f);
            PlayerPrefs.SetInt("chromatic", 0);
            PlayerPrefs.SetInt("vignette", 0);
            PlayerPrefs.SetInt("grain", 0);
        }
        //Graphics
        if(PlayerPrefs.GetInt("graphics", 2) == 2)
        {
            graphicsDrop.value = 0;
            QualitySettings.SetQualityLevel(0);
        }
        if (PlayerPrefs.GetInt("graphics", 1) == 1)
        {
            graphicsDrop.value = 1;
            QualitySettings.SetQualityLevel(1);
        }
        if (PlayerPrefs.GetInt("graphics", 0) == 0)
        {
            graphicsDrop.value = 2;
            QualitySettings.SetQualityLevel(2);
        }
        //Resolution
        if (PlayerPrefs.GetInt("resolution", 2) == 2)
        {
            resoDrop.value = 0;
            Screen.SetResolution(854, 480, true);
        }
        if (PlayerPrefs.GetInt("resolution", 1) == 1)
        {
            resoDrop.value = 1;
            Screen.SetResolution(1280, 720, true);
        }
        if (PlayerPrefs.GetInt("resolution", 0) == 0)
        {
            resoDrop.value = 2;
            Screen.SetResolution(1920, 1080, true);
        }
        //Volume
        volumeSlider.value = PlayerPrefs.GetFloat("mastervolume");
        AudioListener.volume = PlayerPrefs.GetFloat("mastervolume");
        //Chromatic Aberration
        if (PlayerPrefs.GetInt("chromatic", 1) == 1)
        {
            chromaticToggle.isOn = false;
            if(inGame == true)
            {
                chromaticCam.SetActive(false);
            }
        }
        if (PlayerPrefs.GetInt("chromatic", 0) == 0)
        {
            chromaticToggle.isOn = true;
            if (inGame == true)
            {
                chromaticCam.SetActive(true);
            }
        }
        //Vignette
        if (PlayerPrefs.GetInt("vignette", 1) == 1)
        {
            vignetteToggle.isOn = false;
            if (inGame == true)
            {
                vignetteCam.SetActive(false);
            }
        }
        if (PlayerPrefs.GetInt("vignette", 0) == 0)
        {
            vignetteToggle.isOn = true;
            if (inGame == true)
            {
                vignetteCam.SetActive(true);
            }
        }
        //Grain
        if (PlayerPrefs.GetInt("grain", 1) == 1)
        {
            grainToggle.isOn = false;
            if (inGame == true)
            {
                grainCam.SetActive(false);
            }
        }
        if (PlayerPrefs.GetInt("grain", 0) == 0)
        {
            grainToggle.isOn = true;
            if (inGame == true)
            {
                grainCam.SetActive(true);
            }
        }
    }
}