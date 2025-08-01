using System;
using UnityEngine;
using UnityEngine.AI; 

public class SpiderIdleState : SpiderBaseState
{
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float speedThreshold = 2f;
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private SpiderChasePlayerState chaseState;

    private Collider[] colliders = new Collider[10];

    // It's good practice to get component references in Awake
    private void Awake()
    {
        // Assuming the NavMeshAgent is on the same GameObject as the state script's owner
        // This might already be handled in your SpiderBaseState. If so, you can remove this method.
        agent = GetComponentInParent<NavMeshAgent>();
    }

    // This method is called once when the state machine transitions into this state
    public override void OnEnterState()
    {
        if (agent != null)
        {
            // Stop the agent from moving towards a previous destination
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }
    }

    public override void OnUpdateState()
    {
        // Your detection logic remains the same
        var n = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, colliders, detectionLayer, QueryTriggerInteraction.Ignore);
        
        for (var i =0; i < n; i++)
        {
            var t = colliders[i];
            if (t != null && t.TryGetComponent<Rigidbody>(out var rb) && rb.linearVelocity.sqrMagnitude > speedThreshold * speedThreshold)
            {
                chaseState.Target = rb.transform;
                Owner.ChangeState(chaseState);
                return;
            }
            else if (t != null && t.transform.parent != null && t.transform.parent.TryGetComponent<Rigidbody>(out rb) &&
                     rb.linearVelocity.sqrMagnitude > speedThreshold * speedThreshold)
            {
                chaseState.Target = rb.transform;
                Owner.ChangeState(chaseState);
                return;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}