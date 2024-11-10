using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpscareDoor : MonoBehaviour
{
    public Animator animator; // Animator untuk objek yang akan dianimasikan
    public string animationTriggerName; // Nama parameter trigger di Animator

    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator belum diatur.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerAnimation();
        }
    }

    void TriggerAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(animationTriggerName);
        }
    }
}
