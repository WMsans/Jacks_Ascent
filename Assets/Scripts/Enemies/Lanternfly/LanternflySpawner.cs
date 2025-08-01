using System.Collections;
using UnityEngine;

public class LanternflySpawner : MonoBehaviour
{

    [SerializeField] private Transform player;

    [SerializeField] private GameObject swarm;

    [SerializeField] private ParticleSystem burstEffect;
    [SerializeField] private ParticleSystem mistEffect;

    bool hasActivated = false;

    void Update()
    {
        if (!hasActivated && (Vector3.Distance(transform.position, player.position) <= 0.8f))
        {
            burstEffect.Play();
            mistEffect.Play();
            Deactivate();
            StartCoroutine(ActiveAfterSecs());
        }
    }

    private IEnumerator ActiveAfterSecs()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(swarm.GetComponent<LTFSpawnState>().Spawn());
    }

    private void Deactivate()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        hasActivated = true;
    }

}
