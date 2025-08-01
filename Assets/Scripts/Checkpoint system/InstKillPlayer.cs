using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstKillPlayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent.GetComponent<PlayerHarmController>().OnDeath();
        }
    }
}
