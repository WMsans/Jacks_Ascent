using UnityEngine;

/*
GHOST BEHAVIOR:
1. rises up to player height
2. keeps rising / falling until player stops for set amt of time
3. surges toward & attacks player's last position
4. goes back & repeats
*/

public class GhostStateMachineRunner : StateMachineRunner
{

    public Transform player;

    public Vector3 ghostStartPos;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        LookAtPlayer();
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    [SerializeField] private float jitterFreqY = 2f;
    [SerializeField] private float jitterMaxY = 0.75f;


    // makes ghost hover up and down in place
    public void JitterGhostY()
    {
        float t = Time.time * jitterFreqY;

        Vector3 nextPos = ghostStartPos;
        nextPos.y += jitterMaxY * Mathf.Sin(t);

        Vector3 velocity = Vector3.Normalize(nextPos - transform.position) * 3f;

        nextPos = Vector3.SmoothDamp(transform.position, nextPos, ref velocity, 0.3f);
        transform.position = nextPos;
    }

    // makes ghost face player
    private void LookAtPlayer()
    {
        transform.LookAt(player, Vector3.up);
    }
    
    // used to detect for terrain in ghost's movement path (remember to set layer to terrain)
    public bool CheckPathBlocked(Vector3 checkPos, float checkDist)
    {
        Vector3 nextPos = checkPos;
        nextPos.y += checkDist;
        return Physics.Linecast(transform.position, nextPos, LayerMask.NameToLayer("Terrain"));
    }

}
