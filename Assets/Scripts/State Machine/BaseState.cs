using UnityEngine;

public abstract class BaseState : MonoBehaviour
{
    protected StateMachineRunner Owner { get; private set; }
    public virtual void OnEnterState(){}
    public virtual void OnExitState(){}
    public virtual void OnUpdateState(){}
    public virtual void OnFixedUpdateState(){}
    public virtual void OnLateUpdateState(){}

    public void SetOwner(StateMachineRunner newOwner)
    {
        Owner = newOwner;
    }
}
