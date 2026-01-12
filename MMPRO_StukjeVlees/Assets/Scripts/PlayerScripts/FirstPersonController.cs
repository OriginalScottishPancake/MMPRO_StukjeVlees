using System;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement speed")]
    [SerializeField] private float walkspeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header("Jump parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 2.0f;

    [Header("Look parameters")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("Crouch parameters")]
    [SerializeField] private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    [SerializeField] private float crouchTransitionSpeed = 10f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;

    [Header("Phone references")]
    [SerializeField] private GameObject flashlightToggle;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;

    private Vector3 currentMovement;
    private float verticalRotation;

    private float originalControllerHeight;
    private Vector3 originalControllerCenter;
    private Vector3 originalCameraLocalPos;
    private bool isCrouching;

    // Checks if the SprintTriggered is true to apply sprint multiplier, if false, then 1 is applied which means normal speed -> Crouch effects movement speed
    private float currentSpeed => walkspeed * (playerInputHandler.SprintTriggered &&!isCrouching && playerInputHandler.MovementInput.y >=0  ? sprintMultiplier : 1) * (isCrouching ? crouchSpeedMultiplier : 1f);

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (characterController == null) characterController = GetComponent<CharacterController>();
        originalControllerHeight = characterController.height;
        originalControllerCenter = characterController.center;
        originalCameraLocalPos = mainCamera.transform.localPosition;


    }
    void Update()
    {
        HandleCrouch();
        HandleMovement();
        HandleRotation();
    }

    private Vector3 CalculateWorldDiraction()
    {
        Vector3 inputDirection = new Vector3(playerInputHandler.MovementInput.x, 0f, playerInputHandler.MovementInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        return worldDirection.normalized;
    }

    private void HandleJumping()
    {
        if (characterController.isGrounded && !isCrouching)
        {
            currentMovement.y = -0.5f;

            if (playerInputHandler.JumpTriggered)
            {
                currentMovement.y = jumpForce;
            }
        }

        else
        {
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        Vector3 worldDirection = CalculateWorldDiraction();
        currentMovement.x = worldDirection.x * currentSpeed;
        currentMovement.z = worldDirection.z * currentSpeed;

        HandleJumping();
        characterController.Move(currentMovement * Time.deltaTime);

    }

    private void ApplyHorizonalRotation(float rotationAmount)
    {
        transform.Rotate(0, rotationAmount, 0);
    }

    private void ApplyVerticalRotation(float rotationAmount)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleRotation()
    {
        float mouseXRotation = playerInputHandler.RotationInput.x * mouseSensitivity;
        float mouseYRotation = playerInputHandler.RotationInput.y * mouseSensitivity;

        ApplyHorizonalRotation(mouseXRotation);
        ApplyVerticalRotation(mouseYRotation);
    }

    private void HandleCrouch()
    {
        bool wantCrouch = playerInputHandler.CrouchTriggered;
        isCrouching = wantCrouch;

        float targetHeight = wantCrouch
            ? originalControllerHeight * crouchScale.y : originalControllerHeight;

        Vector3 targetCenter = wantCrouch
            ? originalControllerCenter * crouchScale.y : originalControllerCenter;

        Vector3 targetCamPos = wantCrouch
            ? originalCameraLocalPos * crouchScale.y : originalCameraLocalPos;

        characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);

        characterController.center = Vector3.Lerp(characterController.center, targetCenter, Time.deltaTime * crouchTransitionSpeed);

        mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, targetCamPos, Time.deltaTime * crouchTransitionSpeed);

    }
}
