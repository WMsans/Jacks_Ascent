using UnityEngine;

public class Hook : MonoBehaviour
{
    private Rigidbody rb;
    private ODM odm;
    private bool isAttached = false;
    private LineRenderer lineRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }

    public void Initialize(ODM owner, Vector3 initialVelocity)
    {
        odm = owner;
        rb.linearVelocity = initialVelocity;
    }

    void Update()
    {
        if (odm != null && !isAttached)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, odm.firePos.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isAttached && (collision.gameObject.layer == LayerMask.NameToLayer("Climbable") || collision.gameObject.layer == LayerMask.NameToLayer("Default")))
        {
            isAttached = true;
            rb.isKinematic = true;
            odm.HookAttached(transform.position);
        }
    }

    public void Recall()
    {
        Destroy(gameObject);
    }
}