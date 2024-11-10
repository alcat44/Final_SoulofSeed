using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public int checkpointIndex;
    private AINavigationall aiNavigation;

    void Start()
    {
        aiNavigation = FindObjectOfType<AINavigationall>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            aiNavigation.SetCheckpoint(checkpointIndex);
            Debug.Log("Checkpoint " + checkpointIndex + " tercapai!");
        }
    }
}
