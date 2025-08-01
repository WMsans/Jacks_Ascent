using UnityEngine;

// ghost "spawns." basically stays still for a set amount of time before starting to move.
public class GhostSpawnState : BaseState
{

    private GhostStateMachineRunner owner;

    [SerializeField] private float ghostSpawnTime = 3f;

    private float phaseTime;

    public override void OnEnterState()
    {
        owner = (GhostStateMachineRunner)Owner;

        phaseTime = 0f;
    }

    public override void OnUpdateState()
    {
        phaseTime += Time.deltaTime;
        if (phaseTime > ghostSpawnTime)
        {
            phaseTime = 0;
            owner.ChangeState(GetComponent<GhostRiseState>());
        }
    }
}
