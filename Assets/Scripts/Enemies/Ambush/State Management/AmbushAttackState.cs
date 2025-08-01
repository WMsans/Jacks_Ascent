using UnityEngine;
using UnityEngine.Animations;

public class AmbushAttackState : BaseState
{

    private AmbushStateMachineRunner owner;

    private Animator animator;

    private Vector3 startPosition;

    [SerializeField] private float sleepTimer = 2f;
    private float phaseTime;

    [SerializeField] private float retreatTime = 2f;
    [SerializeField] private float attackTime = 3f;

    private bool doneAttacking;
    private bool doneRetreating;

    public override void OnEnterState()
    {
        owner = (AmbushStateMachineRunner)Owner;

        animator = owner.animator;
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", true);

        startPosition = transform.position;

        phaseTime = 0f;

        doneAttacking = false;
        doneRetreating = false;
    }

    public override void OnUpdateState()
    {
        phaseTime += Time.deltaTime;

        if (!doneAttacking) HandleAttack();
        else HandleRetreat();

        // implement logic for determining when TTCOAB stops attacking player
        bool goBackToNapping;

        goBackToNapping = phaseTime >= sleepTimer;

        if (goBackToNapping && doneRetreating)
        {
            // put TTCOAB to bed
            owner.ChangeState(GetComponent<AmbushSleepState>());
        }
    }

    private void HandleAttack()
    {
        if (phaseTime >= attackTime)
        {
            animator.SetBool("isMoving", true);
            animator.SetBool("isAttacking", false);
            doneAttacking = true;
            phaseTime = 0f;
        }
    }

    private void HandleRetreat()
    {
        if (phaseTime > retreatTime)
        {
            doneRetreating = true;
        }

        Vector3 targetPos = startPosition - transform.forward * 8;

        Vector3 nextPos = Vector3.Lerp(startPosition, targetPos, phaseTime / retreatTime);
        transform.position = nextPos;
    }
    
}
