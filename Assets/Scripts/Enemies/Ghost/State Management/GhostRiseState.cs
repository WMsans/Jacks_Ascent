using UnityEngine;

// ghost rises/falls to player's altitude. once it reaches around the same altitude as the player, it will start to watch.
public class GhostRiseState : BaseState
{

    private GhostStateMachineRunner owner;

    [SerializeField] private float riseSpeed = 3f;

    private Transform player;
    private Vector3 playerPos;

    public override void OnEnterState()
    {
        owner = (GhostStateMachineRunner)Owner;

        player = owner.player;
    }

    public override void OnFixedUpdateState()
    {
        playerPos = player.position;
    }

    public override void OnUpdateState()
    {
        // calculate target position based on player position
        float targetYPos = playerPos.y;
        Vector3 targetPos = transform.position; targetPos.y = targetYPos;

        // determine direction that the ghost is traveling in
        bool goDown = transform.position.y >= targetYPos;
        float direction = goDown ? -1 : 1;

        // based on speed + direction, calculate y displacement during this frame
        float deltaY = riseSpeed * direction * Time.deltaTime;
        // account for ghost height to find the distance above the ghost's head that needs to be checked
        float checkDist = deltaY + (GetComponent<MeshRenderer>().bounds.size.y / 2);

        // check if there is any terrain within the calculated distance
        if (owner.CheckPathBlocked(transform.position, checkDist))
        {
            // pass the y displacement to next state for checking
            GetComponent<GhostRepositionState>().SetTargetY(deltaY);
            owner.ChangeState(GetComponent<GhostRepositionState>());
        }
        else
        {
            // smooth movement to next y position
            Vector3 velocity = Vector3.Normalize(targetPos - transform.position) * riseSpeed;
            Vector3 nextPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 0.3f);
            transform.position = nextPos;

            // ghost reaches around the same altitude as player
            if (Mathf.Abs(transform.position.y - targetYPos) < 1f)
            {
                owner.ChangeState(GetComponent<GhostWatchState>());
            }
        }
    }

}
