using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LTFPatrolState : BaseState
{

    private LTFStateMachineRunner owner;

    private Transform player;
    private Vector3 playerPos;

    private int direction;

    [SerializeField] private float patrolSpeed = 2f;

    [SerializeField] private float distanceToFollow = 5f;

    private List<Transform> nodePath;

    private int nextNode;

    private float cooldown;
    private float timeElapsed;

    public override void OnEnterState()
    {
        owner = (LTFStateMachineRunner)Owner;

        player = owner.player;
        playerPos = player.position;

        nodePath = GetComponent<LTFFollowState>().nodePath;

        cooldown = Mathf.Infinity;
        timeElapsed = 0f;
    }

    public override void OnFixedUpdateState()
    {
        playerPos = player.position;
    }

    public override void OnUpdateState()
    {
        timeElapsed += Time.deltaTime;

        if ((timeElapsed >= cooldown) && IsCloseEnough())
        {
            owner.ChangeState(GetComponent<LTFFollowState>());
            return;
        }

        Vector3 velocity = Vector3.Normalize(nodePath[nextNode].position - transform.position) * patrolSpeed;
        Vector3 newPos = Vector3.SmoothDamp(transform.position, nodePath[nextNode].position, ref velocity, 0.5f);
        transform.position = newPos;

        if (Vector3.Distance(transform.position, nodePath[nextNode].position) < 0.1f)
        {
            if (nextNode <= 0) direction = 1;
            if (nextNode >= nodePath.Count - 1) direction = -1;

            nextNode += direction;
        }
    }

    public void SetDirection(int direction)
    {
        this.direction = direction;
    }

    private bool IsCloseEnough()
    {
        return Vector3.Distance(transform.position, playerPos) <= distanceToFollow;
    }

    public void SetCooldown(float cooldown)
    {
        this.cooldown = cooldown;
        timeElapsed = 0f;
    }
}
