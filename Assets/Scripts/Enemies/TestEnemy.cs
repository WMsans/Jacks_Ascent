using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour, IHarmable
{
    public float CurrentHp { get; }
    public void OnHarm(float damage)
    {
        Debug.Log("Test enemy harmed");
    }
}
