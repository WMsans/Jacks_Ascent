using UnityEngine;

public class CentipedeTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool centipedeTriggered = false;
    [SerializeField] private GameObject centipedePrefab;
    [SerializeField] private Transform spawnPos;

    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            centipedeTriggered = true;
            Instantiate(centipedePrefab, spawnPos.position, spawnPos.rotation);
        }
    }
}
