using System;
using UnityEngine;

public class BulletShooting : MonoBehaviour
{
    public float lifetime = 3f;
    public float damage;
    public LayerMask harmableLayer;
    private Collider[] cols = new Collider[20];

    void Start()
    {
        Destroy(gameObject, lifetime); 
    }

    private void FixedUpdate()
    {
        var size = Physics.OverlapSphereNonAlloc(transform.position, 0.2f, cols, harmableLayer, QueryTriggerInteraction.Collide);
        for(var i = 0; i < size; i++)
        {
            var other = cols[i];
            if (other.TryGetComponent<IHarmable>(out var harmable))
            {
                harmable.OnHarm(damage);
            }

            Destroy(gameObject);
        }
    }
}

