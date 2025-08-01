using System.Collections;
using UnityEngine;

public class RisingMouthJug : MonoBehaviour, IClimbable
{
    public RisingMouth risingMouth;
    [SerializeField] private Transform spawnPos;
    private bool hasSpawned = false;
    private bool isHolding = false;
    private Transform holdingHand = null;

    public void OnGrab(Transform hand)
    {
        isHolding = true;
        holdingHand = hand;

        if (!hasSpawned)
        {
            risingMouth.StartMouthBehavior();
            hasSpawned = true;
        }
    }

    public void OnRelease()
    {
        isHolding = false;
        holdingHand = null;
    }

    public bool IsHolding(Transform hand)
    {
        return isHolding && holdingHand == hand;
    }

    public bool CanRelease()
    {
        return true;
    }
}

