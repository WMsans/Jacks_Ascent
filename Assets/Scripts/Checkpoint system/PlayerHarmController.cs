using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHarmController : MonoBehaviour
{
    public UnityEvent onDeath;
    public void OnDeath()
    {
        onDeath.Invoke();
        transform.position = CheckpointManager.Instance.CurrentRespawnPoint;
        CheckpointManager.Instance.RespawnPlayer();
    }
}
