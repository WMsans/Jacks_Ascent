using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class SpiderBaseState : BaseState
{
    [SerializeField] protected NavMeshAgent agent;
}
