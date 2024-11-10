using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public GameObject inttext, light;
    public bool toggle = true, interactable;
    public Renderer lightBulb;
    public Material offlight, onlight;
    public AudioSource lightSwitchSound;
    public Animator switchAnim;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            inttext.SetActive(true);
            interactable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            inttext.SetActive(false);
            interactable = false;
        }
    }

    void Update()
    {
        if (interactable && Input.GetKeyDown(KeyCode.E))
        {
            ToggleLight();
        }
    }

    void ToggleLight()
    {
        toggle = !toggle;

        if (lightSwitchSound != null)
        {
            lightSwitchSound.Play();
        }
        else
        {
            Debug.LogWarning("Light switch sound is not assigned!");
        }

        switchAnim.ResetTrigger("press");
        switchAnim.SetTrigger("press");
        UpdateLightState();
    }

    void UpdateLightState()
    {
        if (toggle)
        {
            light.SetActive(true);
            lightBulb.material = onlight;
        }
        else
        {
            light.SetActive(false);
            lightBulb.material = offlight;
        }
    }
}