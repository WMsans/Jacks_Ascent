using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class GunEffects : MonoBehaviour
{
    [SerializeField] private GameObject effect;
    [Button]
    public void OnGunShoot()
    {
        Instantiate(effect, transform);
        
    }
}
