using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Gun : MonoBehaviour, IShootable, IWeapon
{
    public Rigidbody rigidBody;
    public Transform firePos;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    private float lastShootTime = -Mathf.Infinity;
    public float shootCooldown = 0.5f;
    public UnityEvent onShoot;

    void Awake()
    {
        if(!rigidBody) rigidBody = GetComponent<Rigidbody>();
    }
    [VInspector.Button]
    public void Shoot()
    {
        if (Time.time - lastShootTime < shootCooldown)
            return;

        lastShootTime = Time.time;

        GameObject bullet = Instantiate(bulletPrefab, firePos.position, firePos.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = firePos.forward * bulletSpeed;
        onShoot.Invoke();
    }

    public void PrimaryAction()
    {
        Shoot();
    }

    public void Pickup(Transform hand, OVRInput.Controller controller)
    {
        transform.SetParent(hand);
        transform.localPosition = Vector3.zero;

        // adjustments to weapon position
        Vector3 gunRelativePos = new Vector3(0.0066f, 0.0938f, 0.0898f);
        transform.localPosition = gunRelativePos;

        transform.localRotation = Quaternion.identity;
        rigidBody.isKinematic = true;
        GetComponent<Collider>().enabled = false;
    }

    public void Drop(OVRInput.Controller controller)
    {
        transform.SetParent(null);
        rigidBody.isKinematic = false;
        GetComponent<Collider>().enabled = true;
        rigidBody.linearVelocity = OVRInput.GetLocalControllerVelocity(controller);
        rigidBody.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(controller);
    }
}