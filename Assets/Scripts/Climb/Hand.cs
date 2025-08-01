using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class Hand : MonoBehaviour
{
    private static readonly int Glow = Shader.PropertyToID("_Glow");
    public Renderer meshRenderer;
    private Material _mat;

    private void Awake()
    {
        if (!meshRenderer) meshRenderer = GetComponent<Renderer>();
        _mat = meshRenderer.material;
    }

    public void ChangeGreen(bool grabbed)
    {
        if (grabbed)
        {
            DOTween.To(() => _mat.GetFloat(Glow), x => _mat.SetFloat(Glow, x), 1f, 0.25f);
        }
        else
        {
            DOTween.To(() => _mat.GetFloat(Glow), x => _mat.SetFloat(Glow, x), 0f, 0.25f);
        }
    }
}
