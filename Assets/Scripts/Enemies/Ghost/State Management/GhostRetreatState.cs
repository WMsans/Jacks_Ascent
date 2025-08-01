using UnityEngine;

// ghost returns back to its original position (before approaching).
public class GhostRetreatState : BaseState
{

    private GhostStateMachineRunner owner;

    private Vector3 startPos;

    [SerializeField] private float retreatSpeed = 6f;

    [SerializeField] private float ghostRetreatTime = 3f;
    private float phaseTime;

    public override void OnEnterState()
    {
        owner = (GhostStateMachineRunner)Owner;

        startPos = owner.ghostStartPos;
    }

    public override void OnUpdateState()
    {
        phaseTime += Time.deltaTime;

        // smooth movement to starting position (before approach)
        Vector3 retreatVelocity = (startPos - transform.position) * retreatSpeed;

        Vector3 nextPos = Vector3.SmoothDamp(transform.position, startPos, ref retreatVelocity, 0.3f);
        transform.position = nextPos;

        // after sufficient time, exit retreat
        if (phaseTime >= ghostRetreatTime)
        {
            phaseTime = 0;
            owner.ChangeState(GetComponent<GhostStillState>());
        }
    }

}
