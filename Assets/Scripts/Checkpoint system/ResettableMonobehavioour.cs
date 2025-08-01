using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResettableMonobehavioour : MonoBehaviour, IResettable
{
    protected virtual void Start()
    {
        if (CheckpointManager.Instance)
        {
            CheckpointManager.Instance.onRespawn.AddListener(OnReset);
            OnReset();
        }
        else
        {
            Debug.LogError("Cannot find checkpoint manager. Cannot reset.", this);
        }
    }
    public abstract void OnReset();
}
