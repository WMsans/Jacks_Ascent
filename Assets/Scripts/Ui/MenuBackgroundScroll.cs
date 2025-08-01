using System;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using UnityEngine;

public class MenuBackgroundScroll : MonoBehaviour
{
    [SerializeField] private Transform stPoint;
    [SerializeField] private Transform edPoint;
    [SerializeField] private float downSpeed;


    private void FixedUpdate()
    {
        if (Vector3.Distance(edPoint.position, transform.position) < 1f)
        {
            transform.position = stPoint.position;
        }

        transform.position -= Vector3.up * (downSpeed * Time.fixedDeltaTime);
    }
}
