using UnityEngine;

public class AmbushSleepState : BaseState
{

    private AmbushStateMachineRunner owner;

    private Animator animator;

    [SerializeField] private float wakeUpTimer = 2f;

    private float phaseTime;

    [SerializeField] private Transform hole;
    private Vector3 holeCenter;
    [SerializeField] private float searchRadius = 15f;

    public override void OnEnterState()
    {
        owner = (AmbushStateMachineRunner)Owner;

        animator = owner.animator;
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", false);

        phaseTime = 0f;

        holeCenter = hole.position;
    }

    public override void OnUpdateState()
    {
        phaseTime += Time.deltaTime;

        // implement logic for determining when TTCOAB detects player
        bool wokeUp = TimeToEat();

        if (wokeUp)
        {
            owner.ChangeState(GetComponent<AmbushAlertState>());
        }
    }

    private bool TimeToEat()
    {
        Collider[] hitColliders = Physics.OverlapSphere(holeCenter, searchRadius);
        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

}
