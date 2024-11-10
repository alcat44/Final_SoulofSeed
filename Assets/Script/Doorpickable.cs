using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doorpickable : MonoBehaviour
{
    public GameObject intText;
    public bool interactable, toggle;
    public Animator doorAnim;
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;
    public MonoBehaviour Object;  // Pickable object
    public Collider trigger;      // Trigger collider
    public Collider doorCollider; // Collider pintu pertama
    public Collider doorCollider2; // Collider pintu kedua
    public MonoBehaviour ObjectDidalem;

    // Reference to the PickUpController script on the item
    private PickUpController pickUpController;

    void Start()
    {
         if (Object != null)
        {
            // Find the PickUpController component (assuming the object this script controls has it)
            pickUpController = Object.GetComponent<PickUpController>();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(true);
            interactable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false);
            interactable = false;
        }
    }

    void Update()
    {
        if (interactable == true && Input.GetKeyDown(KeyCode.E))
        {
            // Set interactable and intText to false when pressing "E"
            interactable = false;
            intText.SetActive(false);

            toggle = !toggle;
            if (toggle == true)
            {
                StartCoroutine(OpenDoor());
            }
            else
            {
                StartCoroutine(CloseDoor());
            }
        }
    }

    IEnumerator OpenDoor()
    {
        doorAnim.ResetTrigger("close");
        doorAnim.SetTrigger("open");
        audioSource.PlayOneShot(openSound);

        // Nonaktifkan collider selama animasi berjalan
        if (doorCollider != null) doorCollider.enabled = false;
        if (doorCollider2 != null) doorCollider2.enabled = false;

        // Tunggu hingga animasi selesai
        yield return new WaitForSeconds(doorAnim.GetCurrentAnimatorStateInfo(0).length);

        // Hanya modifikasi Object dan trigger jika objek belum pernah di-pick
        if (pickUpController != null && !pickUpController.wasPicked)
        {
            if (Object != null) Object.enabled = true;
            if (trigger != null) trigger.enabled = true;
        }

        if (ObjectDidalem != null) ObjectDidalem.enabled = true;

        // Aktifkan kembali collider setelah animasi selesai
        if (doorCollider != null) doorCollider.enabled = true;
        if (doorCollider2 != null) doorCollider2.enabled = true;

        toggle = true;
    }

    IEnumerator CloseDoor()
    {
        doorAnim.ResetTrigger("open");
        doorAnim.SetTrigger("close");
        audioSource.PlayOneShot(closeSound);

        // Nonaktifkan collider selama animasi berjalan
        if (doorCollider != null) doorCollider.enabled = false;
        if (doorCollider2 != null) doorCollider2.enabled = false;

        // Tunggu hingga animasi selesai
        yield return new WaitForSeconds(doorAnim.GetCurrentAnimatorStateInfo(0).length);

        // Hanya modifikasi Object dan trigger jika objek belum pernah di-pick
        if (pickUpController != null && !pickUpController.wasPicked)
        {
            if (Object != null) Object.enabled = false;
            if (trigger != null) trigger.enabled = false;
        }

        if (ObjectDidalem != null) ObjectDidalem.enabled = false;

        // Aktifkan kembali collider setelah animasi selesai
        if (doorCollider != null) doorCollider.enabled = true;
        if (doorCollider2 != null) doorCollider2.enabled = true;

        toggle = false;
    }

    public void ResetInteraction()
    {
        intText.SetActive(false); // Make sure interaction text is off
        interactable = false;     // Reset interactable flag
        Debug.Log("Interaction reset after player respawn.");
    }
}
