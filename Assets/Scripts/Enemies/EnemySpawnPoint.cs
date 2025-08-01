using System;
using UnityEngine;

public class EnemySpawnPoint : ResettableMonobehavioour
{
    [SerializeField] private GameObject enemyPrefab;
    private GameObject livingEnemy;
    
    public override void OnReset()
    {
        if (livingEnemy)
        {
            Destroy(livingEnemy);
        }
        livingEnemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
}
