using UnityEngine;

public class LanternflyBehavior : MonoBehaviour
{

    [SerializeField] private Transform player;

    private Vector3 startPos;
    private Vector3 jitterFreq;
    private Vector3 jitterMax;

    void Awake()
    {
        //player = GameObject.FindWithTag("Player").transform;

        startPos = transform.localPosition;

        jitterFreq.x = Random.Range(3, 4.5f);
        jitterFreq.y = Random.Range(1.5f, 3);
        jitterFreq.z = Random.Range(1.5f, 3);

        jitterMax.x = Random.Range(0.05f, 0.1f);
        jitterMax.y = Random.Range(0.05f, 0.1f);
        jitterMax.z = Random.Range(0.05f, 0.1f);
    }

    void Update()
    {
        LookAtPlayer();
        Jitter();
    }

    // makes lanternfly hover in place
    private void Jitter()
    {
        Vector3 t = Time.time * jitterFreq;

        Vector3 nextPos = startPos;
        nextPos.x += jitterMax.x * Mathf.Sin(t.x);
        nextPos.y += jitterMax.y * Mathf.Sin(t.y);
        nextPos.z += jitterMax.z * Mathf.Sin(t.z);

        Vector3 velocity = Vector3.Normalize(nextPos - transform.localPosition) * 3f;
        nextPos = Vector3.SmoothDamp(transform.localPosition, nextPos, ref velocity, 0.3f);
        transform.localPosition = nextPos;
    }


    // makes lanternfly face player
    private void LookAtPlayer()
    {
        transform.LookAt(player, Vector3.up);
        transform.Rotate(180, 90, 0);
    }
}
