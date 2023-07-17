using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;
using System.Runtime.CompilerServices;

public class FirstPersonController : MonoBehaviour
{
    public bool canMove {get;  set; } = true;
    public bool isSprinting => canSprint && Input.GetKey(sprintKey); // only true if canSprint and pressing key
    public bool shouldJump =>  Input.GetKey(jumpKey) && characterController.isGrounded; // only true if press once, and is on the ground
    public bool shouldCrouch =>  Input.GetKeyDown(crouchKey) && !isInCrouchAnimation && characterController.isGrounded; // only true if not animation and is on ground

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadBob = true;
    [SerializeField] private bool canSlideOnSlopes = true;
    [SerializeField] private bool camZoom = true;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool useFootSteps = true;
    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float slideSpeed = 1.5f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Crouch Parameters")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crounchingCenter = new Vector3(0,0.5f,0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0,0,0);
    private bool isCrouching;
    private bool isInCrouchAnimation;
    
    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 100)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 100)] private float lowerLookLimit = 80.0f;

    [Header("Head Parameters")]
    [SerializeField] private float walkBobSpeed = 14.0f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 10.0f;
    [SerializeField] private float sprintBobAmount = 0.11f;
    [SerializeField] private float crounchBobSpeed = 8.0f;
    [SerializeField] private float crounchBobAmount = 0.025f;
    private float defaultYPos = 0;
    private float timer; // determine where camera need to be verticaly 

    private Camera playerCamera;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector2 currentInput;
    private float rotationX = 0;

    [Header("Zoom parameters")]
    [SerializeField] private float timeToZoom = 0.3f;
    [SerializeField] private float zoomFOV = 30.0f;
    private float defaultFOV;
    private Coroutine zoomRoutine;

    // AUDIO 
    [Header("Footstep parameters")]
    private EventInstance playerFootSteps;
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultiplier = 1.5f;
    [SerializeField] private float sprintStepMultiplier = 0.6f;
    //[SerializeField] private AudioSource footStepAudioSource = default;
    //[SerializeField] private AudioClip[] woodClips = default;
    //[SerializeField] private AudioClip[] metalClips = default;
    //[SerializeField] private AudioClip[] grassClips = default;


    [SerializeField] private List<AudioClip> m_FootstepSounds = new List<AudioClip>();    // list of footstep sounds that will be randomly selected from.
    [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
    [SerializeField] private AudioClip m_LandSound;
    private AudioSource m_AudioSource;
    private FootstepSwapper swapper;

    private float footStepTimer = 0;
    private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultiplier : isSprinting ? baseStepSpeed * sprintStepMultiplier : baseStepSpeed; 

    private const int interactableLayerNumber = 6;

    // SLIDING 
    private Vector3 hitPointNormal; // angle of the floor
    private bool isSliding
    {
        get 
        {
            if (characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
            {
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit;
            }
            else 
            {
                return false;
            } 
        }
    }

    [Header("Interaction")]
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance = default;
    [SerializeField] private LayerMask interactionLayer = default;
    private Interactable currentInteractable;

    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        m_AudioSource = GetComponent<AudioSource>();
        defaultYPos = playerCamera.transform.localPosition.y;
        defaultFOV = playerCamera.fieldOfView;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Start()
    {
        swapper = GetComponent<FootstepSwapper>();
        if (swapper == null) {
            print("i dont see swapper");
        } else
            print(swapper);
        //  playerFootSteps = AudioManager.instance.CreateEventInstance(FMODEvents.instance.footStepsSounds);
    }

    void Update()
    {   
        if (canMove)
        {
            PlayFootStepAudio();
            HandleMovement();
            HandleMouseLook();
            if (canJump && !isSliding)
                HandleJumping();

            if (canCrouch)
                HandleCrouch();        

            if (canUseHeadBob)
                HandleHeadBob();

            if (camZoom)
                HandleZoom();

            if (useFootSteps) {
                PlayFootStepAudio();
            }
       
            if (canInteract)
                HandleInteractionInput();
                HandleInteractionCheck();

            ApplyFinalMovements();
        }
    }

    private void HandleInteractionCheck()
    {
        if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {   
            // check if we got object on Interable layer and we look at same object
            if (hit.collider.gameObject.layer == interactableLayerNumber && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.gameObject.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractable);

                if (currentInteractable)
                    currentInteractable.OnFocus();
            }
        }
        // when we lose focus 
        else if (currentInteractable)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(interactKey) && currentInteractable != null && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer))
        // we hit interactable object 
        {   
            print("Handle interactable input");
            currentInteractable.OnInteract();
        }
    }
    #region Audio
    private void PlayFootStepAudio()
    {
      //  swapper.CheckLayers();
        if (!characterController.isGrounded) {
            return;
        }
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, m_FootstepSounds.Count);
        m_AudioSource.clip = m_FootstepSounds[n];
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
        // move picked sound to index 0 so it's not picked next time
        m_FootstepSounds[n] = m_FootstepSounds[0];
        m_FootstepSounds[0] = m_AudioSource.clip;
    }

        private void PlayLandingSound() {
        swapper.CheckLayers();
        m_AudioSource.clip = m_LandSound;
        m_AudioSource.Play();

    }
    public void SwapFootsteps(FootstepsCollection collection) {
        m_FootstepSounds.Clear();
        for (int i = 0; i < collection.FootstepSounds.Count; i++) {
            m_FootstepSounds.Add(collection.FootstepSounds[i]);

        }
        m_JumpSound = collection.jumpSound;
        m_LandSound = collection.landSound;

    }


    //private void HandleFootSteps()
    //{   // not grounded, not movement we return
    //    if (!characterController.isGrounded) return;
    //    if (currentInput == Vector2.zero) return;

    //    footStepTimer -= Time.deltaTime;
    //    if (footStepTimer <= 0)
    //    {
    //        if (Physics.Raycast(characterController.transform.position, Vector3.down, out RaycastHit hit, 2))
    //        print(hit.collider.tag);
    //        {   
    //            // play audio based on tag of hit raycast
    //            switch (hit.collider.tag)
    //            {
    //                case "FOOTSTEPS/Wood":
    //                    //footStepAudioSource.PlayOneShot(woodClips[Random.Range(0, woodClips.Length - 1)]);
    //                  // AudioManager.instance.PlayeOneShot(FMODEvents.instance.footStepsSounds, characterController.transform.position);
    //                    HandleSoundFootsteps();

    //                      break;
    //                //        case "FOOTSTEPS/Metal":
    //                //            footStepAudioSource.PlayOneShot(metalClips[Random.Range(0, metalClips.Length - 1)]);
    //                //            break;
    //                //        case "FOOTSTEPS/Grass":
    //                //            footStepAudioSource.PlayOneShot(grassClips[Random.Range(0, grassClips.Length - 1)]);
    //                //            break;
    //                default:
    //                    break;
    //            }
    //            }

    //            footStepTimer = GetCurrentOffset;
    //    }

    //}
    //private void HandleSoundFootsteps() {
    //    //// get playback state
    //    if ((characterController.isGrounded && currentInput != Vector2.zero)) {
    //        {
    //            // get the playback state
    //                playerFootSteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(characterController.transform.position));

    //            PLAYBACK_STATE playbackState;
    //            playerFootSteps.getPlaybackState(out playbackState);
    //            if (playbackState.Equals(PLAYBACK_STATE.STOPPED)) {
    //                print("play playerFootSteps");
    //                playerFootSteps.start();
    //            }
    //            // otherwise, stop the footsteps event
    //    else if (!characterController.isGrounded ||  currentInput == Vector2.zero) 
    //                {
    //                playerFootSteps.stop(STOP_MODE.ALLOWFADEOUT);
    //                print("play Stop audio footsteps");
    //            }
    //        }
    //    }
    //}
    #endregion
    private void HandleMovement() 
     {
        currentInput = new Vector2((isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));
        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward * currentInput.x) + transform.TransformDirection(Vector3.right * currentInput.y));
        moveDirection.y = moveDirectionY;
    }
    private void HandleJumping()
    {
        if (shouldJump)
        {
            moveDirection.y = jumpForce;
            PlayLandingSound();
        }
    }
    private void HandleCrouch()
    {
        if (shouldCrouch)
            StartCoroutine(CrounchStand());
    }
    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }
    private void HandleHeadBob()
    {
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
                timer += Time.deltaTime * (isCrouching ? crounchBobSpeed : isSprinting ? sprintBobSpeed : walkBobSpeed);
                playerCamera.transform.localPosition = new Vector3 (
                playerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crounchBobAmount : isSprinting ? sprintBobAmount : walkBobAmount),
                playerCamera.transform.localPosition.z); 
        }
    }
    private void HandleZoom()
    {
        if (Input.GetKeyDown(zoomKey))
        {
            if (zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }
            zoomRoutine = StartCoroutine(ToogleZoom(true));
        }
        if (Input.GetKeyUp(zoomKey))
        {
            if (zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }
            zoomRoutine = StartCoroutine(ToogleZoom(false));
        }   
    }
    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        if (characterController.velocity.y < -1 && characterController.isGrounded)
        {
            moveDirection.y = 0;
        }

        if (canSlideOnSlopes && isSliding)
        {
            moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slideSpeed;
        }
        characterController.Move(moveDirection * Time.deltaTime);
    }
    private IEnumerator CrounchStand()
    {
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            yield break;

        isInCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crounchingCenter;
        Vector3 currentCenter = characterController.center; 

        while (timeElapsed < targetHeight)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed/timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed/timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // make sure we have exact values every time
        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        isInCrouchAnimation = false;
    }
    private IEnumerator ToogleZoom(bool isEnter)
    {
        float targetFOV = isEnter ? zoomFOV : defaultFOV;
        float startingFOV = playerCamera.fieldOfView;
        float timeElapsed = 0;

        while (timeElapsed < timeToZoom)
        {
            playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed/timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.fieldOfView = targetFOV;
        zoomRoutine = null;
    
    }

}
