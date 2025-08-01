using UnityEngine;
using System.Collections;

public class LTFSpawnState : BaseState
{

    private LTFStateMachineRunner owner;

    [SerializeField] private int startNode;

    public override void OnEnterState()
    {
        owner = (LTFStateMachineRunner)Owner;
    }

    public IEnumerator Spawn()
    {
        yield return new WaitForSeconds(5f);
        GetComponent<LTFFollowState>().SetStartNode(startNode);
        owner.ChangeState(GetComponent<LTFFollowState>());
    }
    
}
