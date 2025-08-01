using Unity.VisualScripting;
using UnityEngine;

// ghost gets close to the player. after some time, it retreats back to its original position.
public class GhostApproachState : BaseState
{
    private GhostStateMachineRunner owner;
    private Transform player;

    [SerializeField] private float approachDist = 1f;

    [SerializeField] private float approachSpeed = 8f;

    [SerializeField] private float approachTime = 2f;
    [SerializeField] private float upCloseTime = 1f;

    private float phaseTime;

    public override void OnEnterState()
    {
        owner = (GhostStateMachineRunner)Owner;

        player = owner.player;
        phaseTime = 0f;
    }

    public override void OnUpdateState()
    {
        phaseTime += Time.deltaTime;

        // calculate target position (3 units in front of player)
        Vector3 targetPos = player.position + player.forward * approachDist;

        // smooth movement to target position
        Vector3 approachVelocity = (targetPos - transform.position) * approachSpeed;

        Vector3 nextPos = Vector3.SmoothDamp(transform.position, targetPos, ref approachVelocity, 0.3f);
        transform.position = nextPos;

        // after sufficient time, exit approach
        if (phaseTime >= approachTime + upCloseTime)
        {
            phaseTime = 0;
            Owner.ChangeState(GetComponent<GhostRetreatState>());
        }
    }
}
