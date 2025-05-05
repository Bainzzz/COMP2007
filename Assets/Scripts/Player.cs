using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ensures a CharacterController component is attached to the GameObject
[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    
    public Camera playerCamera;
    public float walkSpeed = 5f; //player speed
    public float runSpeed = 8f;
    public float maxStamina = 100f; //maximum stamina 
    public float staminaDrainRate = 20f; //stamina drained per second while sprinting
    public float staminaRegenRate = 10f; //stamina regenerated per second when not sprinting
    public float regenDelay = 5f; //5-second delay before stamina regeneration starts
    [SerializeField] private float currentStamina; //current stamina (visible in Inspector)
    private float timeSinceLastSprint; //tracks time since last sprint attempt

    public float lookSpeed = 2f;  //camera sensitivy 
    public float lookXLimit = 45f;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    public bool canMove = true;

    CharacterController characterController; //ref to character controller

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;          //lock and hide cursor 
        Cursor.visible = false;

        currentStamina = maxStamina;  //initialize stamina to full
        timeSinceLastSprint = regenDelay; 
    }

    void Update()
    {
        #region Movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);  //get player's forward and right directions in world space
        Vector3 right = transform.TransformDirection(Vector3.right);

        //check if player wants to sprint and has enough stamina
        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift);  //left shift to sprint
        bool canSprint = currentStamina > 0; //must have stamina to sprint
        bool isRunning = wantsToSprint && canSprint && canMove; //sprint if all conditions met

        if (wantsToSprint && canSprint && canMove)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            timeSinceLastSprint = 0; 
            Debug.Log($"Sprinting! Stamina: {currentStamina:F1}/{maxStamina}");  //DEBUG.REMEMBER TO ADD ACTUAL HUD LATER
        }
        else
        {
            timeSinceLastSprint += Time.deltaTime;
            if (currentStamina < maxStamina && timeSinceLastSprint >= regenDelay)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;        //regen stam after delay
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
                Debug.Log($"Regenerating stamina: {currentStamina:F1}/{maxStamina}");  //DEBUG
            }
        }

        if (wantsToSprint && !canSprint)
        {
            Debug.Log("Cannot sprint: Stamina depleted!");  //DEBUG
        }

        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;  //binds W and S 
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0; //binds A and D
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        #endregion

        #region Turning
        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;  //if mouse up/down player looks up/down
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);  //if mouse looks left/right player looks left/right
        }
        #endregion
    }
}