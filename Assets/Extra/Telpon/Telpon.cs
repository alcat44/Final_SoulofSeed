using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;

public class Telpon : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public GameObject intText;
    public string[] lines;
    public float textSpeed;
    private int index;
    public bool interactable = false;
    private bool isDialogActive = false;
    private bool hasInteracted = false;

    public MonoBehaviour SC_FPSController;
    public Pause pauseScript;
    public MonoBehaviour door;
    public AudioSource audioSource;
    public AudioSource audioSource2;
    public AudioClip LockedSound;
    public GameObject DialogBG;
    
    public phonering phoneRingScript;
    public AudioClip[] SpeakNoises = new AudioClip[0];
    public AudioSource gibberishSource;
    public float gibberishPitch = 1.0f;
    private bool isGibberishPlaying = false;

    public BoxCollider boxCollider;
    public PlayableDirector timeline;

    public Camera mainCamera;
    public Camera cutsceneCamera;
    private bool cutscenePlaying = false;
    private bool canSkip = false;

    public Transform playerTransform;
    public Vector3 targetPosition;
    public Quaternion targetRotation;
    public Transform targetTransform;

    public GameObject bapakandihilang;
    public GameObject gembokhilang;
    public GameObject gembokawal;

    void Start()
    {
        textComponent.text = string.Empty;
        intText.SetActive(false);
        gembokawal.SetActive(false);

        if (boxCollider == null)
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        timeline.stopped += OnTimelineStopped;
    }

    void Update()
    {
        if (interactable && !isDialogActive && !hasInteracted && Input.GetKeyDown(KeyCode.E))
        {
            StartDialog();
        }

        if (isDialogActive && Input.GetKeyDown(KeyCode.E))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
                StopGibberish();
            }
        }

        if (cutscenePlaying && canSkip && Input.GetKeyDown(KeyCode.E))
        {
            SkipTimeline();
        }
    }

    void StartDialog()
    {
        playerTransform.position = targetPosition;
        playerTransform.rotation = targetRotation;
        index = 0;
        isDialogActive = true;
        hasInteracted = true;
        SC_FPSController.enabled = false;
        audioSource.enabled = false;
        intText.SetActive(false);
        audioSource2.PlayOneShot(LockedSound);
        DialogBG.SetActive(true);
        
        if (pauseScript != null) pauseScript.enabled = false;
        
        if (phoneRingScript != null && phoneRingScript.phoneRingSound.isPlaying)
        {
            phoneRingScript.phoneRingSound.Stop();
        }

        StartCoroutine(TypeLine());

        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }
    }
    
    IEnumerator TypeLine()
    {
        textComponent.text = "";

        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            PlayRandomGibberish();
            
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialog();
        }
    }

    void EndDialog()
    {
        isDialogActive = false;
        textComponent.text = string.Empty;
        DialogBG.SetActive(false);
        
        PlayTimeline();
    }

    void PlayTimeline()
    {
        if (timeline != null)
        {
            StartCoroutine(PlayCutscene());
        }
    }

    IEnumerator PlayCutscene()
    {
        playerTransform.gameObject.SetActive(false);
        cutscenePlaying = true;
        timeline.Play();

        cutsceneCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);

        SC_FPSController.enabled = false;
        audioSource.enabled = false;
        yield return new WaitForSeconds(5f);  // Time delay before allowing skip

        canSkip = true;
    }

    void SkipTimeline()
    {
        timeline.time = timeline.duration;
        timeline.Evaluate();
        EndCutscene();
    }

    void EndCutscene()
    {
        cutscenePlaying = false;
        canSkip = false;

        cutsceneCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);

        if (targetTransform != null)
        {
            // Ubah posisi dan rotasi player sesuai dengan targetTransform
            playerTransform.position = targetTransform.position;
            playerTransform.rotation = targetTransform.rotation;
        }
        
        playerTransform.gameObject.SetActive(true);

        SC_FPSController.enabled = true;
        audioSource.enabled = true;

        if (bapakandihilang != null)
        {
            bapakandihilang.SetActive(false);
        }

        if (gembokhilang != null)
        {
            gembokhilang.SetActive(false);
        }

        if (gembokawal != null)
        {
            gembokawal.SetActive(true);
        }
    }

    void PlayRandomGibberish()
    {
        if (SpeakNoises.Length > 0 && !isGibberishPlaying)
        {
            AudioClip randomClip = SpeakNoises[Random.Range(0, SpeakNoises.Length)];
            gibberishSource.pitch = gibberishPitch;
            gibberishSource.PlayOneShot(randomClip);
            isGibberishPlaying = true;
            StartCoroutine(ResetGibberishPlaying(randomClip.length));
        }
    }

    private IEnumerator ResetGibberishPlaying(float duration)
    {
        yield return new WaitForSeconds(duration);
        isGibberishPlaying = false;
    }
    
    private void OnTimelineStopped(PlayableDirector director)
    {
        EndCutscene();
    }

    void StopGibberish()
    {
        gibberishSource.Stop();
        isGibberishPlaying = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera") && !hasInteracted)
        {
            if (!isDialogActive)
            {
                intText.SetActive(true);
                interactable = true;
                if (door != null) door.enabled = false;
            }
        }                                                                           
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false);
            interactable = false;
            if (door != null) door.enabled = true;
        }
    }

    public void ResetDialog()
    {
        // Reset dialog UI and interaction states
        textComponent.text = string.Empty; // Clear any active dialog text
        intText.SetActive(false); // Hide interaction prompt
        interactable = false; // Reset interaction flag
        isDialogActive = false; // Ensure dialog is not active
        DialogBG.SetActive(false); // Hide dialog background
        StopGibberish(); // Stop any gibberish audio
    }
}
