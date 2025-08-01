using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class BreakableJug : MonoBehaviour, IClimbable
{
    [SerializeField]
    private float breakTime = 2.0f;

    [SerializeField] private float fallSpeed;

    public UnityEvent onBreak;

    private bool isHolding = false;
    private Transform holdingHand = null;
    private ClimbingController climbingController;

    private float breakingTimer;

    private void Start()
    {
        climbingController = FindFirstObjectByType<ClimbingController>();
    }

    public void OnGrab(Transform hand)
    {
        if (!isHolding)
        {
            isHolding = true;
            holdingHand = hand;
            breakingTimer = breakTime;
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

    private void Update()
    {
        if(!isHolding) return;
        breakingTimer -= Time.deltaTime;
        if (breakingTimer < 0)
        {
            Break();
        }
    }

    private void FixedUpdate()
    {
        if(!isHolding) return;
        transform.position += fallSpeed * Time.fixedDeltaTime * Vector3.down;
    }

    private void Break()
    {
        if (isHolding && climbingController != null)
        {
            climbingController.ForceRelease(holdingHand);
        }
        onBreak.Invoke();
        Destroy(gameObject);
    }
}