using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ODM : MonoBehaviour, IWeapon
{
    public Rigidbody rigidBody;
    public Transform firePos;
    public GameObject hookPrefab;
    public float hookSpeed = 40f;
    private Hook currentHook;
    private OdmController odmController;

    void Awake()
    {
        if (!rigidBody) rigidBody = GetComponent<Rigidbody>();
    }

    public void PrimaryAction()
    {
        if (currentHook) return;
        GameObject hookGO = Instantiate(hookPrefab, firePos.position, firePos.rotation);
        currentHook = hookGO.GetComponent<Hook>();
        currentHook.Initialize(this, firePos.forward * hookSpeed);
    }

    public void PrimaryActionCanceled()
    {
        if (!currentHook) return;
        currentHook.Recall();
        currentHook = null;
        if(odmController != null)
        {
            odmController.StopReeling();
        }
    }

    public void Pickup(Transform hand, OVRInput.Controller controller)
    {
        transform.SetParent(hand);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        rigidBody.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        odmController = hand.GetComponentInParent<OdmController>();
    }

    public void Drop(OVRInput.Controller controller)
    {
        transform.SetParent(null);
        rigidBody.isKinematic = false;
        GetComponent<Collider>().enabled = true;
        rigidBody.linearVelocity = OVRInput.GetLocalControllerVelocity(controller);
        rigidBody.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(controller);
        odmController = null;
    }

    public void HookAttached(Vector3 point)
    {
        if (odmController != null)
        {
            odmController.StartReeling(point);
        }
    }
}