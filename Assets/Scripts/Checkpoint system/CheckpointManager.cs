using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }
    public Vector3 CurrentRespawnPoint { get; private set; }
    [Header("Events")] public UnityEvent onRespawn;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetRespawnPoint(Vector3 pt)
    {
        CurrentRespawnPoint = pt;
    }

    public void RespawnPlayer()
    {
        onRespawn.Invoke();
    }
}
