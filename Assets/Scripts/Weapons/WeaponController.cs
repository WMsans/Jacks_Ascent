using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Hand Tracking")]
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private OVRInput.Controller leftHandController = OVRInput.Controller.LTouch;
    [SerializeField] private OVRInput.Controller rightHandController = OVRInput.Controller.RTouch;

    [Header("Hand Positioning")]
    [SerializeField] private GameObject defaultLeftHand;
    [SerializeField] private GameObject weaponLeftHand;
    [SerializeField] private GameObject defaultRightHand;
    [SerializeField] private GameObject weaponRightHand;

    [Header("Weapon Settings")]
    [SerializeField] private LayerMask grabableLayer;
    [SerializeField] private float grabDistance = 0.1f;

    private IWeapon leftHandHeldWeapon = null;
    private IWeapon rightHandHeldWeapon = null;

    private ClimbingController climbingController;

    private void Start()
    {
        climbingController = GetComponent<ClimbingController>();
    }

    void Update()
    {
        // Handle grabbing and dropping
        HandleGrabbing(rightHand, rightHandController, ref rightHandHeldWeapon, ref defaultRightHand, ref weaponRightHand);
        HandleGrabbing(leftHand, leftHandController, ref leftHandHeldWeapon, ref defaultLeftHand, ref weaponLeftHand);

        // Handle weapon actions
        HandleActions();
    }

#if UNITY_EDITOR
    public void EditorAttack(bool isLeft)
    {
        if (isLeft)
        {
            if (leftHandHeldWeapon != null)
                leftHandHeldWeapon.PrimaryAction();
        }
        else
        {
            if (rightHandHeldWeapon != null)
                rightHandHeldWeapon.PrimaryAction();
        }
    }
#endif

    private void HandleGrabbing(Transform hand, OVRInput.Controller controller, ref IWeapon heldWeapon, ref GameObject defaultHand, ref GameObject weaponHand)
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller))
        {
            if (heldWeapon == null)
            {
                TryGrabWeapon(hand, controller, ref heldWeapon, ref defaultHand, ref weaponHand);
            }
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, controller))
        {
            if (heldWeapon != null)
            {
                heldWeapon.Drop(controller);
                heldWeapon = null;

                // switch to default position
                defaultHand.SetActive(true);
                weaponHand.SetActive(false);
            }
        }
    }

    private void TryGrabWeapon(Transform hand, OVRInput.Controller controller, ref IWeapon heldWeapon, ref GameObject defaultHand, ref GameObject weaponHand)
    {
        Collider[] colliders = Physics.OverlapSphere(hand.position, grabDistance, grabableLayer, QueryTriggerInteraction.Collide);
        if (colliders.Length > 0)
        {
            if (colliders[0].TryGetComponent<IWeapon>(out var weapon))
            {
                heldWeapon = weapon;
                heldWeapon.Pickup(hand, controller);

                // switch to grip position
                defaultHand.SetActive(false);
                weaponHand.SetActive(true);
            }
        }
    }

    private void HandleActions()
    {
        // Right hand primary action using the right index trigger
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, rightHandController))
        {
            if (rightHandHeldWeapon != null)
                rightHandHeldWeapon.PrimaryAction();
            else if (climbingController != null && climbingController.LeftHandHeldJug is IHarmable harmable)
                harmable.OnHarm(1);
        }
        else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, rightHandController))
        {
            if (rightHandHeldWeapon != null)
                rightHandHeldWeapon.PrimaryActionCanceled();
        }

        // Left hand primary action using the left index trigger
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, leftHandController))
        {
            if (leftHandHeldWeapon != null)
                leftHandHeldWeapon.PrimaryAction();
            else if (climbingController != null && climbingController.RightHandHeldJug is IHarmable harmable)
                harmable.OnHarm(1);
        }
        else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, leftHandController))
        {
            if (leftHandHeldWeapon != null)
                leftHandHeldWeapon.PrimaryActionCanceled();
        }
    }

    public bool IsHandHoldingWeapon(Transform hand)
    {
        if (hand == leftHand)
        {
            return leftHandHeldWeapon != null;
        }
        else if (hand == rightHand)
        {
            return rightHandHeldWeapon != null;
        }
        return false;
    }
}