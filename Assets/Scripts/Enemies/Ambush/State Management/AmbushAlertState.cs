using UnityEngine;

public class AmbushAlertState : BaseState
{

    private AmbushStateMachineRunner owner;

    private Animator animator;

    private Vector3 startPosition;

    [SerializeField] private float hungryTimer = 2f;
    private float phaseTime;

    [SerializeField] private float emergeTime = 5f;
    [SerializeField] private float stillTime = 1f;

    [SerializeField] private Transform hole;
    private Vector3 holeCenter;
    [SerializeField] private float searchRadius = 5f;

    private bool doneEmerging;

    public override void OnEnterState()
    {
        owner = (AmbushStateMachineRunner)Owner;

        animator = owner.animator;
        animator.SetBool("isMoving", true);
        animator.SetBool("isAttacking", false);

        startPosition = transform.position;

        phaseTime = 0f;

        holeCenter = hole.position;

        doneEmerging = false;
    }

    public override void OnUpdateState()
    {
        phaseTime += Time.deltaTime;

        // make TTCOAB emerge
        if (!doneEmerging) HandleEmerge();

        // implement logic for determining when TTCOAB attacks player
        bool isHungry = TimeToEat();

        if (isHungry && doneEmerging)
        {
            owner.ChangeState(GetComponent<AmbushAttackState>());
        }
    }

    private void HandleEmerge()
    {
        if (phaseTime > emergeTime)
        {
            animator.SetBool("isMoving", false);
            doneEmerging = phaseTime > emergeTime + stillTime;
        }

        Vector3 targetPos = startPosition + transform.forward * 8;

        Vector3 nextPos = Vector3.Lerp(startPosition, targetPos, phaseTime / emergeTime);
        transform.position = nextPos;
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
