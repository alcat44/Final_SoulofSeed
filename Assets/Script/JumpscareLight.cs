using System.Collections;
using UnityEngine;

public class JumpscareLight : MonoBehaviour
{
    public AudioSource scareSound; // Sumber suara jumpscare
    public Collider triggerCollider; // Collider untuk mendeteksi trigger
    public GameObject jumpscareObject; // Objek jumpscare yang akan diaktifkan
    public GameObject lightObject; // Lampu yang akan dimatikan
    public Renderer lightBulbRenderer; // Renderer dari bohlam lampu
    public Material offLightMaterial; // Material untuk bohlam lampu yang mati

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jumpscareObject.SetActive(true); // Aktifkan objek jumpscare
            triggerCollider.enabled = false; // Nonaktifkan trigger collider
            TurnOffLight(); // Matikan lampu
            PlayScareSound(); // Mainkan suara jumpscare
            StartCoroutine(DisableJumpscareAfterDelay(4.0f)); // Coroutine untuk menonaktifkan jumpscare setelah delay
        }
    }

    void TurnOffLight()
    {
        if (lightObject != null)
        {
            lightObject.SetActive(false); // Nonaktifkan lampu
            if (lightBulbRenderer != null && offLightMaterial != null)
            {
                lightBulbRenderer.material = offLightMaterial; // Ganti material bohlam lampu menjadi mati
            }

            RenderSettings.fog = false; // Matikan fog
        }
    }

    void PlayScareSound()
    {
        if (scareSound != null)
        {
            scareSound.Play(); // Mainkan suara jumpscare
        }
    }

    IEnumerator DisableJumpscareAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Tunggu selama delay
        jumpscareObject.SetActive(false); // Nonaktifkan objek jumpscare setelah delay
    }

    void DisableTrigger()
    {
        if (triggerCollider != null)
        {
            triggerCollider.enabled = false; // Menonaktifkan trigger
        }
    }
}
