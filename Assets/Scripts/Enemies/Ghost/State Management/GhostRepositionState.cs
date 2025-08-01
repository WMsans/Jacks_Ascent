using System;
using UnityEngine;

// ghost adjusts x and z to make y movement valid
public class GhostRepositionState : BaseState
{

    private GhostStateMachineRunner owner;

    private Vector3 targetPos;

    private float targetDistY;
    private float initialDistance;

    [SerializeField] private float repositionSpeed = 3f;


    [Header("(Dumb) Pathfinding")]
    [SerializeField] private float searchRadius;

    [SerializeField] private Transform origin;

    private float repositionTime;
    private float phaseTime;

    private bool sideReposition;

    public override void OnEnterState()
    {
        owner = (GhostStateMachineRunner)Owner;

        owner.ghostStartPos = transform.position;

        // retrieve position for ascent (if it exists)
        Vector3? validPos = GetValidPosition();
        if (validPos.HasValue)
        {
            targetPos = validPos.Value;
            sideReposition = true;
        }
        else
        {
            // implement alternate behavior (placeholder rn: just phase through)
            targetPos = transform.position;
            targetPos.y = targetDistY;
        }

        // get distance away from target
        initialDistance = Vector3.Distance(owner.ghostStartPos, targetPos);
        // calculate time needed to reposition based on speed and distance
        repositionTime = Vector3.Distance(transform.position, targetPos) / repositionSpeed;

        phaseTime = 0f;
    }

    public override void OnUpdateState()
    {
        phaseTime += Time.deltaTime;

        Vector3 velocity;

        // repositioning with x and z axes (valid position got)
        if (sideReposition)
        {
            if (initialDistance >= 5f) owner.JitterGhostY();
            velocity = Vector3.Normalize(targetPos - owner.ghostStartPos) * repositionSpeed;
            velocity.y = 0;
        }
        // repositioning with y axis (placeholder behavior: just phase through)
        else
        {
            velocity = Vector3.up * repositionSpeed;
        }

        // after sufficient time, exit reposition (back to rising)
        if (phaseTime >= repositionTime)
        {
            owner.ChangeState(GetComponent<GhostRiseState>());
        }

        // smooth movement to target position
        Vector3 newPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 0.3f);
        transform.position = newPos;
    }

    // returns target position to rise from, null if it doesn't exist
    private Vector3? GetValidPosition()
    {
        // check radially (might have to change depending on shape of chute)
        for (float rad = 1f; rad <= searchRadius; rad += 0.5f)
        {
            for (float angle = 0f; angle < 360f; angle += 30f)
            {
                Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * rad;
                Vector3 testPos = origin.position + offset;
                testPos.y = transform.position.y;

                // returns closest position that works
                if (!owner.CheckPathBlocked(testPos, targetDistY + (GetComponent<MeshRenderer>().bounds.size.y / 2)))
                {
                    return testPos;
                }
            }
        }

        // didn't find a valid position
        return null;
    }

    // utility to pass target distance from rise state
    public void SetTargetY(float targetDistY)
    {
        this.targetDistY = targetDistY;
    }

}
