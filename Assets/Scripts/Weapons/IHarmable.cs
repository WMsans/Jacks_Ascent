using UnityEngine;

public interface IHarmable
{
    float CurrentHp { get; }
    void OnHarm(float damage);
}
