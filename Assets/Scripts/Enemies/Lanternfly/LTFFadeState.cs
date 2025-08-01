using UnityEngine;

public class LTFFadeState : BaseState
{
    
    private LTFStateMachineRunner owner;

    public override void OnEnterState()
    {
        owner = (LTFStateMachineRunner)Owner;
        owner.Despawn();
    }
    
}
