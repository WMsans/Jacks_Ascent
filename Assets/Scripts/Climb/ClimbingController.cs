using System.Collections.Generic;
using System.Linq;
using MEC;
using UnityEngine;
using UnityEngine.Events; 

[RequireComponent(typeof(Rigidbody))]
public class ClimbingController : MonoBehaviour
{
    [Header("Hand Tracking")]
    [SerializeField] public Transform leftHand; // Made public
    [SerializeField] public Transform rightHand; // Made public
    [SerializeField] private OVRInput.Controller leftHandController = OVRInput.Controller.LTouch;
    [SerializeField] private OVRInput.Controller rightHandController = OVRInput.Controller.RTouch;

    [Header("Player")]
    [SerializeField] private Transform playerCameraRig;
    [SerializeField] public Rigidbody playerRigidbody; // Made public
    [SerializeField] public Camera playerHead; // Made public
    [SerializeField] private Collider playerCollider;
    [SerializeField] private WeaponController weaponController;

    [Header("Climbing Settings")]
    [SerializeField] private float grabDistance = 0.1f;
    [SerializeField] private LayerMask climbableLayer;

    [Header("Coyote Time")]
    [Tooltip("The time window (in seconds) where a grab is still registered after pressing the button or leaving a hold.")]
    [SerializeField] private float grabBufferTime = 0.2f;

    [Header("Gravity")]
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float momentumMultiplier;
    
    [Header("Events")]
    public UnityEvent OnLeftGrabbed;
    public UnityEvent OnLeftReleased;
    public UnityEvent OnRightGrabbed;
    public UnityEvent OnRightReleased;

    private IClimbable leftHandHeldJug = null;
    public IClimbable LeftHandHeldJug { get { return leftHandHeldJug; } }
    private IClimbable rightHandHeldJug = null;
    public IClimbable RightHandHeldJug { get { return rightHandHeldJug; } }
    private Vector3 leftHandGrabOffset;
    private Vector3 rightHandGrabOffset;

    private IClimbable activeHeldJug = null;
    private Transform activeHand = null;
    private Vector3 activeGrabOffset = Vector3.zero;

    private Vector3 climbVelocity = Vector3.zero;

    private bool calibrated = false;

    // --- NEW: Coyote Time State Variables ---
    private float leftGrabIntentTimer;
    private float rightGrabIntentTimer;
    private IClimbable leftPotentialJug;
    private IClimbable rightPotentialJug;


    void Awake()
    {
        if (!playerRigidbody) playerRigidbody = playerCameraRig.GetComponent<Rigidbody>();
        if (!playerHead) playerHead = playerCameraRig.GetComponentInChildren<Camera>();
        if (!playerCollider) playerCollider = playerCameraRig.GetComponentInChildren<Collider>();
        if (!weaponController) weaponController = GetComponent<WeaponController>();
        
        playerRigidbody.useGravity = false;
    }

    private void OnEnable()
    {
        calibrated = false;
    }

    private void Start()
    {
        Timing.RunCoroutine(InitializePlayerPositionCoroutine());
    }

    void Update()
    {
        // 1. Continuously find what each hand *could* grab.
        FindPotentialJugs();

        // 2. Register grab intention when the button is pressed, starting a timer.
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, leftHandController))
        {
            leftGrabIntentTimer = grabBufferTime;
        }
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, rightHandController))
        {
            rightGrabIntentTimer = grabBufferTime;
        }
        
        // 3. Tick down the intention timers each frame.
        if(leftGrabIntentTimer > 0) leftGrabIntentTimer -= Time.deltaTime;
        if(rightGrabIntentTimer > 0) rightGrabIntentTimer -= Time.deltaTime;

        // 4. Check if a grab should be executed.
        // Condition: Intent is active, a potential jug is near, and the hand isn't already holding something.
        if (leftGrabIntentTimer > 0 && leftPotentialJug != null && leftHandHeldJug == null)
        {
            ExecuteGrab(leftHand, leftPotentialJug, ref leftHandHeldJug, ref leftHandGrabOffset, ref rightHandHeldJug);
            leftGrabIntentTimer = 0f; // Consume the intent
        }
        
        if (rightGrabIntentTimer > 0 && rightPotentialJug != null && rightHandHeldJug == null)
        {
            ExecuteGrab(rightHand, rightPotentialJug, ref rightHandHeldJug, ref rightHandGrabOffset, ref leftHandHeldJug);
            rightGrabIntentTimer = 0f; // Consume the intent
        }

        // 5. Handle releasing (this logic is unchanged).
        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, leftHandController))
        {
            ReleaseHand(ref leftHandHeldJug, true);
        }
        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, rightHandController))
        {
            ReleaseHand(ref rightHandHeldJug, false);
        }

        // 6. Update the active hand for climbing movement (unchanged).
        UpdateActiveHand();
    }

    // --- NEW: Method to find potential climbable objects near hands ---
    private void FindPotentialJugs()
    {
        // Find the closest valid IClimbable for the left hand
        Collider[] leftColliders = Physics.OverlapSphere(leftHand.position, grabDistance, climbableLayer);
        leftPotentialJug = leftColliders.Length > 0 ? 
            leftColliders.FirstOrDefault(c => c.GetComponent<IClimbable>() != null)?.GetComponent<IClimbable>() : 
            null;

        // Find the closest valid IClimbable for the right hand
        Collider[] rightColliders = Physics.OverlapSphere(rightHand.position, grabDistance, climbableLayer);
        rightPotentialJug = rightColliders.Length > 0 ?
            rightColliders.FirstOrDefault(c => c.GetComponent<IClimbable>() != null)?.GetComponent<IClimbable>() :
            null;
    }

    private void ExecuteGrab(Transform hand, IClimbable jugToGrab, ref IClimbable thisHandJug, ref Vector3 thisHandOffset, ref IClimbable otherHandJug)
    {
        if (weaponController != null && weaponController.IsHandHoldingWeapon(hand))
        {
            return;
        }
        
        thisHandJug = jugToGrab;
        thisHandJug.OnGrab(hand);
        
        // Invoke the correct grab event
        if (hand == leftHand) OnLeftGrabbed?.Invoke();
        else if (hand == rightHand) OnRightGrabbed?.Invoke();

        thisHandOffset = hand.position - thisHandJug.transform.position;
        playerRigidbody.isKinematic = true;

        if (otherHandJug != null)
        {
            if (hand == leftHand) OnRightReleased?.Invoke();
            else if (hand == rightHand) OnLeftReleased?.Invoke();
            
            otherHandJug.OnRelease();
            otherHandJug = null;
        }
    }

#if UNITY_EDITOR
    public void EditorTryGrab(Transform hand, bool isLeft)
    {
        // This function must find a jug first to pass to ExecuteGrab
        Collider[] colliders = Physics.OverlapSphere(hand.position, grabDistance, climbableLayer);
        IClimbable jug = colliders.FirstOrDefault(x => x.GetComponent<IClimbable>() != null)?.GetComponent<IClimbable>();
        
        if (jug != null)
        {
            if (isLeft)
            {
                ExecuteGrab(hand, jug, ref leftHandHeldJug, ref leftHandGrabOffset, ref rightHandHeldJug);
            }
            else
            {
                ExecuteGrab(hand, jug, ref rightHandHeldJug, ref rightHandGrabOffset, ref leftHandHeldJug);
            }
        }
    }
#endif

    private void ReleaseHand(ref IClimbable heldJug, bool isLeftHand)
    {
        if (heldJug != null)
        {
            if (!heldJug.CanRelease())
            {
                return;
            }

            heldJug.OnRelease();
            heldJug = null;

            if (isLeftHand) OnLeftReleased?.Invoke();
            else OnRightReleased?.Invoke();

            if (leftHandHeldJug == null && rightHandHeldJug == null)
            {
                playerRigidbody.isKinematic = false;
                playerRigidbody.linearVelocity = climbVelocity * momentumMultiplier;
            }
        }
    }
    
    // The rest of the script remains unchanged...
    void FixedUpdate()
    {
        if (activeHeldJug != null && activeHand != null)
        {
            Climb();
        }
        else
        {
            ApplyGravity();
        }
        if (calibrated)
            CalibrateColliderPositionToHead();
    }

    public void ForceRelease(Transform hand)
    {
        if (leftHand == hand)
        {
            if (leftHandHeldJug != null)
            {
                leftHandHeldJug.OnRelease();
                leftHandHeldJug = null;
                OnLeftReleased?.Invoke();
            }
        }
        else if (rightHand == hand)
        {
            if (rightHandHeldJug != null)
            {
                rightHandHeldJug.OnRelease();
                rightHandHeldJug = null;
                OnRightReleased?.Invoke();
            }
        }

        if (leftHandHeldJug == null && rightHandHeldJug == null)
        {
            playerRigidbody.isKinematic = false;
            playerRigidbody.linearVelocity = climbVelocity * momentumMultiplier;
        }
    }

    private void UpdateActiveHand()
    {
        if (leftHandHeldJug != null)
        {
            activeHeldJug = leftHandHeldJug;
            activeHand = leftHand;
            activeGrabOffset = leftHandGrabOffset;
        }
        else if (rightHandHeldJug != null)
        {
            activeHeldJug = rightHandHeldJug;
            activeHand = rightHand;
            activeGrabOffset = rightHandGrabOffset;
        }
        else
        {
            activeHeldJug = null;
            activeHand = null;
        }
    }

    private void Climb()
    {
        Vector3 targetHandPosition = activeHeldJug.transform.position + activeGrabOffset;
        Vector3 positionDelta = targetHandPosition - activeHand.position;

        SlipperyJug slipperyJug = activeHeldJug as SlipperyJug;
        if (slipperyJug != null)
        {
            if (activeHand == leftHand) {
                leftHandGrabOffset += Vector3.down * (slipperyJug.FallSpeed * Time.fixedDeltaTime);
                activeGrabOffset = leftHandGrabOffset;
            } else if (activeHand == rightHand) {
                rightHandGrabOffset += Vector3.down * (slipperyJug.FallSpeed * Time.fixedDeltaTime);
                activeGrabOffset = rightHandGrabOffset;
            }
            targetHandPosition = activeHeldJug.transform.position + activeGrabOffset;
            positionDelta = targetHandPosition - activeHand.position;
        }

        const float skinWidth = 0.01f;
        var col = (CapsuleCollider)playerCollider;
        if (playerRigidbody.SweepTest(positionDelta.normalized, out var hitInfo, positionDelta.magnitude, QueryTriggerInteraction.Ignore))
        {
            Vector3 allowedMovement = positionDelta.normalized * (hitInfo.distance - skinWidth);
            playerRigidbody.MovePosition(playerRigidbody.position + allowedMovement);
            climbVelocity = allowedMovement / Time.fixedDeltaTime;
        }
        else
        {
            playerRigidbody.MovePosition(playerRigidbody.position + positionDelta);
            climbVelocity = positionDelta / Time.fixedDeltaTime;
        }
    }

    private void CalibrateColliderPositionToHead()
    {
        Vector3 headPosition = playerHead.transform.position;
        playerCollider.transform.position = new Vector3(headPosition.x, playerCollider.transform.position.y, headPosition.z);
    }

    private void ApplyGravity()
    {
        if (!playerRigidbody.isKinematic)
        {
            playerRigidbody.linearVelocity += Vector3.down * gravity * Time.fixedDeltaTime;
        }
    }

    private void InitializePlayerPosition()
    {
        Camera headCameras = playerCameraRig.GetComponentInChildren<Camera>();
        if (!headCameras)
        {
            Debug.LogError("No Camera found in the children of the Player Camera Rig.", this);
            return;
        }

        Vector3 headPosition = headCameras.transform.position;
        Vector3 rigPosition = playerCameraRig.position;
        
        Vector3 targetRigPosition = new Vector3(headPosition.x, headPosition.y, headPosition.z);
        Vector3 positionDelta = targetRigPosition - rigPosition;

        playerCameraRig.GetComponentInChildren<Collider>().transform.position += positionDelta;
        Debug.LogWarning(positionDelta);
        calibrated = true;
    }

    private IEnumerator<float> InitializePlayerPositionCoroutine()
    {
        yield return Timing.WaitForSeconds(1.5f);
        InitializePlayerPosition();
        calibrated = true;
    }
}