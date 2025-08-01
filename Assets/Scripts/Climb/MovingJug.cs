using UnityEngine;
using DG.Tweening;

public class MovingJug : MonoBehaviour, IClimbable
{
    public Transform pointA;
    public Transform pointB;
    public float moveDuration = 2f;

    private bool isHolding = false;
    private Transform holdingHand = null;

    void Start()
    {
        // Set up the movement sequence using DoTween
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(pointB.position, moveDuration).SetEase(Ease.InOutSine));
        sequence.Append(transform.DOMove(pointA.position, moveDuration).SetEase(Ease.InOutSine));
        sequence.SetLoops(-1, LoopType.Yoyo);
    }

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

    void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pointA.position, 0.1f);
            Gizmos.DrawWireSphere(pointB.position, 0.1f);
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}