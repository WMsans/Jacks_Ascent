#if UNITY_EDITOR
using UnityEngine;

public class EditorController : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] private ClimbingController climbingController;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Transform playerHead;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float climbSpeed = 2f;

    [Header("Grabbing")]
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private float grabCheckDistance = 2f;
    [SerializeField] private LayerMask grabableLayer;


    private float verticalLookRotation;
    private float horizontalLookRotation;

    void Start()
    {
        if (Application.isEditor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (Application.isEditor)
        {
            HandleMouseLook();
            HandleMovement();
            HandleGrabbing();
            HandleAttack();
        }
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Accumulate the horizontal and vertical mouse input
        horizontalLookRotation += mouseX;
        verticalLookRotation -= mouseY;

        // Clamp the vertical rotation to prevent the camera from flipping over
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        // Apply both rotations to the head's local rotation at once
        playerHead.localRotation = Quaternion.Euler(verticalLookRotation, horizontalLookRotation, 0f);
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (climbingController.LeftHandHeldJug == null && climbingController.RightHandHeldJug == null)
        {
            // Grounded movement
            Vector3 moveDirection = playerHead.forward * verticalInput + playerHead.right * horizontalInput;
            playerRigidbody.linearVelocity = new Vector3(moveDirection.x * walkSpeed, playerRigidbody.linearVelocity.y, moveDirection.z * walkSpeed);
        }
        else
        {
            // Climbing movement
            Vector3 moveDirection = playerHead.transform.forward * verticalInput + playerHead.transform.right * horizontalInput;
            if (Input.GetKey(KeyCode.Space))
            {
                moveDirection += Vector3.up;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveDirection += Vector3.down;
            }

            if(climbingController.LeftHandHeldJug != null)
            {
                leftHand.position -= moveDirection.normalized * climbSpeed * Time.deltaTime;
            }

            if (climbingController.RightHandHeldJug != null)
            {
                rightHand.position -= moveDirection.normalized * climbSpeed * Time.deltaTime;
            }
        }
    }

    private void HandleGrabbing()
    {
        // We only need to do a raycast if the player presses a button to start a grab.
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            // Center of the screen for grabbing
            if (Physics.Raycast(playerHead.position, playerHead.forward, out var grabPoint, grabCheckDistance,
                    grabableLayer, QueryTriggerInteraction.Ignore))
            {
                // If Left Mouse Button is pressed and the hand is free, grab.
                if (Input.GetMouseButtonDown(0) && climbingController.LeftHandHeldJug == null)
                {
                    leftHand.position = grabPoint.point;
                    climbingController.EditorTryGrab(leftHand, true);
                }

                // If Right Mouse Button is pressed and the hand is free, grab.
                if (Input.GetMouseButtonDown(1) && climbingController.RightHandHeldJug == null)
                {
                    rightHand.position = grabPoint.point;
                    climbingController.EditorTryGrab(rightHand, false);
                }
            }
        }

        // If Left Mouse Button is released and the hand is holding something, release.
        if (Input.GetMouseButtonUp(0) && climbingController.LeftHandHeldJug != null)
        {
            climbingController.ForceRelease(leftHand);
        }

        // If Right Mouse Button is released and the hand is holding something, release.
        if (Input.GetMouseButtonUp(1) && climbingController.RightHandHeldJug != null)
        {
            climbingController.ForceRelease(rightHand);
        }
    }

    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Prioritize right hand attack, then left hand
            if (weaponController.IsHandHoldingWeapon(rightHand))
            {
                weaponController.EditorAttack(false);
            }
            else if (weaponController.IsHandHoldingWeapon(leftHand))
            {
                weaponController.EditorAttack(true);
            }
        }
    }
}
#endif