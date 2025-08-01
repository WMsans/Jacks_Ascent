using UnityEngine;

public class SpiderHealth : MonoBehaviour, IHarmable
{
    [SerializeField] private StateMachineRunner stateMachine;
    [SerializeField] private SpiderEscapeState escapeState;

    [SerializeField] private ParticleSystem hitEffect;

    public float CurrentHp { get; private set; } = 1f;

    public void OnHarm(float damage)
    {
        hitEffect.Play();
        stateMachine.ChangeState(escapeState);
    }
}