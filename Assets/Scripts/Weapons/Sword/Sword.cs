using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Sword : MonoBehaviour, IWeapon
{
    public float damage = 10f;
    public float minSpeed = 2f;

    [Tooltip("A point on the blade (in local space) to use for speed calculation. E.g., (0, 0, 1) for 1 meter along the Z-axis.")]
    public Vector3 slashPointOffset = new Vector3(0, 0, 1f);

    public Rigidbody rigidBody;
    
    private Vector3 _previousSlashPointPosition;
    private float _currentSpeed;
    private OVRInput.Controller _controller;

    void Awake()
    {
        if(!rigidBody) rigidBody = GetComponent<Rigidbody>();
        // Initialize the slash point's position tracker.
        _previousSlashPointPosition = transform.TransformPoint(slashPointOffset);
    }

    public void PrimaryAction()
    {
    }

    public void Pickup(Transform hand, OVRInput.Controller controller)
    {
        SetController(controller);
        transform.SetParent(hand);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        rigidBody.isKinematic = true;
        GetComponent<Collider>().isTrigger = true; 
        
        // Reset the slash point tracker when the sword is picked up.
        _previousSlashPointPosition = transform.TransformPoint(slashPointOffset);
    }

    public void Drop(OVRInput.Controller controller)
    {
        transform.SetParent(null);
        rigidBody.isKinematic = false;
        GetComponent<Collider>().isTrigger = false;
        rigidBody.linearVelocity = OVRInput.GetLocalControllerVelocity(controller);
        rigidBody.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(controller);
    }
    
    private void SetController(OVRInput.Controller c)
    {
        _controller = c;
    }
    
    void Update()
    {
        // Get the current world position of the slash point on the blade.
        Vector3 currentSlashPointPosition = transform.TransformPoint(slashPointOffset);
        
        // Calculate displacement of the slash point since the last frame.
        Vector3 displacement = currentSlashPointPosition - _previousSlashPointPosition;

        // Calculate the speed of that point.
        if (Time.deltaTime > 0)
        {
            _currentSpeed = displacement.magnitude / Time.deltaTime;
        }
        
//        Debug.Log(_currentSpeed);

        // Update the tracker for the next frame.
        _previousSlashPointPosition = currentSlashPointPosition;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (_currentSpeed > minSpeed)
            {
                if (other.TryGetComponent<IHarmable>(out var harmable))
                {
                    harmable.OnHarm(damage);
                }
            }
        }
    }
}