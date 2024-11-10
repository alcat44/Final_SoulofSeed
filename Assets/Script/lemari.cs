using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lemari : MonoBehaviour
{
    public GameObject intText;
    public bool interactable, toggle;
    public Animator lemariAnim;

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("MainCamera"))
        {
            intText.SetActive(true);
            interactable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("MainCamera"))
        {
            intText.SetActive(false);
            interactable = false;
        }
    }

    void Update()
    {
        if(interactable == true)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                toggle = !toggle;
                if(toggle == true)
                {
                    lemariAnim.ResetTrigger("close");
                    lemariAnim.SetTrigger("open");
                }
                if(toggle == false)
                {
                    lemariAnim.ResetTrigger("open");
                    lemariAnim.SetTrigger("close");
                }
                intText.SetActive(false);
                interactable = false;
            }
        }
    }
}
