using UnityEngine;

public interface IWeapon
{
    void PrimaryAction();
    void PrimaryActionCanceled(){}
    void Pickup(Transform hand, OVRInput.Controller controller);
    void Drop(OVRInput.Controller controller);
}