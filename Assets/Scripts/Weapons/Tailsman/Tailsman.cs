using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tailsman : MonoBehaviour, IWeapon
{
    public Rigidbody rigidBody;
    public float damage;

    void Awake()
    {
        if(!rigidBody) rigidBody = GetComponent<Rigidbody>();
    }

    public void PrimaryAction()
    {
    }

    public void Pickup(Transform hand, OVRInput.Controller controller)
    {
        transform.SetParent(hand);
        transform.localPosition = Vector3.zero;
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

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Enemy"))
        {
            if (other.gameObject.TryGetComponent<IHarmable>(out var harmable))
            {
                harmable.OnHarm(damage);
            }
        }
    }
}
