using UnityEngine;

public class SlipperyJug : MonoBehaviour, IClimbable
{
    [SerializeField] private float fallSpeed = 0.5f;
    public float FallSpeed { get { return fallSpeed; } }

    private bool isHolding = false;
    private Transform holdingHand = null;
    public void OnGrab(Transform hand)
    {
        isHolding = true;
        holdingHand = hand;
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