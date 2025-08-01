using UnityEngine;
using UnityEngine.AI;

public class SpiderEscapeState : SpiderBaseState
{
    private Transform playerTransform;
    [SerializeField] private float escapeDistance = 10f;

    public override void OnEnterState()
    {
        var player = FindFirstObjectByType<ClimbingController>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
        agent.enabled = true;
        agent.isStopped = false;
        
        Debug.Log("Enter escape scene");
    }

    public override void OnUpdateState()
    {
        if (playerTransform == null)
        {
            return;
        }

        Vector3 runToPosition = transform.position + (transform.position - playerTransform.position).normalized * escapeDistance;

        if (NavMesh.SamplePosition(runToPosition, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.SetDestination(runToPosition);
        }
    }

    public override void OnExitState()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }
    }
}