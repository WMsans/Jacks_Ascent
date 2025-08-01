using System;
using UnityEngine;
using UnityEngine.Events;

public class GunSlot : MonoBehaviour
{
    [SerializeField] private Transform followPoint;
    [SerializeField] private Vector3 deltaPos = new Vector3(0, 0.5f, 0);
    [SerializeField] private Quaternion deltaRot = Quaternion.Euler(90, 180, -90);
    [Header("Events")] [SerializeField] private UnityEvent onGunIn;
    [SerializeField] private UnityEvent onGunOut;

    private void Awake()
    {
        deltaPos = new Vector3(0, 0.5f, 0);
        deltaRot = Quaternion.Euler(90, 180, -90);
    }

    private void FixedUpdate()
    {
        transform.position = followPoint.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.NameToLayer("Grabable") != other.gameObject.layer) return;
        if (other.transform.parent == null)
        {
            if (other.TryGetComponent<Rigidbody>(out var rb))
                rb.isKinematic = true;
            
            other.transform.SetParent(transform);
            other.transform.localRotation = deltaRot;
            other.transform.localPosition = deltaPos;
            
            onGunIn.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (LayerMask.NameToLayer("Grabable") != other.gameObject.layer) return;
        onGunOut.Invoke();
    }
}