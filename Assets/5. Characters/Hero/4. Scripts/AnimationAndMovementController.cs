using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class AnimationAndMovementController : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;
    public Animator animator;

    int isWalkingHash;
    int isRunningHash;
    int isJumpingHash;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 appliedMovement;
    bool isMovementPressed;
    bool isRunPressed;
    [SerializeField] float rotationFactorPerFrame = 15.0f;
    [SerializeField] float runMultiplier = 3f;
    [SerializeField] float moveSpeedMultiplayer = 1f;

    float groundedGravity = -.5f;
    float gravity = -9.8f;

    bool isJumpPressed = false;
    float initialJumpVelocity;
    [SerializeField] float maxJumpHeight = 2.0f;
    [SerializeField] float maxJumpTime = 1.0f;
    bool isJumping = false;
    bool isJumpingAnimating = false;

    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");

        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;

        playerInput.CharacterControls.Run.started += OnRun;
        playerInput.CharacterControls.Run.canceled += OnRun;

        playerInput.CharacterControls.Jump.started += OnJump;
        playerInput.CharacterControls.Jump.canceled += OnJump;

        SetupJumpVariables();
    }

    void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void HandleJump()
    {
        if (isJumping == false && characterController.isGrounded == true && isJumpPressed == true)
        {
            animator.SetBool(isJumpingHash, true);
            isJumpingAnimating = true;
            isJumping = true;
            currentMovement.y = initialJumpVelocity;
            appliedMovement.y = initialJumpVelocity;
            //float previousYVelocity = currentMovement.y;
            //float newYVelocity = currentMovement.y + initialJumpVelocity;
            //float nextYvelocity = (previousYVelocity + newYVelocity) * .5f;
            //////currentMovement.y = initialJumpVelocity;
            //////currentRunMovement.y = initialJumpVelocity;
            //currentMovement.y = nextYvelocity;
            //currentRunMovement.y = nextYvelocity;
        }
        else if (isJumpPressed == false && isJumping == true && characterController.isGrounded == true)
        {
            isJumping = false;
        }
    }

    void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    void OnRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }


    public bool deactivateRotation = false;

    void HandleRotation()
    {
        //return;
        
        if (deactivateRotation == true && onMouseCameraRotate == true)
        {

            cameraRelativeMovement = CameraForwardVector(appliedMovement);

            Quaternion currentRotation2 = transform.rotation;

            Quaternion targetRotation = Quaternion.LookRotation(cameraRelativeMovement);
            transform.rotation = Quaternion.Slerp(currentRotation2, targetRotation, rotationFactorPerFrame * Time.deltaTime);

            return;
        }


        Vector3 positionToLookAt;
        if (onMouseCameraRotate == false)
        {
            positionToLookAt.x = currentMovement.x;
            positionToLookAt.y = 0.0f;
            positionToLookAt.z = currentMovement.z;
        }
        //else if (deactivateRotation == true)
        //{
        //    positionToLookAt.x = cameraRelativeMovement.x;
        //    positionToLookAt.y = 0.0f;
        //    positionToLookAt.z = cameraRelativeMovement.z;
        //}
        else
        {
            positionToLookAt.x = cameraRelativeMovement.x;
            positionToLookAt.y = 0.0f;
            positionToLookAt.z = cameraRelativeMovement.z;
        }
        

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed == true)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
        else if (deactivateRotation == true)
        {
            //Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            //transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x * moveSpeedMultiplayer;
        currentMovement.z = currentMovementInput.y * moveSpeedMultiplayer;

        currentRunMovement.x = currentMovementInput.x * runMultiplier * moveSpeedMultiplayer;
        currentRunMovement.z = currentMovementInput.y * runMultiplier * moveSpeedMultiplayer;

        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void HandleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        if (isMovementPressed == true && isWalking == false)
        {
            animator.SetBool(isWalkingHash, true);
        }
        else if (isMovementPressed == false && isWalking == true)
        {
            animator.SetBool(isWalkingHash, false);
        }

        if ((isMovementPressed == true && isRunPressed == true) && isRunning == false)
        {
            animator.SetBool(isRunningHash, true);
        }
        else if ((isMovementPressed == false || isRunPressed == false) && isRunning == true)
        {
            animator.SetBool(isRunningHash, false);
        }
    }

    void HandleGravity()
    {
        bool isFalling = currentMovement.y <= 0f || isJumpPressed == false;
        float fallMultiplier = 2f;

        if (characterController.isGrounded)
        {
            if (isJumpingAnimating == true)
            {
                animator.SetBool(isJumpingHash, false);
                isJumpingAnimating = false;
            }
            currentMovement.y = groundedGravity;
            appliedMovement.y = groundedGravity;
        }
        else if (isFalling == true)
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            appliedMovement.y = Mathf.Max((previousYVelocity + currentMovement.y) * .5f, -20f);
        }
        else
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * Time.deltaTime);
            appliedMovement.y = (previousYVelocity + currentMovement.y) * .5f;
            //currentMovement.y += gravity * Time.deltaTime;
            //currentRunMovement.y += gravity * Time.deltaTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        

        //if (deactivateRotation == false) {  }

        HandleRotation();

        HandleAnimation();
        if (isRunPressed == true)
        {
            //characterController.Move(currentRunMovement * Time.deltaTime);
            appliedMovement.x = currentRunMovement.x;
            appliedMovement.z = currentRunMovement.z;
        }
        else
        {
            appliedMovement.x = currentMovement.x;
            appliedMovement.z = currentMovement.z;
            //characterController.Move(currentMovement * Time.deltaTime);
        }
        //if (isJumping == true)
        //{
        //    characterController.Move(currentRunMovement * Time.deltaTime);
        //}
        //else
        //{
        //    //characterController.Move(currentMovement * Time.deltaTime);
        //}

        //characterController.Move(appliedMovement * Time.deltaTime);

        cameraRelativeMovement = ConvertToCameraSpace(appliedMovement);


        if (isJumping == true)
        {
            if (onMouseCameraRotate == true)
            {
                characterController.Move(cameraRelativeMovement * Time.deltaTime);
            }
            else
            {
                characterController.Move(appliedMovement * Time.deltaTime);
            }
        }
        else
        {
            //characterController.Move(currentMovement * Time.deltaTime);
            if (onMouseCameraRotate == true)
            {
                //transform.forward = ConvertToCameraSpace(appliedMovement);
                characterController.Move(cameraRelativeMovement * Time.deltaTime);
            }
        }

        HandleGravity();
        HandleJump();
    }

    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public bool onMouseCameraRotate = false;
    Vector3 cameraRelativeMovement;


    Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {
        float currentYValue = vectorToRotate.y;

        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        Vector3 cameraForwardZProduct = vectorToRotate.z * cameraForward;
        Vector3 cameraRightXProduct = vectorToRotate.x * cameraRight;

        Vector3 vectorRotateToCameraSpace = cameraForwardZProduct + cameraRightXProduct;
        vectorRotateToCameraSpace.y = currentYValue;

        return vectorRotateToCameraSpace;
    }

    Vector3 CameraForwardVector(Vector3 vectorToRotate)
    {
        float currentYValue = vectorToRotate.y;

        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        Vector3 cameraForwardZProduct = cameraForward;
        Vector3 cameraRightXProduct = cameraRight;

        Vector3 vectorRotateToCameraSpace = cameraForwardZProduct + cameraRightXProduct;
        vectorRotateToCameraSpace.y = currentYValue;

        //cameraForward.y = currentYValue;

        return cameraForward;
    }

    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
