using UnityEngine;

// ghost stays still. idles by hovering up and down.
public class GhostStillState : BaseState
{

    private GhostStateMachineRunner owner;

    [SerializeField] private float ghostStillTime = 1f;

    private float phaseTime;

    public override void OnEnterState()
    {
        owner = (GhostStateMachineRunner)Owner;

        phaseTime = 0f;
    }

    public override void OnUpdateState()
    {
        phaseTime += Time.deltaTime;

        owner.JitterGhostY();

        if (phaseTime > ghostStillTime)
        {
            phaseTime = 0;
            owner.ChangeState(GetComponent<GhostRiseState>());
        }
    }
}
