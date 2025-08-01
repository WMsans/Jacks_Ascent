using System.Collections.Generic;
using UnityEngine;

public class LTFFollowState : BaseState
{

    private LTFStateMachineRunner owner;

    private Transform player;
    private Vector3 playerPos;

    [SerializeField] private float followSpeed = 2f;

    [SerializeField] private float distanceToPester = 0.5f;
    [SerializeField] private float distanceToPatrol = 10f;

    public List<Transform> nodePath;

    private int nextNode, targetNode, secNode;

    private int direction;

    public override void OnEnterState()
    {
        owner = (LTFStateMachineRunner)Owner;

        player = owner.player;
        playerPos = player.position;

        gameObject.SetActive(true);
    }

    public override void OnFixedUpdateState()
    {
        playerPos = player.position;

        UpdateTargetNode();

        direction = (nextNode <= targetNode) ? 1 : -1;
    }

    public override void OnUpdateState()
    {
        if (IsCloseEnough())
        {
            owner.ChangeState(GetComponent<LTFApproachState>());
            return;
        }

        if (IsTooFar())
        {
            owner.ChangeState(GetComponent<LTFPatrolState>());
            GetComponent<LTFPatrolState>().SetDirection(direction);
            return;
        }

        if (nodePath.Count == 0)
        {
            // just disappear
            owner.ChangeState(GetComponent<LTFSpawnState>());
            return;
        }

        Vector3 velocity = Vector3.Normalize(nodePath[nextNode].position - transform.position) * followSpeed;
        Vector3 newPos = Vector3.SmoothDamp(transform.position, nodePath[nextNode].position, ref velocity, 0.5f);
        transform.position = newPos;

        if (Vector3.Distance(transform.position, nodePath[nextNode].position) < 0.1f)
        {
            // heading in right direction
            if ((targetNode - nextNode >= 0) == (direction >= 0))
            {
                nextNode += direction;
            }
            // just passed target
            else
            {
                nextNode = secNode;
            }
        }
    }

    // assigns target and secondary nodes based on player position
    private void UpdateTargetNode()
    {
        float curDistance, minDistance = Mathf.Infinity;
        int index = -1;
        for (int i = 0; i < nodePath.Count; i++)
        {
            curDistance = Vector3.Distance(playerPos, nodePath[i].position);
            if (curDistance < minDistance)
            {
                minDistance = curDistance;
                index = i;
            }
        }

        // assign two nodes to shift between
        targetNode = index;
        if (targetNode == 0)
        {
            secNode = 1;
        }
        else if (targetNode == nodePath.Count - 1)
        {
            secNode = nodePath.Count - 2;
        }
        else
        {
            secNode = (Vector3.Distance(playerPos, nodePath[targetNode - 1].position) > Vector3.Distance(playerPos, nodePath[targetNode + 1].position)) ? targetNode - 1 : targetNode + 1;
        }
    }

    // checks if swarm is close enough to player to pester
    private bool IsCloseEnough()
    {
        return Vector3.Distance(transform.position, playerPos) <= distanceToPester;
    }

    // check if player left the vicinity
    private bool IsTooFar()
    {
        return Vector3.Distance(transform.position, playerPos) >= distanceToPatrol;
    }

    public void SetStartNode(int startNode)
    {
        nextNode = startNode;
    }

}
