using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlaySound : MonoBehaviour
{
    public UnityEvent playSoundEvent;

    private void OnEnable()
    {
        playSoundEvent.Invoke();
        this.enabled = false;
    }
}
