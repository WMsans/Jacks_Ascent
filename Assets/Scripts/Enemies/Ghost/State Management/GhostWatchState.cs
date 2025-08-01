using UnityEngine;

// ghost watches player. if the player stays around the same altitude for some time, the ghost will approach.
public class GhostWatchState : BaseState
{

    private GhostStateMachineRunner owner;

    private Transform player;
    private Vector3 playerWatchPos;

    [SerializeField] private float ghostWatchTime = 4f;
    private float phaseTime;

    public override void OnEnterState()
    {
        owner = (GhostStateMachineRunner)Owner;

        player = owner.player;
        owner.ghostStartPos = transform.position;
        phaseTime = 0f;
    }
    public override void OnUpdateState()
    {
        phaseTime += Time.deltaTime;

        owner.JitterGhostY();

        // check if player stayed at around the same altitude
        if (Mathf.Abs(player.position.y - playerWatchPos.y) < 1f)
        {
            // player has stayed at around the same altitude for long enough
            if (phaseTime > ghostWatchTime)
            {
                phaseTime = 0;
                // approach the player
                owner.ChangeState(GetComponent<GhostApproachState>());
            }
        }
        // player's altitude shifted too much
        else
        {
            phaseTime = 0;
            playerWatchPos = player.position;
            // rise to player's new altitude
            owner.ChangeState(GetComponent<GhostRiseState>());
        }
    }
}
