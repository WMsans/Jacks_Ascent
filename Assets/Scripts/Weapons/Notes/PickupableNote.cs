using UnityEngine;

public class PickupableNote : MonoBehaviour, IWeapon
{
    public Rigidbody rigidBody;
    public Vector3 sizeChange;

    public void PrimaryAction()
    {
    }

    public void Pickup(Transform hand, OVRInput.Controller controller)
    {
        transform.SetParent(hand);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale += sizeChange;
        Debug.Log(transform.localScale);
        rigidBody.isKinematic = true;
        GetComponent<Collider>().enabled = false;
    }

    public void Drop(OVRInput.Controller controller)
    {   
        transform.SetParent(null);
        rigidBody.isKinematic = false;
        transform.localScale -= sizeChange;
        GetComponent<Collider>().enabled = true;
        rigidBody.linearVelocity = OVRInput.GetLocalControllerVelocity(controller);
        rigidBody.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(controller);
    }
}
