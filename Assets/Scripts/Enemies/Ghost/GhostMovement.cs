using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public enum GhostMovement { spawn, still, rise, watch, approach, retreat };
    public GhostMovement phase;

    [Header("Player Tracking")]
    public Transform player;
    public Vector3 playerPos;
    public Vector3 playerWatchPos;

    [Header("Movement")]
    public float riseSpeed;
    public float approachSpeed;

    public float jitterMaxY;
    public float jitterFreqY;

    private Vector3 ghostStartPos;

    public float ghostSpawnTime;
    public float ghostStillTime;
    public float ghostApproachTime;
    public float ghostUpCloseTime;

    private float currentPhaseTime;

    void Awake()
    {
        phase = GhostMovement.spawn;
    }

    void Start()
    {

    }

    void FixedUpdate()
    {
        playerPos = player.position;
    }

    void Update()
    {
        currentPhaseTime += Time.deltaTime;

        LookAtPlayer();

        switch (phase)
        {
            case GhostMovement.spawn:
                HandleGhostSpawn();
                break;
            case GhostMovement.still:
                HandleGhostStill();
                break;
            case GhostMovement.rise:
                HandleGhostRise();
                break;
            case GhostMovement.watch:
                HandleGhostWatch();
                break;
            case GhostMovement.approach:
                HandleGhostApproach();
                break;
            case GhostMovement.retreat:
                HandleGhostRetreat();
                break;
        }
    }

    // ghost "spawns." basically stays still for a set amount of time before starting to move.
    private void HandleGhostSpawn()
    {
        if (currentPhaseTime > ghostSpawnTime)
        {
            currentPhaseTime = 0;
            phase = GhostMovement.rise;
        }
    }

    // ghost stays still. idles by hovering up and down.
    private void HandleGhostStill()
    {
        JitterGhostY();
        if (currentPhaseTime > ghostStillTime)
        {
            currentPhaseTime = 0;
            phase = GhostMovement.rise;
        }
    }

    // ghost rises/falls to player's altitude. once it reaches around the same altitude as the player, it will start to watch.
    private void HandleGhostRise()
    {
        float targetYPos = playerPos.y;

        bool goDown = transform.position.y >= targetYPos;
        float direction = goDown ? -1 : 1;

        Vector3 nextPos = transform.position;
        nextPos.y += riseSpeed * direction * Time.deltaTime;
        transform.position = nextPos;

        // reaches around the same altitude as player
        if (Mathf.Abs(transform.position.y - targetYPos) < 1f)
        {
            currentPhaseTime = 0;
            ghostStartPos = transform.position;
            playerWatchPos = player.position;
            phase = GhostMovement.watch;
        }
    }

    // ghost watches player. if the player stays around the same altitude for some time, the ghost will approach.
    private void HandleGhostWatch()
    {
        JitterGhostY();

        // check if player stayed at around the same altitude
        if (Mathf.Abs(player.position.y - playerWatchPos.y) < 1f)
        {
            // player has stayed at around the same altitude for long enough
            if (currentPhaseTime > 4f)
            {
                currentPhaseTime = 0;
                // approach the player
                phase = GhostMovement.approach;
            }
        }
        // player's altitude shifted too much
        else
        {
            currentPhaseTime = 0;
            playerWatchPos = player.position;
            // rise to player's new altitude
            phase = GhostMovement.rise;
        }
    }

    // ghost gets close to the player. after some time, it retreats back to its original position.
    private void HandleGhostApproach()
    {
        Vector3 targetPos = player.position + player.forward * 3;

        Vector3 approachVelocity = (targetPos - transform.position) * approachSpeed;

        Vector3 nextPos = Vector3.SmoothDamp(transform.position, targetPos, ref approachVelocity, ghostApproachTime);
        transform.position = nextPos;

        if (currentPhaseTime >= ghostApproachTime + ghostUpCloseTime)
        {
            currentPhaseTime = 0;
            phase = GhostMovement.retreat;
        }
    }

    // ghost returns back to its position before approaching.
    private void HandleGhostRetreat()
    {
        Vector3 approachVelocity = (ghostStartPos - transform.position) * approachSpeed;

        Vector3 nextPos = Vector3.SmoothDamp(transform.position, ghostStartPos, ref approachVelocity, ghostApproachTime);
        transform.position = nextPos;

        if (currentPhaseTime >= ghostApproachTime)
        {
            currentPhaseTime = 0;
            phase = GhostMovement.still;
        }
    }

    // makes ghost hover up and down in place
    private void JitterGhostY()
    {
        float t = Time.time * jitterFreqY;

        Vector3 nextPos = ghostStartPos;
        nextPos.y += jitterMaxY * Mathf.Sin(t);

        Vector3 approachVelocity = (nextPos - transform.position) * approachSpeed;

        nextPos = Vector3.SmoothDamp(transform.position, nextPos, ref approachVelocity, ghostApproachTime);
        transform.position = nextPos;
    }

    // makes ghost face player
    private void LookAtPlayer()
    {
        transform.LookAt(player, Vector3.up);
    }

}
