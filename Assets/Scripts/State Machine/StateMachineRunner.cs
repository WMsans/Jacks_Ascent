using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineRunner : MonoBehaviour
{
    [SerializeField] private BaseState initialState;
    public BaseState CurrentState { get; private set; }
    public virtual void ChangeState(BaseState next)
    {
        if(!next) return;
        CurrentState?.OnExitState();
        CurrentState = next;
        CurrentState.SetOwner(this);
        CurrentState.OnEnterState();
    }

    protected virtual void Start()
    {
        ChangeState(initialState);
    }

    protected virtual void Update()
    {
        CurrentState?.OnUpdateState();
    }

    protected virtual void FixedUpdate()
    {
        CurrentState?.OnFixedUpdateState();
    }

    protected virtual void LateUpdate()
    {
        CurrentState?.OnLateUpdateState();
    }
}
