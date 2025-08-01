using UnityEngine;

public class LTFStateMachineRunner : StateMachineRunner
{

    public Transform player;

    public Vector3 startPos;

    [SerializeField] ParticleSystem mistEffect;

    void Awake()
    {
        //player = GameObject.FindWithTag("Player").transform;
        mistEffect.Play();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    // despawn the lanternfly swarm
    public void Despawn()
    {
        gameObject.SetActive(false);
        mistEffect.Stop();
    }
    
}
