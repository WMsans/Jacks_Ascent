using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.AI;

public class SpiderChasePlayerState : SpiderBaseState
{
    public Transform Target { get; set; }

    [Header("Navigation")]
    [SerializeField]
    private float navMeshSearchRadius = 2.0f;

    [Header("Audio")]
    [SerializeField] private StudioEventEmitter crawlSoundEmitter;
    [SerializeField] private float speedThreshold = 0.2f;

    private bool isCrawlingSoundPlaying = false;

    public override void OnEnterState()
    {
        agent.enabled = true;
        agent.isStopped = false;
    }

    public override void OnUpdateState()
    {
        if (Target == null)
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }
            return;
        }

        // Set the destination to a valid point on the NavMesh.
        if (NavMesh.SamplePosition(Target.position, out NavMeshHit hit, navMeshSearchRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            // Fallback: If no point is found, set destination to the target's raw position.
            agent.SetDestination(Target.position);
        }

        // --- Sound Logic ---
        // Get the agent's current speed on the horizontal plane.
        float currentSpeed = agent.velocity.magnitude;

        // If speed is above the threshold and the sound isn't playing, play it.
        if (currentSpeed > speedThreshold && !isCrawlingSoundPlaying)
        {
            crawlSoundEmitter.Play();
            isCrawlingSoundPlaying = true;
        }
        // If speed is below the threshold and the sound is playing, stop it.
        else if (currentSpeed <= speedThreshold && isCrawlingSoundPlaying)
        {
            crawlSoundEmitter.Stop();
            isCrawlingSoundPlaying = false;
        }
    }

    public override void OnExitState()
    {
        // Stop the agent
        if (agent != null && agent.isOnNavMesh && agent.enabled)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
        
        // Ensure the sound is stopped when we exit this state.
        if (isCrawlingSoundPlaying)
        {
            crawlSoundEmitter.Stop();
            isCrawlingSoundPlaying = false;
        }

        Target = null; // Clear the target
    }

    private void OnDestroy()
    {
        if (isCrawlingSoundPlaying)
        {
            crawlSoundEmitter.Stop();
            isCrawlingSoundPlaying = false;
        }
    }
}