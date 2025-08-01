using UnityEngine;

public class PlayerHandAnimationController : MonoBehaviour
{

    private Animator animator;

    [SerializeField] private float grabSpeed = 8f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void HandGrabbed()
    {
        animator.SetTrigger("grab");
    }

    public void HandReleased()
    {
        animator.SetTrigger("release");
    }
}
