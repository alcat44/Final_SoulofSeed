using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]

public class SC_FPSController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float crouchSpeed = 3.0f;  // Speed when crouching
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    public AudioClip walkingSound;
    public AudioClip runningSound;
    public AudioClip crouchingSound;

    public GameObject exhaustionText; // Reference to UI Text

    // Head bobbing parameters
    public float bobbingSpeed = 0.18f;
    public float bobbingAmount = 0.05f;
    public float runningBobbingMultiplier = 1.5f;

    // Camera movement parameters
    public Vector3 runCameraOffset = new Vector3(0, 0, 0.2f); // Offset when running
    public Vector3 crouchCameraOffset = new Vector3(0, -0.5f, 0); // Offset when crouching
    public float cameraTransitionSpeed = 5f; // Speed of the transition

    private Vector3 originalCameraPosition; // To store the original camera position

    private float defaultCameraYPos;
    private float timer;

    private CharacterController characterController;
    private AudioSource audioSource;
    private Animator animator;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    private bool canRun = true; // Flag to check if player can run
    private bool isExhausted = false; // Flag to check if player is exhausted
    private bool isCrouching = false; // Flag for crouching
    public float runCooldown = 3f; // Cooldown time after running
    public float runningDuration = 5f; // Max duration the player can run continuously
    private float shiftPressedTime = 0f; // Timer for how long shift is held down
    private float timeSinceLastShiftPress = 0f; // Timer to track time since last shift press
    public float shiftResetTime = 2f; // Time after which shiftPressedTime resets if shift is not pressed

    public Image blackScreen;
    public float fadeDuration = 1f;

    void Start()
    {
        StartCoroutine(FadeInScreen());
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Store the default Y position of the camera
        defaultCameraYPos = playerCamera.transform.localPosition.y;
        originalCameraPosition = playerCamera.transform.localPosition; // Store original camera position

        // Initialize text to be invisible
        exhaustionText.gameObject.SetActive(false);

        // Inisialisasi exhausting dengan aman saat game dimulai
        isExhausted = false;            // Tidak dalam kondisi exhausted saat start
        shiftPressedTime = 0f;          // Reset waktu shift pressed di awal
        timeSinceLastShiftPress = 0f;   // Reset waktu terakhir shift ditekan di awal
    }

    public void ResetPlayerMovement()
    {
        isExhausted = false;  // Reset kondisi exhaustion
        shiftPressedTime = 0f;  // Reset timer shift pressed
        timeSinceLastShiftPress = 0f;  // Reset waktu sejak shift terakhir ditekan
        exhaustionText.gameObject.SetActive(false);  // Sembunyikan teks exhaustion

        // Reset posisi kamera ke posisi semula
        playerCamera.transform.localPosition = originalCameraPosition;
    }


     IEnumerator FadeInScreen()
    {
        float elapsedTime = 0f;
        Color color = blackScreen.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration); // Fade out
            blackScreen.color = color;
            yield return null;
        }

        color.a = 0f; // Pastikan alpha tepat di 0
        blackScreen.color = color;
    }

    void Update()
    {
        // Handle crouch input
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            // Mulai crouch
            StartCrouching();
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            // Berhenti crouch
            StopCrouching();
        }

        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Check if player is pressing shift and running is allowed
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && canRun && !isExhausted && !isCrouching; // No running while crouching

        if (Input.GetKey(KeyCode.LeftShift) && !isExhausted && !isCrouching)
        {
            shiftPressedTime += Time.deltaTime; // Increment the time shift is held down
            timeSinceLastShiftPress = 0f; // Reset the "since last press" timer

            // If shift is held down for longer than the runningDuration, trigger exhaustion
            if (shiftPressedTime >= runningDuration)
            {
                StartCoroutine(ExhaustionCooldown());
            }
        }
        else
        {
            timeSinceLastShiftPress += Time.deltaTime; // Increment time since last shift press

            // If Shift hasn't been pressed for 2 seconds, reset the shiftPressedTime
            if (timeSinceLastShiftPress >= shiftResetTime)
            {
                shiftPressedTime = 0f;
            }
        }

        // Adjust speed based on crouch or running state
        float curSpeedX = canMove ? (isCrouching ? crouchSpeed : (isRunning ? runningSpeed : walkingSpeed)) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isCrouching ? crouchSpeed : (isRunning ? runningSpeed : walkingSpeed)) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded && !isCrouching) // No jumping while crouching
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        // Apply camera movement (crouching or running)
        UpdateCameraPosition(isRunning, isCrouching);

        // Apply head bobbing effect
        ApplyHeadBobbing(isRunning, isCrouching);

        // Handle sound effects
        HandleMovementSound(curSpeedX, curSpeedY, isRunning);

        // Update animator parameters
        float speed = new Vector3(characterController.velocity.x, 0, characterController.velocity.z).magnitude;
        animator.SetFloat("Speed", speed);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsIdle", speed == 0 && !isCrouching); // IsIdle is true only when not crouching and stopped

        // Update exhaustion text visibility
        if (isExhausted && !exhaustionText.gameObject.activeSelf)
        {
            exhaustionText.gameObject.SetActive(true); // Show text when tired
        }
        else if (!isExhausted && exhaustionText.gameObject.activeSelf)
        {
            exhaustionText.gameObject.SetActive(false); // Hide text when not tired
        }
    }

    void StartCrouching()
    {
        isCrouching = true;
        animator.SetBool("IsCrouching", true);  // Set animator crouching state
    }

    void OnEnable()
    {
        if (isCrouching)
        {
            StopCrouching(); // Reset crouch state and make the player stand up
        }
    }

    void StopCrouching()
    {
        isCrouching = false;
        animator.SetBool("IsCrouching", false); // Reset crouching state in the animator

        // Reset camera position to the original standing position
        playerCamera.transform.localPosition = Vector3.Lerp(
            playerCamera.transform.localPosition,
            originalCameraPosition,
            Time.deltaTime * cameraTransitionSpeed
        );
    }


   void UpdateCameraPosition(bool isRunning, bool isCrouching)
    {
        // Shift camera forward when running, and lower it when crouching
        if (isRunning)
        {
            // Transition camera position when running
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition, 
                originalCameraPosition + runCameraOffset, 
                Time.deltaTime * cameraTransitionSpeed
            );
        }
        else if (isCrouching)
        {
            // Lower the camera when crouching (affecting all X, Y, Z axes)
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                originalCameraPosition + crouchCameraOffset, // Use all X, Y, Z for crouch offset
                Time.deltaTime * cameraTransitionSpeed
            );
        }
        else
        {
            // Return camera to original position when not running or crouching
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition, 
                originalCameraPosition, 
                Time.deltaTime * cameraTransitionSpeed
            );
        }
    }

    void ApplyHeadBobbing(bool isRunning, bool isCrouching)
    {
        // Only apply head bobbing when the player is not crouching
        if (!isCrouching && characterController.velocity.magnitude > 0 && characterController.isGrounded)
        {
            // Calculate the bobbing frequency (faster if running)
            float adjustedBobbingSpeed = isRunning ? bobbingSpeed * runningBobbingMultiplier : bobbingSpeed;

            // Bobbing effect
            timer += Time.deltaTime * adjustedBobbingSpeed;
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultCameraYPos + Mathf.Sin(timer) * bobbingAmount,
                playerCamera.transform.localPosition.z
            );
        }
        else
        {
            // Reset the timer when the player is not moving or is crouching
            timer = 0;

            // When crouching, maintain the camera's crouch position
            if (isCrouching)
            {
                playerCamera.transform.localPosition = new Vector3(
                    playerCamera.transform.localPosition.x,
                    originalCameraPosition.y + crouchCameraOffset.y,
                    playerCamera.transform.localPosition.z
                );
            }
            else
            {
                // Reset to default Y position when not crouching or moving
                playerCamera.transform.localPosition = new Vector3(
                    playerCamera.transform.localPosition.x,
                    defaultCameraYPos,
                    playerCamera.transform.localPosition.z
                );
            }
        }
    }

    void HandleMovementSound(float curSpeedX, float curSpeedY, bool isRunning)
    {
        if (characterController.isGrounded && (curSpeedX != 0 || curSpeedY != 0))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = isCrouching ? crouchingSound : (isRunning ? runningSound : walkingSound);
                audioSource.Play();
            }
            else if (audioSource.clip == walkingSound && isRunning)
            {
                audioSource.clip = runningSound;
                audioSource.Play();
            }
            else if (audioSource.clip == runningSound && !isRunning)
            {
                audioSource.clip = walkingSound;
                audioSource.Play();
            }
            else if (audioSource.clip == walkingSound && isCrouching)
            {
                audioSource.clip = crouchingSound;
                audioSource.Play();
            }
        }
        else if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // Coroutine to handle running cooldown
    IEnumerator ExhaustionCooldown()
    {
        isExhausted = true; // Set player to exhausted
        yield return new WaitForSeconds(runCooldown); // Wait for cooldown duration
        isExhausted = false; // Re-enable running after cooldown
    }

}
